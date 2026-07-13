using POS_System.Models;
namespace POS_System.Repository
{
    public interface IPengaturanRepo
    {
        Task<IEnumerable<PengaturanToko>> GetAllAsync();
        Task<PengaturanToko?> GetByIdAsync(int id);
        Task AddAsync(PengaturanToko pengaturanToko);
        void Update(PengaturanToko pengaturanToko);
        void Delete(PengaturanToko pengaturanToko);
        Task<bool> SaveChangesAsync();
    }
}
