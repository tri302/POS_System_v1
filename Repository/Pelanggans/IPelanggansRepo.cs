using POS_System.Models;
namespace POS_System.Repository
{
    public interface IPelanggansRepo
    {
        Task<(IEnumerable<Pelanggan> Data, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize);
        Task<Pelanggan> GetByIdAsync(int? id);
        Task<IEnumerable<JenisPelanggan>> GetJenisPelanggansAsync();
        Task AddAsync(Pelanggan pelanggan);
        void Update(Pelanggan pelanggan);
        void Delete(Pelanggan pelanggan);        
        Task<bool> SaveChangesAsync();
        bool Exists(int id);
    }
}
