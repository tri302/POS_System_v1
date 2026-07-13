using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace POS_System.Models
{
    public class TransaksiDetail
    {
        [Key]
        public long Id { get; set; }
        // Qty produk yang dibeli
        public int Qty { get; set; }        
        // Harga jual produk saat transaksi
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaJual { get; set; }         
        [NotMapped]
        public decimal HargaModal { get; set; }    // salin dari Produk saat transaksi
        [NotMapped]        
        public string? NamaProduk { get; set; }
        [NotMapped]
        public string? Gambar { get; set; }        
        // Foreign key ke transaksi induk
        public long IdTransaksi { get; set; }
        [ForeignKey(nameof(IdTransaksi))]
        public virtual Transaksi? Transaksi { get; set; }
        // Foreign key ke produk
        public long IdProduk { get; set; }
        [ForeignKey(nameof(IdProduk))]
        public virtual Produk? Produk { get; set; }
        public long ProductUnitId { get; set; }
        // Foreign key ke ProductUnit
        [ForeignKey("ProductUnitId")]
        public ProductUnit? ProductUnit { get; set; }
        // Profit produk saat transaksi
        [Column(TypeName = "decimal(18,2)")]
        public decimal Profit { get; set; }
        [ValidateNever]
        public int Conversion { get; set; }
    }
}