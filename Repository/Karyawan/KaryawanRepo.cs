using POS_System.DbCtx;
using POS_System.Models;
namespace POS_System.Repository
{
    public class KaryawanRepo : IKaryawanRepo
    {
        private readonly AppDbContext _context;
        public KaryawanRepo(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Karyawan> GetAllQueryable()
        {
            return _context.Karyawans;
        }
        public async Task<Karyawan?> GetByIdAsync(int id)
        {
            return await _context.Karyawans.FindAsync(id);
        }
        public async Task AddAsync(Karyawan karyawan)
        {
            await _context.Karyawans.AddAsync(karyawan);
        }
        public void Update(Karyawan karyawan)
        {
            _context.Karyawans.Update(karyawan);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}