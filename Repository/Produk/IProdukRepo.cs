using POS_System.Models;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public interface IProdukRepo
    {
        Task<bool> IsBarcodeSudahAda(string barcode);
        Task<IEnumerable<LaporanStokDto>> GetLaporanStokAsync(string filter);
        Task<ProductUnit> GetProdukUnitById(int id);
        IQueryable<Produk> GetProduksQuery();
        IQueryable<Produk> GetProduksWithFiltersQuery();
        IQueryable<Produk> GetFilteredProduksQuery(string filter, string searchTerm);
        Task<List<Kategori>> GetKategoriListAsync();
        Task<Produk?> GetByIdAsync(long id);
        Task<ProductUnit?> GetUnitByIdAsync(int id);
        Task AddAsync(Produk produk);
        Task AddUnitAsync(ProductUnit unit);
        Task AddGambarAsync(ProdukGambar gambar);
        void UpdateProduk(Produk produk);
        void UpdateUnit(ProductUnit unit);
        Task<bool> SaveChangesAsync();
    }
}
