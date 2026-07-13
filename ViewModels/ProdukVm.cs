using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace POS_System.ViewModels
{
    public class ProdukVm
    {
        public long IdProduk { get; set; }
        [DisplayName("Nama Produk")]
        [Required(ErrorMessage = "Nama Produk tidak boleh kosong !")]
        public string? NamaProduk { get; set; }
        [DisplayName("Kode Barcode")]
        [Required(ErrorMessage = "Kode Barcode tidak boleh kosong !")]
        public string? KodeBarcode { get; set; }
        [DisplayName("Stok")]
        [Range(0, int.MaxValue, ErrorMessage = "Stok harus berupa angka dan tidak boleh negatif")]
        [Required(ErrorMessage = "Stok tidak boleh kosong !")]
        public int Stok { get; set; }
        [DisplayName("Harga Modal")]
        [Range(0, double.MaxValue, ErrorMessage = "Harga Modal harus berupa angka dan tidak boleh negatif")]
        [Required(ErrorMessage = "Harga Modal tidak boleh kosong !")]
        public decimal HargaModal { get; set; }
        [DisplayName("Harga Jual")]
        [Range(0, double.MaxValue, ErrorMessage = "Harga Jual harus berupa angka dan tidak boleh negatif")]
        [Required(ErrorMessage = "Harga Jual tidak boleh kosong !")]
        public decimal HargaJual { get; set; }
        [DisplayName("SKU")]
        [MaxLength(20, ErrorMessage = "SKU maksimal 20 karakter")]        
        [Required(ErrorMessage = "SKU tidak boleh kosong !")]
        public string? SKU { get; set; }
        [DisplayName("Status Produk")]
        [Required(ErrorMessage = "Status Produk harus dipilih")]
        public string? StatusProduk { get; set; }
        [DisplayName("Deskripsi")]
        public string? Deskripsi { get; set; }
        [DisplayName("Upload Gambar")]        
        public IFormFile? UploadGambar { get; set; }//Untuk Upload Gambar
        public string? FotoPath { get; set; }//Untuk Menyimpan Path Gambar
        [DisplayName("Kategori")]
        [Required(ErrorMessage = "Kategori harus dipilih")]
        public long IdKategori { get; set; } // ini yang dipakai untuk binding dropdown
        [DisplayName("Nama Kategori")]
        public string? NamaKategori { get; set; } // opsional: kalau mau include data kategori        
        [JsonPropertyName("productunits")] 
        public List<ProductUnitDto> ProductUnits { get; set; } = new();
        [ValidateNever]
        public IEnumerable<SelectListItem> KategoriList { get; set; } = new List<SelectListItem>(); // untuk isi dropdown langsung dari VM        
        [ValidateNever]
        public string? UnitName { get; set; }
    }   
}
