using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
using POS_System.Pageing;
using POS_System.ViewModels;
namespace POS_System.Repository
{
    public class AlurKasRepo : IAlurKasRepo
    {
        private readonly AppDbContext _context;
        public AlurKasRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<AlurKas>> GetPaginatedAlurKasAsync(int pageNumber, int pageSize)
        {
            var query = _context.AlurKass
                .Include(a => a.CashFlow)
                .ThenInclude(cf => cf.TipeCashFlow)
                .OrderByDescending(a => a.Id)
                .AsNoTracking();
            return await PaginatedList<AlurKas>.CreateAsync(query, pageNumber, pageSize);
        }
        public async Task<List<AlurKas>> GetAlurKasRawListAsync()
        {
            return await _context.AlurKass
                .Include(a => a.CashFlow)
                .ThenInclude(cf => cf.TipeCashFlow)
                .ToListAsync();
        }
        public async Task<List<object>> GetSourcesByTipeAsync(int idTipeCashFlow)
        {
            return await _context.CashFlows
                .Where(s => s.IdTipeCashFlow == idTipeCashFlow)
                .Select(s => new { s.IdCashFlow, s.CashFlowName })
                .Cast<object>()
                .ToListAsync();
        }
        public async Task<AlurKas> GetByIdWithIncludesAsync(long id)
        {
            return await _context.AlurKass
                .Include(t => t.CashFlow)
                .ThenInclude(s => s.TipeCashFlow)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<AlurKas> FindAsync(long id)
        {
            return await _context.AlurKass.FindAsync(id);
        }
        public async Task AddAsync(AlurKas alurKas)
        {
            await _context.AlurKass.AddAsync(alurKas);
        }
        public void Update(AlurKas alurKas)
        {
            _context.AlurKass.Update(alurKas);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<AlurKasIndexVm> GetSummaryViewModelAsync()
        {
            var totalModal = await _context.AlurKass
                .Where(a => a.CashFlow.IdTipeCashFlow == 1 &&
                            (a.CashFlow.CashFlowName == "Modal Awal" || a.CashFlow.CashFlowName == "Modal Tambahan"))
                .SumAsync(a => (decimal?)a.Amount) ?? 0;
            var totalMasuk = await _context.AlurKass
                .Where(a => a.CashFlow.IdTipeCashFlow == 1)
                .SumAsync(a => (decimal?)a.Amount) ?? 0;
            var totalKeluar = await _context.AlurKass
                .Where(a => a.CashFlow.IdTipeCashFlow == 2)
                .SumAsync(a => (decimal?)a.Amount) ?? 0;
            decimal totalUangToko = totalMasuk - totalKeluar;
            var culture = new System.Globalization.CultureInfo("id-ID");
            return new AlurKasIndexVm
            {
                Summary = (totalModal == 0 && totalMasuk == 0 && totalKeluar == 0)
                    ? new AlurKasVm()
                    : new AlurKasVm
                    {
                        TotalModal = totalModal.ToString("C0", culture),
                        TotalUangMasuk = totalMasuk.ToString("C0", culture),
                        TotalUangKeluar = totalKeluar.ToString("C0", culture),
                        TotalUangToko = totalUangToko.ToString("C0", culture)
                    }
            };
        }
    }
}