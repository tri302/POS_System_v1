using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class PengaturanToko
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Display(Name = "Logo Toko")]
        public string? LogoToko { get; set; }   // simpan path relatif seperti /uploads/logo/xxx.png
        [Required]
        [StringLength(150)]
        [Display(Name = "Nama Toko")]
        public string NamaToko { get; set; } = "";
        [Required]
        [StringLength(300)]
        [Display(Name = "Alamat Toko")]
        public string AlamatToko { get; set; } = "";
        [Required]
        [Phone]
        [Display(Name = "Nomor Telepon")]
        public string NomorTelepon { get; set; } = "";
        //[Display(Name = "Print Via Bluetooth")]
        //public bool PrintViaBluetooth { get; set; } = false;
    }
}
