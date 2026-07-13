using POS_System.DbCtx;
using POS_System.Models;
using Microsoft.EntityFrameworkCore;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public class ProdukRepo : IProdukRepo
    {
        private readonly AppDbContext _context;
        public ProdukRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task <ProductUnit> GetProdukUnitById(int id)
        {
           return await _context.ProductUnits.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<LaporanStokDto>> GetLaporanStokAsync(string filter)
        {
            // 1. Mulai query dari Produk
            var query = _context.Produks.AsQueryable();
            // 2. Sesuaikan logika Filter berdasarkan Stok di tabel Produk (pu di sini adalah Produk)
            switch (filter?.ToLower())
            {
                case "banyak":
                    query = query.Where(pu => pu.Stok >= 50);
                    break;
                case "sedikit":
                    query = query.Where(pu => pu.Stok <= 5 && pu.Stok > 0);
                    break;
                case "habis":
                    query = query.Where(pu => pu.Stok == 0);
                    break;
            }
            // 3. Gunakan SelectMany untuk membongkar ICollection<ProductUnit> ke DTO
            return await query
                .SelectMany(
                p => p.ProductUnits
                .Where(u => u.Is_Default == true), 
                (p, u) => new LaporanStokDto
                {
                    IdProduk = p.IdProduk,
                    NamaProduk = p.NamaProduk,
                    KodeBarcode = p.KodeBarcode,
                    SKU = p.SKU,
                    NamaSatuan = u.UnitName, // 'u' merujuk ke data single ProductUnit
                    Stok = p.Stok,
                    HargaModal = u.HargaModal,
                    HargaJual = u.HargaJual,
                    StatusProduk = p.StatusProduk
                })
                .OrderBy(x => x.NamaProduk)
                .ToListAsync();
        }
        // Logika Pencarian & Filter Stok dipisahkan ke sini
        public IQueryable<Produk> GetFilteredProduksQuery(string filter, string searchTerm)
        {
            // Ambil kueri dasar terlebih dahulu
            var query = GetProduksQuery();
            // Prosedur Filter Kata Kunci Pencarian (Search Term)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLower();
                query = query.Where(p => p.NamaProduk.ToLower().Contains(searchTerm) ||
                                         p.SKU.ToLower().Contains(searchTerm));
            }
            // Prosedur Filter Kategori Stok
            if (!string.IsNullOrEmpty(filter) && filter != "all")
            {
                switch (filter.ToLower())
                {
                    case "banyak":
                        query = query.Where(p => p.Stok >= 50);
                        break;
                    case "sedikit":
                        query = query.Where(p => p.Stok <= 5 && p.Stok > 0);
                        break;
                    case "habis":
                        query = query.Where(p => p.Stok == 0);
                        break;
                }
            }
            // Urutkan data berdasarkan ID Terbesar/Terbaru
            return query.OrderByDescending(p => p.IdProduk);
        }
        public IQueryable<Produk> GetProduksQuery()
        {
            return _context.Produks
                .Include(p => p.ProductUnits)
                .Include(p => p.ProdukGambars)
                .AsQueryable();
        }
        public IQueryable<Produk> GetProduksWithFiltersQuery()
        {
            return _context.Produks.IgnoreQueryFilters().AsQueryable();
        }
        public async Task<List<Kategori>> GetKategoriListAsync()
        {
            return await _context.Kategoris.ToListAsync();
        }
        public async Task<Produk?> GetByIdAsync(long id)
        {
            return await _context.Produks
                .Include(p => p.ProductUnits)
                .Include(p => p.ProdukGambars)
                //.Include(p => p.Kategori)
                .FirstOrDefaultAsync(p => p.IdProduk == id);
        }
        public async Task<ProductUnit?> GetUnitByIdAsync(int id)
        {
            return await _context.ProductUnits.FindAsync(id);
        }
        public async Task AddAsync(Produk produk)
        {
            await _context.Produks.AddAsync(produk);
        }
        public async Task AddUnitAsync(ProductUnit unit)
        {
            await _context.ProductUnits.AddAsync(unit);
        }
        public async Task AddGambarAsync(ProdukGambar gambar)
        {
            await _context.ProdukGambars.AddAsync(gambar);
        }
        public void UpdateProduk(Produk produk)
        {
            _context.Produks.Update(produk);
        }
        public void UpdateUnit(ProductUnit unit)
        {
            _context.ProductUnits.Update(unit);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> IsBarcodeSudahAda(string barcode)
        {
            // AnyAsync menghasilkan bool, cocok dengan return type Task<bool>
            return await _context.Produks.AnyAsync(p => p.KodeBarcode == barcode);
        }
    }
}