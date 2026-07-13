using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using POS_System.DbCtx;
using POS_System.Services;
using POS_System.ViewModels;
public class DashboardService(AppDbContext _db, IMemoryCache _cache) : IDashboardService
{
    // Hitung tanggal berdasarkan rentang waktu
    public (DateTime Start, DateTime End) HitungTanggalPublic(RentangWaktu rentang, DateTime? from, DateTime? to)
    {
        return HitungTanggal(rentang, from, to);
    }
    private (DateTime Start, DateTime End) HitungTanggal(RentangWaktu rentang, DateTime? from, DateTime? to)
    {
        DateTime start, end;
        switch (rentang)
        {
            case RentangWaktu.HariIni:
                start = DateTime.Today;
                end = DateTime.Today.AddDays(1).AddSeconds(-1);
                break;
            case RentangWaktu.MingguIni:
                start = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                end = end = start.AddDays(7).AddSeconds(-1);
                break;
            case RentangWaktu.BulanIni:
                start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                end = start.AddMonths(1).AddSeconds(-1);
                break;
            case RentangWaktu.TahunIni:
                start = new DateTime(DateTime.Today.Year, 1, 1);
                end = new DateTime(DateTime.Today.Year, 12, 31).AddDays(1).AddSeconds(-1);
                break;
            case RentangWaktu.Manual:
                start = from ?? DateTime.Today;
                end = to ?? DateTime.Today;
                break;
            default:
                start = DateTime.Today;
                end = DateTime.Today;
                break;
        }
        return (start, end);
    }
    public async Task<DashboardIndexVm> GetDashboardAsync(RentangWaktu rentang, DateTime? from, DateTime? to)
    {
        var (start, end) = HitungTanggal(rentang, from, to);
        var cacheKey = $"DASHBOARD_{start:yyyyMMdd}_{end:yyyyMMdd}";
        if (_cache.TryGetValue(cacheKey, out DashboardIndexVm cachedVm))
        {
            return cachedVm;
        }
        // Get penjualan per day
        var penjualan = await _db.Transaksis
            .Include(t => t.TransaksiDetails)            
            .Where(t => t.TanggalTransaksi >= start && t.TanggalTransaksi <= end)
            .ToListAsync();
        decimal Penjualan = penjualan
            .Sum(t => t.TransaksiDetails.Sum(d => d.HargaJual * d.Qty));
        // Get uang masuk per day        
        decimal uangMasuk = await _db.AlurKass
               .Where(i => i.CashFlow.IdTipeCashFlow == 1 &&
               i.Date >= start && i.Date <= end)
               .SumAsync(a => (decimal?)a.Amount) ?? 0;
        // Get uang keluar per day        
        decimal uangKeluar = await _db.AlurKass
               .Where(i => i.CashFlow.IdTipeCashFlow == 2 &&
               i.Date >= start && i.Date <= end)
               .SumAsync(a => (decimal?)a.Amount) ?? 0;
        // Get total uang masuk keseluruhan
        // Dari CashFlow dengan IdTipeCashFlow = 1
        decimal TotaluangMasuk1 = await _db.AlurKass
               .Where(i => i.CashFlow.IdTipeCashFlow == 1 &&
               i.Date >= start && i.Date <= end)
               .SumAsync(a => (decimal?)a.Amount) ?? 0;
        // Get total uang masuk keseluruhan
        // Dari Penjualan produk
        decimal TotaluangMasuk2 = await _db.Transaksis
            .Include(t => t.TransaksiDetails)
            .Where(t => t.TanggalTransaksi >= start && t.TanggalTransaksi <= end)
            .SumAsync(t => (decimal?)t.TransaksiDetails.Sum(d => d.HargaJual * d.Qty)) ?? 0;            
        decimal HasilTotaluangMasuk = TotaluangMasuk1 + TotaluangMasuk2;
        // Get total uang keluar keseluruhan
        decimal TotaluangKeluar = await _db.AlurKass
               .Where(i => i.CashFlow.IdTipeCashFlow == 2 &&
               i.Date >= start && i.Date <= end)
               .SumAsync(a => (decimal?)a.Amount) ?? 0;
        // Get produk terlaris
        var produkTerlaris = await _db.TransaksiDetails
                .Include(td => td.ProductUnit)
                .Include(td => td.Produk)
                    .ThenInclude(p => p.ProdukGambars)
                .Where(td =>
                    td.Transaksi.TanggalTransaksi >= start &&
                    td.Transaksi.TanggalTransaksi <= end)
                .GroupBy(td => new { td.IdProduk, td.Produk.NamaProduk, td.ProductUnit.UnitName })
                .Select(g => new ProdukTerlarisVm
                {
                    IdProduk = g.Key.IdProduk,
                    NamaProduk = g.Key.NamaProduk,
                    UnitName = g.Key.UnitName ?? "Pcs",
                    TotalTerjual = g.Sum(x => x.Qty),
                    GambarUrl = g.First().Produk.ProdukGambars
                        .Select(pg => pg.PathGambar)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.TotalTerjual)
                .Take(10)
                .ToListAsync();
        // Get status stok produk
        var statusStok = await _db.Produks
                .Include(p => p.ProdukGambars)
                .Include(pu => pu.ProductUnits)
                .Select(pu => new StatusStokProdukVm
                {
                    IdProduk = pu.IdProduk,
                    NamaProduk = pu.NamaProduk,
                    UnitName = pu.UnitName ?? "Pcs",
                    Stok = pu.Stok,
                    Status =
                        pu.Stok <= 0 ? "Habis" :
                        pu.Stok <= 5 ? "Hampir Habis" :
                        "Aman",
                    GambarUrl = pu.ProdukGambars
                        .Select(pg => pg.PathGambar)
                        .FirstOrDefault()
                })                
                .OrderBy(p => p.Stok)
                .Take(10)
                .ToListAsync();
        var vm = new DashboardIndexVm
        {
            RentangWaktu = rentang,
            FromDate = start,
            ToDate = end,
            Penjualan = Penjualan,
            UangMasuk = uangMasuk,
            UangKeluar = uangKeluar,            
            TotalUangMasuk = HasilTotaluangMasuk,
            TotalUangKeluar = TotaluangKeluar,
            TotalUangToko = HasilTotaluangMasuk - uangKeluar,
            ProdukTerlaris = produkTerlaris,
            StatusStokProduk = statusStok,           
        };
        // Simpan ke cache selama 5 menit sudah termasuk sliding expiration 2 menit
        // sehingga jika diakses dalam 2 menit, masa cache diperpanjang
        // dari waktu akses terakhir mempercepatkan load data berikutnya
        _cache.Set(cacheKey, vm, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        });
        return vm;
    }
    // Alur Kas
    public async Task<List<ChartItemVM>> GetAlurKasChartAsync(DateTime start, DateTime end)
    {
        var cacheKey = $"ARUS_KAS_{start:yyyyMMdd}_{end:yyyyMMdd}";
        if (_cache.TryGetValue(cacheKey, out List<ChartItemVM> cached))
            return cached;
        var data = await _db.AlurKass
                            .Include(a => a.CashFlow)
                                .ThenInclude(cf => cf.TipeCashFlow)
                            .Where(a => a.Date >= start && a.Date <= end)
                            .GroupBy(a => a.CashFlow.TipeCashFlow.TipeCashFlowName)
                            .Select(g => new ChartItemVM
                            {
                                Label = g.Key,          // Masuk / Keluar
                                Value = g.Sum(x => x.Amount)
                            })
                            .ToListAsync();
        _cache.Set(cacheKey, data, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        });
        return data;
    }
    // Transaksi per Metode Pembayaran
    public async Task<List<ChartItemVM>> GetTransaksiPerMetodeBayarChartAsync(DateTime start, DateTime end)
    {
        var cacheKey = $"TRX_PER_METODE_{start:yyyyMMdd}_{end:yyyyMMdd}";
        if (_cache.TryGetValue(cacheKey, out List<ChartItemVM> cached))
            return cached;
        var data = await _db.Transaksis
                            .Include(t => t.MetodeBayar)
                            .Where(t =>
                                t.TanggalTransaksi >= start &&
                                t.TanggalTransaksi <= end)
                            .GroupBy(t => t.MetodeBayar.NamaMetodeBayar)
                            .Select(g => new ChartItemVM
                            {
                                Label = g.Key,
                                Value = g.Count()
                            })
                            .ToListAsync();
        _cache.Set(cacheKey, data, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        });
        return data;
    }    
    // Fungsi untuk menghapus cache dashboard
    public void ClearDashboardCache(DateTime start, DateTime end)
    {
        var cacheKeyArusKas = $"ARUS_KAS_{start:yyyyMMdd}_{end:yyyyMMdd}";
        var cacheKeyTrx = $"TRX_PER_METODE_{start:yyyyMMdd}_{end:yyyyMMdd}";
        _cache.Remove(cacheKeyArusKas);
        _cache.Remove(cacheKeyTrx);
    }
    // Pencarian Produk Terlaris di seluruh database berdasarkan rentang waktu
    public async Task<List<ProdukTerlarisVm>> SearchProdukTerlarisAsync(string search, DateTime start, DateTime end)
    {
        var query = _db.TransaksiDetails
            .Include(td => td.Produk)
            .Include(td => td.ProductUnit)
            .Where(td => td.Transaksi.TanggalTransaksi >= start && td.Transaksi.TanggalTransaksi <= end);
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(td => td.Produk.NamaProduk.Contains(search));
        }
        // Ambil data ke memory dulu
        var hasil = await query
            .GroupBy(td => new { td.IdProduk, td.Produk.NamaProduk, td.ProductUnit.UnitName })
            .Select(g => new ProdukTerlarisVm
            {
                IdProduk = g.Key.IdProduk,
                NamaProduk = g.Key.NamaProduk,
                UnitName = g.Key.UnitName ?? "Pcs",
                TotalTerjual = g.Sum(x => x.Qty),
                GambarUrl = g.First().Produk.ProdukGambars.Select(pg => pg.PathGambar).FirstOrDefault()
            })
            .OrderByDescending(x => x.TotalTerjual)
            .ToListAsync();
        //// --- LOOPING DEBUGGING ---
        //Console.WriteLine("=== DEBUG PRODUK TERLARIS ===");
        //foreach (var item in hasil) // Loop dari variabel 'hasil'
        //{
        //    Console.WriteLine($"Produk: {item.NamaProduk} | Unit: '{item.UnitName}' | Qty: {item.TotalTerjual}");
        //}
        //Console.WriteLine("=============================");
        return hasil; // Return di paling bawah
    }
    // Pencarian Status Stok di seluruh database
    public async Task<List<StatusStokProdukVm>> SearchStatusStokAsync(string search)
    {
        var query = _db.Produks
            .Include(pg => pg.ProductUnits)
            .AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(pu => pu.NamaProduk.Contains(search));
        }
        var hasil = await query
            .Select(pu => new StatusStokProdukVm
            {
                IdProduk = pu.IdProduk,
                NamaProduk = pu.NamaProduk,
                UnitName = pu.UnitName ?? "Pcs",
                Stok = pu.Stok,
                Status = pu.Stok <= 0 ? "Habis" : pu.Stok <= 5 ? "Hampir Habis" : "Aman",
                GambarUrl = pu.ProdukGambars.Select(pg => pg.PathGambar).FirstOrDefault()
            })
            .OrderBy(p => p.Stok)
            .ToListAsync();        
        return hasil; 
    }
}
