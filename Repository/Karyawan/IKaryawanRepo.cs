using POS_System.Models;
namespace POS_System.Repository
{
    public interface IKaryawanRepo
    {
        IQueryable<Karyawan> GetAllQueryable();
        Task<Karyawan?> GetByIdAsync(int id);
        Task AddAsync(Karyawan karyawan);
        void Update(Karyawan karyawan);
        Task<bool> SaveChangesAsync();
    }
}
