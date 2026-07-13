using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace POS_System.Models
{
    [Table("shifts")] // Menyelaraskan nama tabel dengan database MySQL
    public class Shift
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // AUTO_INCREMENT
        [Column("IdShift")]
        public int IdShift { get; set; }
        [Required]
        [Column("IdKaryawan")]
        public int IdKaryawan { get; set; }
        [Required]
        [Column("WaktuBuka")]
        public DateTime WaktuBuka { get; set; }
        [Column("WaktuTutup")]
        public DateTime? WaktuTutup { get; set; } // Nullable karena diisi saat close shift
        [Required]
        [Column("SaldoAwal", TypeName = "decimal(18,2)")]
        public decimal SaldoAwal { get; set; }
        [Column("SaldoAkhirSistem", TypeName = "decimal(18,2)")]
        public decimal SaldoAkhirSistem { get; set; } = 0;
        [Column("SaldoAkhirFisik", TypeName = "decimal(18,2)")]
        public decimal SaldoAkhirFisik { get; set; } = 0;
        [Column("Selisih", TypeName = "decimal(18,2)")]
        public decimal Selisih { get; set; } = 0;
        // Opsional: Relasi Navigasi ke tabel Karyawan jika diperlukan EF Core
        [ForeignKey("IdKaryawan")]
        public virtual Karyawan? Karyawan { get; set; }
        [Column("SaldoAkhirFisikRevisi", TypeName = "decimal(18,2)")]
        public decimal SaldoAkhirFisikRevisi { get; set; } = 0;
        [Column("SelisihRevisi", TypeName = "decimal(18,2)")]
        public decimal SelisihRevisi { get; set; } = 0;
        public string? KeteranganRevisi { get; set; }
        public DateTime? TanggalRevisi { get; set; }
    }
}
