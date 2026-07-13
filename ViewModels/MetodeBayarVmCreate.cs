using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace POS_System.ViewModels
{
    public class MetodeBayarVmCreate
    {
        public int IdMetodeBayar { get; set; }
        [Required(ErrorMessage = "Metode Pembayaran tidak boleh kosong !!!")]
        [DisplayName("Metode Pembayaran")]
        public string? NamaMetodeBayar { get; set; }
        // Properti ini digunakan untuk menerima file saat POST.
        // IFormFile harus ada untuk validasi required upload.
        [Required(ErrorMessage = "Upload Gambar tidak boleh kosong !!!")]
        [DisplayName("Upload Gambar")]
        public IFormFile? UploadGambar { get; set; } // Pastikan properti ini ada dan bertipe IFormFile
        // Properti ini hanya untuk menyimpan URL/path gambar setelah di-upload (tidak perlu Required)
        public string? UrlGambar { get; set; }
    }
}