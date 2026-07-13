using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class ProdukGambar
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long IdProdukGambar { get; set; }
        [DisplayName("Upload Gambar")]
        [NotMapped]// Supaya EF tidak mencoba mapping IFormFile ke DB
        public IFormFile? UrlGambar { get; set; }
        public string? PathGambar { get; set; } // simpan path ke DB
        public long IdProduk { get; set; }
        [ForeignKey("IdProduk")]
        public Produk? Produk { get; set; }
    }
}
