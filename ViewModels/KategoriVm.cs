using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace POS_System.ViewModels
{
    public class KategoriVm
    {
        public long IdKategori { get; set; }
        [DisplayName("Nama Kategori")]
        [Required(ErrorMessage = "Nama Kategori harus diisi")]
        public string? NamaKategori { get; set; }
        [NotMapped]
        [ValidateNever]
        public int JumlahProduk { get; set; }
        [NotMapped]
        [ValidateNever]
        public long IdProduk { get; set; }
        // ✅ Static List Kategori
        public static List<KategoriVm> StaticList => new List<KategoriVm>
        {
            new KategoriVm { IdKategori = 1, NamaKategori = "Makanan" },
            new KategoriVm { IdKategori = 2, NamaKategori = "Minuman" },
            new KategoriVm { IdKategori = 3, NamaKategori = "Elektronik" }
        };
    }
}
