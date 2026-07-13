using Microsoft.AspNetCore.Identity;
namespace POS_System.Models
{
    //diturunkankan/extend dari IdentityUser
    public class ApplicationUser: IdentityUser
    {
        // Tambahkan properti kustom Anda di sini, jika perlu
        // public string NamaLengkap { get; set; }
        // public string Alamat { get; set; }
        public string Theme { get; set; } = "default"; // light, dark, blue, purple, dll
    }
}
