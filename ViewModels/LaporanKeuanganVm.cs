using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace POS_System.ViewModels
{
    public class LaporanKeuanganVm
    {
        public string? KodeLaporanKeuangan { get; set; }
        [DisplayName("Report Type")]
        public string? TipeLaporan { get; set; } //e.g. Uang masuk, Uang keluar, Penjualan
        [DisplayName("From Date")]
        [Required(ErrorMessage = "Tanggal Mulai wajib diisi.")]
        public DateTime? TanggalMulai { get; set; }
        [DisplayName("To Date")]
        [Required(ErrorMessage = "Tanggal Selesai wajib diisi.")]
        public DateTime? TanggalSelesai { get; set; }
        public decimal? Total {  get; set; }
        public string? Catatan { get; set; }
    }
}
