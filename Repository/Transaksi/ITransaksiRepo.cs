using POS_System.Models;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public interface ITransaksiRepo
    {
        Task<List<Produk>> GetProduksForUpdateByIdsAsync(List<long> ids);
        Task UpdateProduk(Produk produk);
        Task<bool> EditTransaksiWithDetailsAsync(TransaksiVm model);
        Task<Transaksi?> GetByIdAsync(long id);
        Task<Transaksi?> GetWithDetailsAsync(long id);
        Task<List<Pelanggan>> GetPelangganListAsync();
        Task<List<MetodeBayar>> GetMetodeBayarListAsync();
        Task<ProductUnit?> GetProductUnitByIdAsync(long productUnitId);
        Task<List<TransaksiDetail>> GetDetailsByTransaksiIdAsync(long idTransaksi);
        Task<(List<Transaksi> Data, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize);               
        Task<Produk> GetProdukByIdAsync(long id);
        Task<List<Produk>> GetAllProdukAsync(List<long> ids);
        Task<List<Pelanggan>> SearchPelangganAsync(string term);
        Task<object?> GetPelangganDetailAsync(int id);
        Task<dynamic?> GetProdukDetailAsync(long id);  
        Task<Karyawan?> GetKaryawanByUserIdAsync(string userId);
        Task<PengaturanToko?> GetPengaturanTokoAsync();
        // State Management / DB Operations
        Task UpdateProductUnitAsync(ProductUnit productUnit);
        void UpdateTransaksi(Transaksi transaksi);
        Task AddTransaksiAsync(Transaksi transaksi);
        Task AddTransaksiDetailAsync(TransaksiDetail detail);
        void RemoveTransaksiDetails(IEnumerable<TransaksiDetail> details);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<Produk> GetProductByBarcodeAsync(string barcode);
        Task<IEnumerable<Produk>> SearchProdukAsync(string term);
    }
}