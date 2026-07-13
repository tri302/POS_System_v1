using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using POS_System.Models;
namespace POS_System.ViewModels
{
    public class InventoriVm
    {
        public string? KodeBarcode { get; set; }

        [ValidateNever]
        [NotMapped]
        public string? UnitName { get; set; }// pcs, dus, pack
        public long IdInventori { get; set; }
        [DisplayName("No Invoice/No Faktur")]        
        public string? NoReferensi { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipeStok == "1" && string.IsNullOrWhiteSpace(NoReferensi))
            {
                yield return new ValidationResult(
                    "No Invoice/No Faktur wajib diisi untuk Barang Masuk",
                    new[] { nameof(NoReferensi) });
            }
        }
        public int TipeStokId { get; set; }  // untuk bind radio button
        [DisplayName("Tipe Stok")]
        public string? TipeStok { get; set; }//Masuk,Keluar, Penyesuaian
        [Required(ErrorMessage = "Sumber wajib dipilih")]
        public string Sumber { get; set; } = string.Empty;//Penambahan Stok,Barang Rusak, Kadaluarsa, Stok Opname
        [DisplayName("Total Modal")]
        [Required()]
        public decimal TotalModal { get; set; }
        public string? Notes { get; set; }
        [DisplayName("Nama Produk")]        
        public string? NamaProduk { get; set; }
        [DisplayName("Harga Modal")]
        [Required(ErrorMessage = "Harga Modal harus diisi")]
        public decimal HargaModal { get; set; }= 0;
        [DisplayName("Stok Saat Ini")]
        [Required(ErrorMessage = "Stok Saat Ini harus diisi")]
        public int Stok { get; set; }
        [DisplayName("Jumlah")]
        [Required(ErrorMessage = "Jumlah harus diisi")]
        public int Jumlah { get; set; } = 0;
        public long IdProduk { get; set; }
        public long ProductUnitId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaJual { get; set; }
        public List<TipeStok>? ListTipeStok { get; set; } = new();
        // Daftar produk yang dipilih
        public List<InventoriItemVm> Items { get; set; } = new();
        [NotMapped]
        public TipeStok? TipeStokObj { get; set; }
        [NotMapped]
        public SumberStok? SumberStokObj { get; set; }
    }
}
