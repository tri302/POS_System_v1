using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
namespace POS_System.Repository
{
    public class ShiftsRepo : IShiftsRepo
    {
        private readonly AppDbContext _context;
        public ShiftsRepo(AppDbContext context)
        {
            _context = context;
        }
        // 1. Cek Shift Aktif (Shift yang WaktuTutup-nya masih NULL)
        public async Task<Shift?> GetActiveShiftAsync(int idKaryawan)
        {
            return await _context.Shifts
                .Where(s => s.IdKaryawan == idKaryawan && s.WaktuTutup == null)
                .FirstOrDefaultAsync();
        }
        // 2. Buka Shift Baru
        public async Task<bool> OpenShiftAsync(Shift shift)
        {
            // Validasi: pastikan tidak ada shift ganda yang aktif untuk karyawan yang sama
            var activeShift = await GetActiveShiftAsync(shift.IdKaryawan);
            if (activeShift != null) return false; // Gagal karena shift sebelumnya belum ditutup
            shift.WaktuBuka = DateTime.Now;
            shift.WaktuTutup = null;
            shift.SaldoAkhirSistem = 0;
            shift.SaldoAkhirFisik = 0;
            shift.Selisih = 0;
            _context.Shifts.Add(shift);
            return await _context.SaveChangesAsync() > 0;
        }
        // 3. Tutup Shift & Rekonsiliasi Uang Kas
        public async Task<bool> CloseShiftAsync(int idShift, decimal saldoAkhirFisik)
        {
            var shift = await _context.Shifts.FindAsync(idShift);
            if (shift == null || shift.WaktuTutup != null) return false; // Shift tidak ada atau sudah ditutup
            // Ambil semua transaksi tunai yang dilakukan kasir ini semenjak shift dibuka hingga saat ini            
            // Menggunakan IdShift jauh lebih aman daripada tebak-tebakan berdasarkan waktu
            var totalTransaksiTunai = await _context.Transaksis
                .Where(t => t.IdShift == idShift && t.IdMetodeBayar == 1)
                .SumAsync(t => t.TotalBelanja);
            // Hitung ekspektasi saldo laci menurut sistem
            shift.SaldoAkhirSistem = shift.SaldoAwal + totalTransaksiTunai;
            // Set input fisik dari kasir
            shift.SaldoAkhirFisik = saldoAkhirFisik;
            // Hitung selisih (Fisik - Sistem). Jika minus berarti uang laci tekor/kurang.
            shift.Selisih = shift.SaldoAkhirFisik - shift.SaldoAkhirSistem;
            // Set waktu penutupan
            shift.WaktuTutup = DateTime.Now;
            _context.Shifts.Update(shift);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
