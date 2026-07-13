using POS_System.Models;
namespace POS_System.Repository
{
    public interface IMetodeBayarRepo
    {
        IQueryable<MetodeBayar> GetAllQuery();
        Task<MetodeBayar?> GetByIdAsync(int id);
        Task AddAsync(MetodeBayar metodeBayar);
        void Update(MetodeBayar metodeBayar);
        void Delete(MetodeBayar metodeBayar);
        Task<bool> ExistsAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
