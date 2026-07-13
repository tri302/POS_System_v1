using POS_System.Models;
namespace POS_System.Repository
{
    public interface IKategoriRepo
    {
        IQueryable<Kategori> GetAllQuery();
        Task<Kategori?> GetByIdAsync(long id);
        Task<bool> IsNameExistsAsync(string name, long? excludeId = null);
        Task AddAsync(Kategori kategori);
        Task UpdateAsync(Kategori kategori);
        Task SaveChangesAsync();
    }
}
