using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class Kategori
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long IdKategori { get; set; }        
        public string? NamaKategori { get; set; }
        // Navigation Property
        // Satu Category bisa punya banyak Produk
        // Satu Produk hanya milik satu Category
        public ICollection<Produk> Products { get; set; } = new List<Produk>();
    }
}
