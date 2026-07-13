using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class ProductUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProductUnitId { get; set; }
        //base_unit atau UnitName
        public string? UnitName { get; set; }// pcs, dus, pack
        //public int? IsiUnit { get; set; }      // optional
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaJual { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaModal { get; set; }
        //public int Stok { get; set; }        
        public bool IsDeleted { get; set; } = false;
        // Foreign Key relation to Produk
        public long IdProduk { get; set; }
        [ForeignKey(nameof(IdProduk))]
        public Produk? Produk { get; set; }
        public ICollection<Inventori> Inventoris { get; set; } = new List<Inventori>();
        public int ConversionValue { get; set; }
        public bool Is_Default { get; set; }        
        [NotMapped]
        [ValidateNever]
        public int Stok { get; set; }
    }
}
