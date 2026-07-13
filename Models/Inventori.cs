using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using POS_System.ViewModels;
namespace POS_System.Models
{
    public class Inventori
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long IdInventori { get; set; }
        public string? NoReferensi { get; set; }
        public string? TipeStok { get; set; }//Masuk,Keluar, Penyesuaian
        public string? Sumber { get; set; }//Penambahan Stok,Barang Rusak, Kadaluarsa, Stok Opname
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalModal { get; set; }
        public string? Notes { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaModal { get; set; }
        public int Jumlah { get; set; }
        [NotMapped]
        public int Stok { get; set; }
        public long IdProduk { get; set; }
        [ForeignKey("IdProduk")]
        public Produk? Produk { get; set; }
        public long ProductUnitId { get; set; }
        [ForeignKey("ProductUnitId")]
        public ProductUnit? ProductUnit { get; set; }
        [NotMapped]
        public string? NamaProduk { get; set; }        
        public DateTime Tanggal { get; set; } = DateTime.Now;
        [NotMapped]
        public TipeStokEnum TipeStokEnum
        {
            get => (TipeStokEnum)int.Parse(TipeStok ?? "1");
            set => TipeStok = ((int)value).ToString();
        }
    }
}
