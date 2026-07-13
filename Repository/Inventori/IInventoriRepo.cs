using POS_System.Models;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public interface IInventoriRepo
    {
        Task RemoveInventoriByIdAsync(long idInventori);
        void UpdateInventoriEdit(Inventori entity);
        Task<Produk> GetProductByBarcodeAsync(string barcode);
        Task<List<LaporanInventoriViewModel>> GetLaporanInventoriAsync(DateTime startDate, DateTime endDate, string tipeString, string tipeLabel);
        Task<(List<Inventori> Data, int TotalCount)> GetPaginatedInventoriAsync(int pageNumber, int pageSize);
        Task<List<TipeStok>> GetAllTipeStokAsync();
        Task<List<SumberStok>> GetAllSumberStokAsync();
        Task<List<InventoriItemPartialVm>> GetDetailProdukByNoReferensiAsync(string noReferensi);
        Task<List<ProdukVm>> GetAllProdukVmAsync();
        Task<Inventori> GetInventoriByIdAsync(long id);
        Task<List<Inventori>> GetInventoriByNoReferensiAsync(string noReferensi);
        Task<int> GetCountNoReferensiTodayAsync(string prefix, string today);
        Task<Produk> GetProdukWithUnitsAsync(long idProduk);
        Task<ProductUnit> GetProductUnitByIdAsync(long productUnitId);
        Task AddInventoriAsync(Inventori entity);
        void UpdateInventori(Inventori entity);
        void RemoveInventori(Inventori entity);
        void UpdateProductUnit(ProductUnit productUnit);
        void UpdateProducts(Produk product);
        Task<Produk?> GetProdukByIdAsync(long id);
        Task<List<SumberStok>> GetSumberByTipeAsync(int tipeStokId);
        Task<object> GetProdukDetailJsonAsync(int id);
        Task<object> SearchProdukAsync(string term);
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
