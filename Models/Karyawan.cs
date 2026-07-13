using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class Karyawan
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdKaryawan { get; set; }
        public string UserId { get; set; } = string.Empty; // foreign key ke AspNetUsers table
        public string? NIK { get; set; }//Nomor Induk Karyawan, unique identifier Kar-001-25, Kar-002-25 dst
        public string? Name { get; set; }
        public string? TempatLahir { get; set; }
        public DateTime TanggalLahir { get; set; }
        public string? AlamatLengkap { get; set; }
        public string? Email { get; set; }
        public string? NoHp { get; set; }
        public string? StatusNikah { get; set; }//single, married, divorced
        public string? StatusKerja { get; set; }//aktif, non-aktif
        public DateTime DateJoin { get; set; }
        public string? Catatan { get; set; }
        public string? FotoPath { get; set; } // untuk ditampilkan (URL ke gambar)    
        public string? StatusKaryawan { get; set; }
    }
}
