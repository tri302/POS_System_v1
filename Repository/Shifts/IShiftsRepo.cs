using POS_System.Models;
namespace POS_System.Repository
{
    public interface IShiftsRepo
    {
        // Mengecek apakah kasir tertentu saat ini punya shift yang masih aktif/terbuka
        Task<Shift?> GetActiveShiftAsync(int idKaryawan);
        // Membuka shift baru (input modal awal)
        Task<bool> OpenShiftAsync(Shift shift);
        // Menutup shift (hitung otomatis sistem dan hitung selisih fisik)
        Task<bool> CloseShiftAsync(int idShift, decimal saldoAkhirFisik);
    }
}