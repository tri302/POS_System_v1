using Microsoft.EntityFrameworkCore.Storage;
using POS_System.Models;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public interface IKasirRepo
    {
        void UpdateProduk(Produk produk);
        Task<ProductUnit?> GetProductUnitByIdAsync(long ProductUnitId);
        Task<Produk?> GetProdukByIdAsync(long id);
        Task<object> GetUnitDetailsAsync(int productId);
        Task<object> GetProductByBarcodeAsync(string barcode);
        Task<(int totalCount, List<object> items)> SearchProductsAsync(int page, int pageSize, string query, int? categoryId);
        Task<(int totalCount, List<object> items)> LoadPageAsync(int pageNumber, int pageSize);
        Task<List<PelangganVm>> GetPelangganListAsync();
        Task<List<object>> GetMetodeBayarListAsync();
        Task<List<object>> GetProductUnitsDropdownAsync();
        Task<object> GetKaryawanByUserIdAsync(string userId);
        // Transaction methods
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<object> GetProductUnitForUpdateAsync(int productUnitId);
        Task AddTransaksiAsync(object transaksi);
        Task AddTransaksiDetailAsync(object detail);
        void UpdateProductUnit(object productUnit);
        Task SaveChangesAsync();
        Task<object> GetTransaksiForPrintAsync(int id);
        Task<object> GetPengaturanTokoAsync();
    }
}
