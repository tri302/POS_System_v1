using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
using POS_System.Repository;
public class PengaturanRepo : IPengaturanRepo
{
    private readonly AppDbContext _context;
    public PengaturanRepo(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<PengaturanToko>> GetAllAsync()
    {
        return await _context.PengaturanTokos.ToListAsync();
    }
    public async Task<PengaturanToko?> GetByIdAsync(int id)
    {
        return await _context.PengaturanTokos.FindAsync(id);
    }
    public async Task AddAsync(PengaturanToko pengaturanToko)
    {
        await _context.PengaturanTokos.AddAsync(pengaturanToko);
    }
    public void Update(PengaturanToko pengaturanToko)
    {
        _context.PengaturanTokos.Update(pengaturanToko);
    }
    public void Delete(PengaturanToko pengaturanToko)
    {
        _context.PengaturanTokos.Remove(pengaturanToko);
    }
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}