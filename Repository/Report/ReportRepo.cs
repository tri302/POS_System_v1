using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using static POS_System.ViewModels.KasirReportViewModel;
namespace POS_System.Repository
{
    public class ReportRepo : IReportRepo
    {
        private readonly AppDbContext _context;
        public ReportRepo(AppDbContext context)
        {
            _context = context;
        }
        public async Task<KasirReportResult> GetKasirReportAsync(int idKaryawan, DateTime tanggal)
        {
            var result = new KasirReportResult();
            // ===================================================================
            // 1. CARI DATA SHIFT KARYAWAN PADA TANGGAL TERSEBUT
            // ===================================================================
            var listShifts = await _context.Shifts
                .Where(s => s.IdKaryawan == idKaryawan && s.WaktuBuka.Date == tanggal.Date)
                .ToListAsync();
            var listShiftIds = listShifts.Select(s => s.IdShift).ToList();
            // Jika tidak ada shift yang dibuka pada tanggal tersebut, langsung set default kosong
            if (!listShiftIds.Any())
            {
                result.Summary.IdKaryawan = idKaryawan;
                result.Summary.NamaKaryawan = await _context.Karyawans
                    .Where(k => k.IdKaryawan == idKaryawan)
                    .Select(k => k.Name)
                    .FirstOrDefaultAsync() ?? "Karyawan Tidak Ditemukan";
                result.Summary.SaldoAwal = 0;
                result.Details = new List<KasirReportDetail>();
                return result;
            }
            // Hitung akumulasi data closing dari tabel shift            
            var totalSaldoAwal = listShifts.Sum(s => s.SaldoAwal);
            var totalSaldoAkhirSistem = listShifts.Sum(s => s.SaldoAkhirSistem);
            // Jika ada kolom revisi, gunakan Nilai Revisi jika tidak null/0, jika tidak ada gunakan nilai asli kasir
            var totalSaldoAkhirFisik = listShifts.Sum(s => s.SaldoAkhirFisikRevisi > 0 ? s.SaldoAkhirFisikRevisi : s.SaldoAkhirFisik);
            var totalSelisih = listShifts.Sum(s => s.SaldoAkhirFisikRevisi > 0 ? s.SelisihRevisi : s.Selisih);
            // ===================================================================
            // 2. FILTER TRANSAKSI BERDASARKAN ID SHIFT
            // ===================================================================
            var queryDasar = _context.Transaksis
                .Where(t => t.IdKaryawan == idKaryawan
                         && t.IdMetodeBayar == 1 // Tunai
                         && listShiftIds.Contains(t.IdShift ?? 0));
            // 3. Eksekusi Query Ringkasan (Summary) menggunakan LINQ GroupBy
            var summaryData = await queryDasar
                .GroupBy(t => t.IdKaryawan)
                .Select(g => new KasirReportSummary
                {
                    IdKaryawan = g.Key,
                    NamaKaryawan = _context.Karyawans.Where(k => k.IdKaryawan == g.Key).Select(k => k.Name).FirstOrDefault(),
                    TotalUangMasuk = g.Sum(t => t.NominalBayar),
                    TotalKembalian = g.Sum(t => t.Kembalian),
                    TotalPendapatanBersih = g.Sum(t => t.TotalBelanja)
                })
                .FirstOrDefaultAsync();
            if (summaryData != null)
            {
                result.Summary = summaryData;
            }
            else
            {
                // Default jika shift ada tapi belum ada transaksi tunai yang terjadi
                result.Summary.IdKaryawan = idKaryawan;
                result.Summary.NamaKaryawan = await _context.Karyawans
                    .Where(k => k.IdKaryawan == idKaryawan)
                    .Select(k => k.Name)
                    .FirstOrDefaultAsync() ?? "Karyawan Tidak Ditemukan";
            }
            // Masukkan data audit & saldo awal yang diambil dari objek listShifts ke Summary laporan
            result.Summary.IdShift = listShifts.FirstOrDefault()?.IdShift ?? 0;
            result.Summary.SaldoAwal = totalSaldoAwal;
            result.Summary.SaldoAkhirSistem = totalSaldoAkhirSistem;
            result.Summary.SaldoAkhirFisik = totalSaldoAkhirFisik;
            result.Summary.Selisih = totalSelisih;
            // 4. Eksekusi Query Rincian Transaksi (Details)
            result.Details = await queryDasar
                .OrderByDescending(t => t.TanggalTransaksi)
                .Select(t => new KasirReportDetail
                {
                    NoTransaksi = t.NoTransaksi,
                    TanggalTransaksi = t.TanggalTransaksi,
                    TotalBelanja = t.TotalBelanja,
                    NominalBayar = t.NominalBayar,
                    Kembalian = t.Kembalian,
                    Status = t.Status
                })
                .ToListAsync();
            return result;
        }
    }
}