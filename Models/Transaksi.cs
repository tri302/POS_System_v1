using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class Transaksi
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long IdTransaksi { get; set; }
        [NotMapped]
        public string? NamaMetodeBayar { get; set; }
        [NotMapped]
        public string? NamaPemesan { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBelanja { get; set; }        
        [NotMapped]
        public string? Email { get; set; }
        [NotMapped]
        public string? Phone { get; set; }        
        public string? Notes { get; set; }        
        [Column(TypeName = "decimal(18,2)")]
        public decimal NominalBayar { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Kembalian { get; set; }
        [NotMapped]
        public string? NIK { get; set; }
        public string? NoTransaksi { get; set; }
        public string Status { get; set; } = string.Empty;        
        public DateTime TanggalTransaksi { get; set; }
        // Properti navigasi ke detail transaksi (list produk yang dibeli)
        public virtual ICollection<TransaksiDetail> TransaksiDetails { get; set; } = new HashSet<TransaksiDetail>();
        //Untuk detail Kustomer/Pelanggan
        // Foreign key ke Pelanggan (bisa null untuk pelanggan umum)
        public int IdPelanggan { get; set; }
        [ForeignKey("IdPelanggan")]
        public Pelanggan? Pelanggan { get; set; }
        // Foreign key ke Karyawan/Kasir yang menangani transaksi
        public int IdKaryawan { get; set; }
        [ForeignKey("IdKaryawan")]
        public Karyawan? Karyawans { get; set; }
        public int IdMetodeBayar { get; set; }
        // Foreign key ke MetodeBayar
        [ForeignKey("IdMetodeBayar")]
        public MetodeBayar? MetodeBayar { get; set; }
        // Foreign key ke Shift
        public int? IdShift { get; set; } // Dibuat nullable (int?) jaga-jaga jika ada transaksi lama yang kosong atau transaksi non-kasir
        [ForeignKey("IdShift")]
        public Shift? Shift { get; set; }
        [NotMapped]
        [ValidateNever]
        public int Conversion { get; set; }
    }
}
