using POS_System.DbCtx;
using POS_System.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using POS_System.ViewModels;
using Microsoft.CodeAnalysis;
namespace POS_System.Repository
{
    public class KasirRepo : IKasirRepo
    {
        private readonly AppDbContext _context;
        public KasirRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Produk?> GetProdukByIdAsync(long id)
        {
            return await _context.Produks
                .Include(p => p.ProductUnits)
                .Include(p => p.ProdukGambars)
                //.Include(p => p.Kategori)
                .FirstOrDefaultAsync(p => p.IdProduk == id);
        }
        public async Task<ProductUnit?> GetProductUnitByIdAsync(long ProductUnitId)
        {
            var unit = _context.ProductUnits.FindAsync(ProductUnitId);
            return await unit;
        }
        public async Task<object> GetUnitDetailsAsync(int productId)
        {
            return await _context.Produks
                .Include(p => p.ProductUnits)                
                .Where(pu => pu.IdProduk == productId)
                .Select(pu => new
                {
                    pu.ProductUnitId,
                    pu.UnitName,
                    pu.HargaJual,
                    pu.Stok
                })
                .ToListAsync();
        }
        public async Task<object> GetProductByBarcodeAsync(string barcode)
        {
            return await _context.Produks
                .Include(pu => pu.ProductUnits)
                .Include(pu => pu.ProdukGambars)
                .Where(pu => pu.KodeBarcode == barcode || pu.SKU == barcode)
                .FirstOrDefaultAsync();
        }
        public async Task<(int totalCount, List<object> items)> SearchProductsAsync(int page, int pageSize, string query, int? categoryId)
        {
            var productsQuery = _context.Produks
                .Include(g => g.ProdukGambars)
                .Include(u => u.ProductUnits)
                .OrderByDescending(x => x.IdProduk)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
                productsQuery = productsQuery.Where(p => p.NamaProduk.Contains(query) || p.SKU.Contains(query));
            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.IdKategori == categoryId.Value);
            int count = await productsQuery.CountAsync();
            var paginatedProducts = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (count, paginatedProducts.Cast<object>().ToList());
        }
        public async Task<(int totalCount, List<object> items)> LoadPageAsync(int pageNumber, int pageSize)
        {
            var productsQuery = _context.Produks
                .OrderByDescending(x => x.IdProduk)
                .Include(pg => pg.ProdukGambars)
                .Include(pu => pu.ProductUnits)
                .AsNoTracking();
            int count = await productsQuery.CountAsync();
            var paginatedProducts = await productsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (count, paginatedProducts.Cast<object>().ToList());
        }
        public async Task<List<PelangganVm>> GetPelangganListAsync()
        {
            return await _context.Pelanggans
                .OrderByDescending(p => p.IsPinned)
                .ThenBy(p => p.Nama) // Opsional: Tambahkan urutan kedua jika IsPinned sama
                .Select(p => new PelangganVm
                {
                    Id = p.Id,
                    Nama = p.Nama
                })
                .ToListAsync();
        }
        //public async Task<List<object>> GetPelangganListAsync()
        //{
        //    return await _context.Pelanggans.
        //        Select(p => new { Id = p.Id, Name = p.Nama }).Cast<object>().ToListAsync();
        //}
        public async Task<List<object>> GetMetodeBayarListAsync()
        {
            return await _context.MetodeBayars.Select(m => new { m.IdMetodeBayar, m.NamaMetodeBayar }).Cast<object>().ToListAsync();
        }
        public async Task<List<object>> GetProductUnitsDropdownAsync()
        {
            return await _context.ProductUnits.Cast<object>().ToListAsync();
        }
        public async Task<object> GetKaryawanByUserIdAsync(string userId)
        {
            return await _context.Karyawans.FirstOrDefaultAsync(x => x.UserId == userId);
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<object> GetProductUnitForUpdateAsync(int productUnitId)
        {
            return await _context.ProductUnits.FirstOrDefaultAsync(pu => pu.ProductUnitId == productUnitId);
        }
        public async Task AddTransaksiAsync(object transaksi)
        {
            await _context.Transaksis.AddAsync((Transaksi)transaksi);
        }
        public async Task AddTransaksiDetailAsync(object detail)
        {
            await _context.TransaksiDetails.AddAsync((TransaksiDetail)detail);
        }
        public void UpdateProductUnit(object productUnit)
        {
            _context.ProductUnits.Update((ProductUnit)productUnit);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<object> GetTransaksiForPrintAsync(int id)
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
        public async Task<object> GetPengaturanTokoAsync()
        {
            return await _context.PengaturanTokos.FirstOrDefaultAsync();
        }
        public void UpdateProduk(Produk produk)
        {
            _context.Produks.Update(produk);
        }
    }
}