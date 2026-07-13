using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using POS_System.Models;
namespace POS_System.ViewModels
{
    public class ProdukGambarVm
    {
        public long IdProdukGambar { get; set; }
        [DisplayName("Upload Gambar")]
        public IFormFile? UploadGambar { get; set; }
        public string? FotoPath { get; set; }
        [ValidateNever]
        [NotMapped]
        public long IdProduk { get; set; }
        [ValidateNever]
        [NotMapped]
        public Produk? ProdukVm { get; set; }
    }
}
