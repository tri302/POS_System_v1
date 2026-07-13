using POS_System.DbCtx;
using POS_System.Models;
using Microsoft.EntityFrameworkCore;
namespace POS_System.Repository
{
    public class PelanggansRepo : IPelanggansRepo
    {
        private readonly AppDbContext _context;
        public PelanggansRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<(IEnumerable<Pelanggan> Data, int TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Pelanggans.Include(p => p.JenisPelanggan);
            int totalCount = await query.CountAsync();
            var data = await query
                .OrderByDescending(p => p.IsPinned)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
        public async Task<Pelanggan> GetByIdAsync(int? id)
        {
            return await _context.Pelanggans
                .Include(p => p.JenisPelanggan)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<IEnumerable<JenisPelanggan>> GetJenisPelanggansAsync()
        {
            return await _context.JenisPelanggans.ToListAsync();
        }
        public async Task AddAsync(Pelanggan pelanggan)
        {
            await _context.Pelanggans.AddAsync(pelanggan);
        }
        public void Update(Pelanggan pelanggan)
        {
            _context.Pelanggans.Update(pelanggan);
        }
        public void Delete(Pelanggan pelanggan)
        {
            _context.Pelanggans.Remove(pelanggan);
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
        public bool Exists(int id)
        {
            return _context.Pelanggans.Any(e => e.Id == id);
        }        
    }
}