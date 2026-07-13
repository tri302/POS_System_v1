using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class LaporanKeuangan
    {
        [Key] 
        public string? KodeLaporanKeuangan { get; set; }    
        public string? TipeLaporan { get; set; } //e.g. Uang masuk, Uang keluar, Penjualan
        [DisplayName("From Date")]
        public DateTime? TanggalMulai { get; set; }
        [DisplayName("To Date")]
        public DateTime? TanggalSelesai { get; set; }
        [NotMapped]
        public decimal? Total { get; set; }
        [NotMapped]
        public string? Catatan { get; set; }
    }
}
