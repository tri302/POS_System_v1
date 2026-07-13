using static POS_System.ViewModels.KasirReportViewModel;
namespace POS_System.Repository
{
    public interface IReportRepo
    {
        // Fungsi untuk mengambil summary dan detail transaksi per kasir & tanggal
        Task<KasirReportResult> GetKasirReportAsync(int idKaryawan, DateTime tanggal);
    }
}
