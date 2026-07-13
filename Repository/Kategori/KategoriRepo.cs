using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
namespace POS_System.Repository
{
    public class KategoriRepo : IKategoriRepo
    {
        private readonly AppDbContext _context;
        public KategoriRepo(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Kategori> GetAllQuery()
        {
            return _context.Kategoris;
        }
        public async Task<Kategori?> GetByIdAsync(long id)
        {
            return await _context.Kategoris.FindAsync(id);
        }
        public async Task<bool> IsNameExistsAsync(string name, long? excludeId = null)
        {
            var query = _context.Kategoris.AsNoTracking();
            if (excludeId.HasValue)
            {
                return await query.AnyAsync(k => k.NamaKategori.ToLower() == name.ToLower() && k.IdKategori != excludeId.Value);
            }
            return await query.AnyAsync(k => k.NamaKategori.ToLower() == name.ToLower());
        }
        public async Task AddAsync(Kategori kategori)
        {
            await _context.Kategoris.AddAsync(kategori);
        }
        public async Task UpdateAsync(Kategori kategori)
        {
            // Entity Framework akan melacak perubahan otomatis, 
            // fungsi ini disediakan jika diperlukan attach manual/state modification.
            _context.Kategoris.Update(kategori);
            await Task.CompletedTask;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}