using Microsoft.EntityFrameworkCore;
using POS_System.Models;
using Microsoft.EntityFrameworkCore.Storage;
using POS_System.DbCtx;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public class TransaksiRepo : ITransaksiRepo
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        public TransaksiRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Produk> GetProdukByIdAsync(long id)
        {
            return await _context.Produks
                .Include(p => p.ProductUnits)
                .Include(p => p.ProdukGambars)
                //.Include(p => p.Kategori)
                .FirstOrDefaultAsync(p => p.IdProduk == id);
        }
        public async Task<bool> EditTransaksiWithDetailsAsync(TransaksiVm model)
        {
            await BeginTransactionAsync();
            try
            {
                //Ambil transaksi lama beserta detailnya
                var transaksi = await GetWithDetailsAsync(model.IdTransaksi);
                if (transaksi == null) return false;
                // Proteksi tambahan di tingkat repository
                if (transaksi.Status == "Dibatalkan")
                {
                    throw new Exception("Gagal Edit: Transaksi ini sudah berstatus Dibatalkan.");
                }
                //Ambil semua ID produk unik yang terlibat
                var allProductIds = transaksi.TransaksiDetails.Select(x => x.IdProduk)
                                    .Concat(model.Items.Select(x => x.IdProduk))
                                    .Distinct()
                                    .OrderBy(id => id) // PENTING: Mencegah Deadlock
                                    .ToList();
                // 2. Load semua produk ke Memory (untuk menghindari N+1 query)
                var products = await GetProduksForUpdateByIdsAsync(allProductIds);
                //Kembalikan stok lama sebelum detailnya dihapus
                foreach (var oldDetail in transaksi.TransaksiDetails)
                {
                    var productUnit = await GetProdukByIdAsync(oldDetail.IdProduk);
                    if (productUnit != null)
                    {
                        // Cari produk langsung dari list 'products' yang sudah di-load di memory
                        var p = products.FirstOrDefault(x => x.IdProduk == oldDetail.IdProduk);
                        if (p != null)
                        {
                            //Ambil konversi dari ProductUnit yang di-include jika oldDetail.Conversion bernilai 0
                            var conversion = oldDetail.Conversion > 0
                                ? oldDetail.Conversion
                                : (oldDetail.ProductUnit?.ConversionValue ?? 1); // default ke 1 jika pcs
                            // Kembalikan stok lama ke memory
                            p.Stok += (oldDetail.Qty * conversion);
                        }
                    }
                }
                // Update data transaksi & Hapus detail lama
                RemoveTransaksiDetails(transaksi.TransaksiDetails);
                transaksi.IdPelanggan = model.IdPelanggan;
                transaksi.TotalBelanja = model.TotalBelanja;
                UpdateTransaksi(transaksi);
                //Tambahkan detail transaksi yang baru dimasukkan dari model.Items
                if (model.Items != null)
                {
                    //Cek stok untuk item baru
                    foreach (var item in model.Items)
                    {
                        // Hitung total fisik pcs/dus yang keluar
                        // Multi satuan produk Stok dikurangi pakai satuan dasar unit
                        // 1 Pcs = 1 , 1 Dus = 40 Pcs 
                        var p = products.FirstOrDefault(x => x.IdProduk == item.IdProduk);
                        if (p == null || p.Stok < (item.Qty * item.Conversion))
                        {
                            throw new Exception("Stok tidak mencukupi untuk " + item.IdProduk);
                        }
                        p.Stok -= (item.Qty * item.Conversion);
                        var detail = new TransaksiDetail
                        {                            
                            IdTransaksi = transaksi.IdTransaksi,
                            IdProduk = item.IdProduk,
                            ProductUnitId = item.ProductUnitId,
                            Qty = item.Qty,
                            HargaJual = item.HargaJual,
                            HargaModal = item.HargaModal,
                            Profit = item.Profit,
                            Conversion=item.Conversion
                        };
                        await AddTransaksiDetailAsync(detail);
                    }
                }
                // Simpan semua perubahan dan kunci transaksi database
                await SaveChangesAsync();
                await CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                await RollbackTransactionAsync();
                throw;
            }
        }
        public async Task<Transaksi?> GetByIdAsync(long id) =>
            await _context.Transaksis.FindAsync(id);
        public async Task<Transaksi?> GetWithDetailsAsync(long id)
        {
            return await _context.Transaksis
                .Include(t => t.Pelanggan)
                .Include(t => t.MetodeBayar)
                .Include(t => t.Karyawans)
                .Include(t => t.TransaksiDetails)
                    .ThenInclude(d => d.ProductUnit)
                    .ThenInclude(p => p.Produk)
                .FirstOrDefaultAsync(t => t.IdTransaksi == id);
        }
        public async Task<List<Pelanggan>> GetPelangganListAsync() =>
            await _context.Pelanggans.ToListAsync();
        public async Task<List<MetodeBayar>> GetMetodeBayarListAsync() =>
            await _context.MetodeBayars.ToListAsync();
        public async Task<ProductUnit?> GetProductUnitByIdAsync(long productUnitId) =>
            await _context.ProductUnits.FirstOrDefaultAsync(pu => pu.ProductUnitId == productUnitId);
        public async Task<List<TransaksiDetail>> GetDetailsByTransaksiIdAsync(long idTransaksi)
        {
            return await _context.TransaksiDetails
                .Include(d => d.Produk)
                .Include(d => d.ProductUnit)
                .Where(d => d.IdTransaksi == idTransaksi)
                .ToListAsync();
        }
        public async Task<(List<Transaksi> Data, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Transaksis
                .Include(t => t.Pelanggan)
                .Include(t => t.Karyawans)
                .Include(t => t.MetodeBayar)
                .Include(td => td.TransaksiDetails)
                .OrderByDescending(t => t.IdTransaksi);
            int totalCount = await query.CountAsync();
            var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (data, totalCount);
        }
        public async Task<dynamic?> GetProdukDetailAsync(long id)
        {
            var produk = await _context.Produks
                .Include(x => x.ProductUnits)
                .FirstOrDefaultAsync(x => x.IdProduk == id);
            if (produk == null) return null;
            return new
            {
                idProduk = produk.IdProduk,
                barcode = produk.KodeBarcode,
                namaProduk = produk.NamaProduk,
                stok = produk.Stok,
                availableUnits = produk.ProductUnits
                    .Where(x => !x.IsDeleted)
                    .Select(x => new
                    {
                        productUnitId = x.ProductUnitId,
                        unitName = x.UnitName,
                        hargaModal = x.HargaModal,
                        hargaJual = x.HargaJual
                    }).ToList()
            };
        }
        public async Task<List<Pelanggan>> SearchPelangganAsync(string term) =>
            await _context.Pelanggans.Where(p => p.Nama.Contains(term)).ToListAsync();
        public async Task<object?> GetPelangganDetailAsync(int id)
        {
            return await _context.Pelanggans
                .Where(p => p.Id == id)
                .Select(p => new { p.Email, p.Phone })
                .FirstOrDefaultAsync();
        }
        public async Task<Karyawan?> GetKaryawanByUserIdAsync(string userId) =>
            await _context.Karyawans.FirstOrDefaultAsync(x => x.UserId == userId);
        public async Task<PengaturanToko?> GetPengaturanTokoAsync() =>
            await _context.PengaturanTokos.FirstOrDefaultAsync();
        public async Task UpdateProductUnitAsync(ProductUnit productUnit)
        {
            _context.ProductUnits.Update(productUnit);
            await Task.CompletedTask;
        }
        public async Task UpdateProduk(Produk produk)
        {
            _context.Produks.Update(produk);
            await Task.CompletedTask;
        }
        public void UpdateTransaksi(Transaksi transaksi) =>
            _context.Entry(transaksi).State = EntityState.Modified;
        public async Task AddTransaksiAsync(Transaksi transaksi) =>
            await _context.Transaksis.AddAsync(transaksi);
        public async Task AddTransaksiDetailAsync(TransaksiDetail detail) =>
            await _context.TransaksiDetails.AddAsync(detail);
        public void RemoveTransaksiDetails(IEnumerable<TransaksiDetail> details) =>
            _context.TransaksiDetails.RemoveRange(details);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();
        public async Task CommitTransactionAsync() { if (_transaction != null) await _transaction.CommitAsync(); }
        public async Task RollbackTransactionAsync() { if (_transaction != null) await _transaction.RollbackAsync(); }
        public async Task<Produk> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Produks
                .Include(p => p.ProductUnits)
                .Where(pu => pu.KodeBarcode == barcode)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Produk>> SearchProdukAsync(string term)
        {
            return await _context.Produks
            .Where(p => p.NamaProduk.Contains(term))
            .Take(10)
            .ToListAsync();
        }
        public async Task<List<Produk>> GetAllProdukAsync(List<long> Id)
        {
            return await _context.Produks
        .Include(x => x.ProductUnits) // Tetap sertakan jika perlu data unitnya
        .Where(p => Id.Contains(p.IdProduk))
        .ToListAsync();
        }
        public async Task<List<Produk>> GetProduksForUpdateByIdsAsync(List<long> ids)
        {
            // String untuk ID: "1, 2, 3"
            string idList = string.Join(",", ids);
            // Query ini akan mengunci semua baris produk yang terlibat dalam SATU perintah
            return await _context.Produks
                .FromSqlRaw($"SELECT * FROM Produks WHERE IdProduk IN ({idList}) FOR UPDATE")
                .ToListAsync();
        }
    }
}
