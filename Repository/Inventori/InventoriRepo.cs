using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using POS_System.DbCtx;
using POS_System.Models;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public class InventoriRepo : IInventoriRepo
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction _transaction;
        public InventoriRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Produk> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Produks
                // Melakukan Filtered Include hanya untuk satuan yang Is_Default == true
                .Include(p => p.ProductUnits.Where(pu => pu.Is_Default))
                .Where(pu => pu.KodeBarcode == barcode || pu.SKU == barcode)
                .FirstOrDefaultAsync();
        }
        public async Task<List<LaporanInventoriViewModel>> GetLaporanInventoriAsync(DateTime startDate, DateTime endDate, string tipeString, string tipeLabel)
        {
            // Ambil master sumber stok ke memory
            var sumberLookup = await _context.SumberStoks
                .AsNoTracking()
                .ToDictionaryAsync(
                    x => x.Id.ToString(),
                    x => x.Nama ?? "-"
                );
            // Ambil data inventori
            var data = await _context.Inventoris
                .AsNoTracking()
                .Where(inv =>
                    inv.TipeStok == tipeString &&
                    inv.Tanggal >= startDate.Date &&
                    inv.Tanggal <= endDate.Date.AddDays(1).AddTicks(-1))
                .Join(
                    _context.Produks,
                    inv => inv.IdProduk,
                    p => p.IdProduk,
                    (inv, p) => new { inv, p })
                .Join(
                    _context.ProductUnits,
                    x => x.inv.ProductUnitId,
                    pu => pu.ProductUnitId,
                    (x, pu) => new
                    {
                        x.inv,
                        x.p,
                        pu
                    })
                .OrderBy(x => x.inv.Tanggal)
                .ToListAsync();
            var result = data.Select(x => new LaporanInventoriViewModel
            {
                NoReferensi = x.inv.NoReferensi,
                TipeStok = tipeLabel,
                Sumber = sumberLookup.TryGetValue(
                            x.inv.Sumber ?? "",
                            out var sumberNama)
                         ? sumberNama
                         : "-",
                Tanggal = x.inv.Tanggal,
                NamaProduk = x.p.NamaProduk,
                SatuanUnit = x.pu.UnitName,
                JumlahProduk = x.inv.Jumlah,
                HargaModal = x.inv.HargaModal,
                TotalModal = x.inv.TotalModal,
                Catatan = x.inv.Notes
            })
            .ToList();
            return result;
        }
        public async Task<(List<Inventori> Data, int TotalCount)> GetPaginatedInventoriAsync(int pageNumber, int pageSize)
        {
            // Filter data yang rusak (NoReferensi null) langsung di level database
            var query = _context.Inventoris
                .Include(i => i.Produk) // Pastikan relasi ke produk ada
                .Where(i => i.NoReferensi != null && i.NoReferensi != "") // Data yang rusak tidak usah diambil
                .OrderByDescending(i => i.IdInventori);
            int totalCount = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
        public async Task<List<TipeStok>> GetAllTipeStokAsync() => await _context.TipeStoks.ToListAsync();
        public async Task<List<SumberStok>> GetAllSumberStokAsync() => await _context.SumberStoks.ToListAsync();
        public async Task<List<InventoriItemPartialVm>> GetDetailProdukByNoReferensiAsync(string noReferensi)
        {
            return await _context.Inventoris
                .Include(p => p.Produk)
                .Include(pu => pu.ProductUnit)
                .Where(x => x.NoReferensi == noReferensi)
                .Select(p => new InventoriItemPartialVm
                {
                    Index = 0,
                    Item = new InventoriVm
                    {
                        IdInventori = p.IdInventori,
                        IdProduk = p.IdProduk,
                        NamaProduk = p.Produk.NamaProduk,
                        Stok = p.Stok,
                        HargaModal = p.HargaModal,
                        Jumlah = p.Jumlah,
                        ProductUnitId = p.ProductUnitId,
                        UnitName = p.ProductUnit.UnitName
                    }
                }).ToListAsync();
        }
        public async Task<List<ProdukVm>> GetAllProdukVmAsync()
        {
            return await _context.Produks
                // Filter produk yang memiliki setidaknya satu unit dasar (Is_Default == 1 atau true)
                .Where(p => p.ProductUnits.Any(pu => pu.Is_Default))
                .Select(p => new ProdukVm
                {
                    IdProduk = p.IdProduk,
                    NamaProduk = p.NamaProduk,
                    Stok = p.Stok,
                    // Mengambil NamaUnit dasar/default dari unit yang aktif
                    UnitName = p.ProductUnits
                                   .Where(pu => pu.Is_Default)
                                   .Select(pu => pu.UnitName)
                                   .FirstOrDefault()
                })
                .ToListAsync();
        }
        public async Task<Inventori> GetInventoriByIdAsync(long id) =>
            await _context.Inventoris.FirstOrDefaultAsync(x => x.IdInventori == id);
        public async Task<List<Inventori>> GetInventoriByNoReferensiAsync(string noReferensi)
        {
            return await _context.Inventoris
                .Include(x => x.ProductUnit)
                .Include(x => x.Produk)
                .Where(x => x.NoReferensi == noReferensi)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<int> GetCountNoReferensiTodayAsync(string prefix, string today) =>
            await _context.Inventoris.CountAsync(x => x.NoReferensi != null && x.NoReferensi.StartsWith(prefix + "-" + today));
        public async Task<Produk> GetProdukWithUnitsAsync(long idProduk) =>
            await _context.Produks.Include(pu => pu.ProductUnits).FirstOrDefaultAsync(p => p.IdProduk == idProduk);
        public async Task<ProductUnit> GetProductUnitByIdAsync(long productUnitId) =>
            await _context.ProductUnits.FindAsync(productUnitId);
        public async Task AddInventoriAsync(Inventori entity) => await _context.Inventoris.AddAsync(entity);
        public void UpdateInventori(Inventori entity) => _context.Inventoris.Update(entity);
        public void RemoveInventori(Inventori entity) => _context.Inventoris.Remove(entity);
        public void UpdateProductUnit(ProductUnit productUnit) => _context.ProductUnits.Update(productUnit);
        public async Task<Produk?> GetProdukByIdAsync(long id)
        {
            return await _context.Produks
                .Include(p => p.ProductUnits)
                .Include(p => p.ProdukGambars)
                //.Include(p => p.Kategori)
                .FirstOrDefaultAsync(p => p.IdProduk == id);
        }
        public async Task<List<SumberStok>> GetSumberByTipeAsync(int tipeStokId) =>
            await _context.SumberStoks.Where(s => s.TipeStokId == tipeStokId).ToListAsync();
        public async Task<dynamic> GetProdukDetailJsonAsync(int id)
        {
            var produk = await _context.Produks
                .Include(p => p.ProductUnits)
                .FirstOrDefaultAsync(p => p.IdProduk == id);
            if (produk == null) return null;
            var defaultUnit = produk.ProductUnits.FirstOrDefault(u => u.Is_Default == true);
            // Kirim dengan struktur yang dikenali mapProductToCard
            return new
            {
                idProduk = produk.IdProduk,
                namaProduk = produk.NamaProduk,
                stok = produk.Stok,
                // Ini kunci perbaikannya:
                productUnit = defaultUnit != null ? new
                {
                    productUnitId = defaultUnit.ProductUnitId,
                    unitName = defaultUnit.UnitName,
                    hargaModal = defaultUnit.HargaModal
                } : null
            };
        }
        public async Task<object> SearchProdukAsync(string term)
        {
            return await _context.Produks
                .Where(p => p.NamaProduk.Contains(term))
                .Select(p => new { id = p.IdProduk, nama = p.NamaProduk })
                .ToListAsync();
        }
        public async Task<bool> SaveChangesAsync() => await _context.SaveChangesAsync() > 0;
        public async Task BeginTransactionAsync() => _transaction = await _context.Database.BeginTransactionAsync();
        public async Task CommitTransactionAsync() => await _transaction.CommitAsync();
        public async Task RollbackTransactionAsync() => await _transaction.RollbackAsync();
        public void UpdateInventoriEdit(Inventori entity)
        {
             _context.Entry(entity).State = EntityState.Modified;
        }
        public async Task RemoveInventoriByIdAsync(long idInventori)
        {
            var item = await _context.Inventoris.FindAsync(idInventori);
            if (item != null)
            {
                _context.Inventoris.Remove(item);
            }
        }
        public void UpdateProducts(Produk product)
        {            
            _context.Entry(product).State = EntityState.Modified;
        }
    }
}