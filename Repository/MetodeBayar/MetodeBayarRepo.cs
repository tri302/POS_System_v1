using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
namespace POS_System.Repository
{
    public class MetodeBayarRepo : IMetodeBayarRepo
    {
        private readonly AppDbContext _context;
        public MetodeBayarRepo(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<MetodeBayar> GetAllQuery()
        {
            return _context.MetodeBayars;
        }
        public async Task<MetodeBayar?> GetByIdAsync(int id)
        {
            return await _context.MetodeBayars.FindAsync(id);
        }
        public async Task<MetodeBayar?> GetByIdAsNoTrackingAsync(int id)
        {
            return await _context.MetodeBayars.AsNoTracking()
                .FirstOrDefaultAsync(m => m.IdMetodeBayar == id);
        }
        public async Task AddAsync(MetodeBayar metodeBayar)
        {
            await _context.MetodeBayars.AddAsync(metodeBayar);
        }
        public void Update(MetodeBayar metodeBayar)
        {
            _context.MetodeBayars.Update(metodeBayar);
        }
        public void Delete(MetodeBayar metodeBayar)
        {
            _context.MetodeBayars.Remove(metodeBayar);
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MetodeBayars.AnyAsync(e => e.IdMetodeBayar == id);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) >= 0;
        }
    }
}