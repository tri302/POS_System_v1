using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
namespace POS_System.Repository
{
    public class LaporanKeuanganRepo : ILaporanKeuanganRepo
    {
        private readonly AppDbContext _db;
        public LaporanKeuanganRepo(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Transaksi>> GetLaporanPenjualanAsync(DateTime tglMulai, DateTime tglSelesai)
        {
            return await _db.Transaksis
                .IgnoreQueryFilters()
                .Include(t => t.TransaksiDetails)
                    .ThenInclude(p => p.Produk)
                        .ThenInclude(pu => pu.ProductUnits)
                .OrderBy(t => t.TanggalTransaksi)
                .ToListAsync();
        }
        public async Task<List<AlurKas>> GetLaporanKasAsync(string tipeLaporan, DateTime tglMulai, DateTime tglSelesai)
        {
            return await _db.AlurKass
                .Include(l => l.CashFlow)
                    .ThenInclude(cf => cf.TipeCashFlow)
                .Where(l => l.CashFlow.TipeCashFlow.TipeCashFlowName == tipeLaporan &&
                            l.Date >= tglMulai &&
                            l.Date <= tglSelesai)
                .OrderBy(t => t.Date)
                .ToListAsync();
        }
        public async Task<(List<LaporanKeuangan> Data, int TotalCount)> GetPaginatedLaporanAsync(int pageNumber, int pageSize)
        {
            var query = _db.LaporanKeuangans.OrderByDescending(k => k.KodeLaporanKeuangan);
            int totalCount = await query.CountAsync();
            var data = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (data, totalCount);
        }
        public async Task<LaporanKeuangan> GetByIdAsync(string id)
        {
            return await _db.LaporanKeuangans.FirstOrDefaultAsync(x => x.KodeLaporanKeuangan == id);
        }
        public async Task<string> GenerateKodeLaporanAsync()
        {
            DateTime today = DateTime.Now.Date;
            string datePart = today.ToString("yyyyMMdd");
            var lastReport = await _db.LaporanKeuangans
                .Where(l => l.KodeLaporanKeuangan.StartsWith($"LAPORAN-{datePart}"))
                .OrderByDescending(l => l.KodeLaporanKeuangan)
                .Select(l => l.KodeLaporanKeuangan)
                .FirstOrDefaultAsync();
            int nextNumber = 1;
            if (lastReport != null)
            {
                string numberPart = lastReport.Split('-').Last();
                if (int.TryParse(numberPart, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
            }
            return $"LAPORAN-{datePart}-{nextNumber:D2}";
        }
        public async Task AddAsync(LaporanKeuangan laporan)
        {
            await _db.LaporanKeuangans.AddAsync(laporan);
        }
        public async Task UpdateAsync(LaporanKeuangan laporan)
        {
            // Entity Framework otomatis melacak perubahan, 
            // metode ini opsional tetapi baik untuk dokumentasi arsitektur.
            _db.LaporanKeuangans.Update(laporan);
            await Task.CompletedTask;
        }
        public async Task<bool> SaveChangesAsync()
        {
            return (await _db.SaveChangesAsync()) > 0;
        }                
    }
}