using System.ComponentModel.DataAnnotations;
using POS_System.Models;
namespace POS_System.ViewModels
{
    public class PelangganVm
    {        
        public int Id { get; set; }
        [Required(ErrorMessage = "Nama is required")]
        public string Nama { get; set; } = string.Empty;
        [Required(ErrorMessage = "Alamat is required")]
        public string Alamat { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone is required")]
        public string Phone { get; set; } = string.Empty;
        public bool IsPinned { get; set; } = false; // Default false
        public int JenisPelangganId { get; set; }
        public JenisPelanggan? JenisPelanggan { get; set; }
        public string? NamaJenis { get; set; } //Umum,Grosir,Reseller,Distributor,VIP
        }
}
