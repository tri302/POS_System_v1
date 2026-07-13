using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class Pelanggan
    {
        public int Id { get; set; }
        public string? Nama { get; set; }
        public string? Alamat { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool IsPinned { get; set; } = false; // Default false
        public int JenisPelangganId { get; set; }
        [ForeignKey("JenisPelangganId")]
        public JenisPelanggan? JenisPelanggan { get; set; }
    }
}
