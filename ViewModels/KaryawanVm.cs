using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace POS_System.ViewModels
{
    public class KaryawanVm
    {
        public int IdKaryawan { get; set; }
        [DisplayName("Nomor Induk Karyawan")]
        [Required(ErrorMessage = "NIK harus diisi")]
        public string? NIK { get; set; }//Nomor Induk Karyawan, unique identifier Kar-001-25, Kar-002-25 dst
        [Required(ErrorMessage = "Nama Karyawan harus diisi")]
        [DisplayName("Nama Karyawan")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Jenis Kelamin harus diisi")]
        [DisplayName("Tempat Lahir")]
        public string? TempatLahir { get; set; }
        [Required(ErrorMessage = "Tanggal Lahir harus diisi")]
        [DisplayName("Tanggal Lahir")]
        public DateTime TanggalLahir { get; set; }
        [Required(ErrorMessage = "Alamat Lengkap harus diisi")]
        [DisplayName("Alamat Lengkap/Sesuai KTP")]
        public string? AlamatLengkap { get; set; }
        [Required(ErrorMessage = "Email harus diisi")]
        [DisplayName("Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required(ErrorMessage = "No Hp/WA harus diisi")]
        [DisplayName("No Hp/WA")]
        public string? NoHp { get; set; }
        [Required(ErrorMessage = "Status Pernikahan harus diisi")]
        [DisplayName("Status Pernikahan")]
        public string? StatusNikah { get; set; }//single, married, divorced
        [Required(ErrorMessage = "Status Kerja harus diisi")]
        [DisplayName("Status Kerja Karyawan")]
        public string? StatusKerja { get; set; }//aktif, non-aktif
        [Required(ErrorMessage = "Tanggal Awal Masuk Bekerja harus diisi")]
        [DisplayName("Tanggal Awal Masuk Bekerja")]
        public DateTime DateJoin { get; set; }
        [DisplayName("Catatan")]
        public string? Catatan { get; set; }
        [DisplayName("Upload Foto Karyawan")]
        public IFormFile? UploadFoto { get; set; }// hanya untuk upload
        public string? FotoPath { get; set; } // untuk ditampilkan (URL ke gambar)        
        [Required(ErrorMessage = "Status Karyawan harus diisi")]
        [DisplayName("Status Karyawan")]
        public string? StatusKaryawan { get; set; }
        // Pilih Role (Owner, Kasir, dll.)
        [Required]
        public string Role { get; set; }= string.Empty;
        [Required(ErrorMessage = "Password harus diisi")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        // 🌟 PROPERTI PAGINASI 🌟
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        // Properti Navigasi Pembantu (Opsional, tapi direkomendasikan)
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
