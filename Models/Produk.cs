using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace POS_System.Models
{
    public class Produk
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long IdProduk { get; set; }
        public string? NamaProduk { get; set; }
        public string? KodeBarcode { get; set; }           
        public string? SKU { get; set; }
        public string? StatusProduk { get; set; }//Produk Aktif, Produk Tidak Aktif
        public string? Deskripsi { get; set; }
        //base_unit atau UnitName
        [NotMapped]
        public string? UnitName { get; set; }// pcs, dus, pack
        // Foreign Key
        public long IdKategori { get; set; }
        // Navigation Property
        [ForeignKey(nameof(IdKategori))]
        // Navigation Property
        // One to Many
        // Satu produk 1 Kategori
        public Kategori? Kategori { get; set; }       
        // 👇 Tambahkan ini (relasi One-to-Many)
        public ICollection<ProdukGambar> ProdukGambars { get; set; } = new List<ProdukGambar>();
        public ICollection<ProductUnit> ProductUnits { get; set; } = new List<ProductUnit>();
        public ICollection<Inventori> Inventoris { get; set; } = new List<Inventori>();        
        public virtual ICollection<TransaksiDetail> TransaksiDetails { get; set; } = new HashSet<TransaksiDetail>();
        [NotMapped]
        public string? Gambar
        {
            get
            {
                // Jika produk punya koleksi gambar, ambil gambar pertama
                if (ProdukGambars != null && ProdukGambars.Any())
                {
                    return ProdukGambars.First().PathGambar;
                }
                // Jika tidak ada gambar, bisa kembalikan placeholder
                return "/images/no-image.png"; // ganti sesuai path default kamu
            }
        }
        public int Stok { get; set; }
        [NotMapped]
        [ValidateNever]
        public long ProductUnitId { get; set; }
        [NotMapped]
        [ValidateNever]
        public decimal HargaJual { get; set; }
        [NotMapped]
        [ValidateNever]
        public decimal HargaModal { get; set; }
    }
}
