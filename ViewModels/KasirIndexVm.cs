using System.ComponentModel.DataAnnotations;
namespace POS_System.ViewModels
{
    public class KasirIndexVm
    {
        // Daftar produk yang akan ditampilkan
        public IEnumerable<TransaksiItemVm>? ProdukItems { get; set; }         
        // Properti untuk Input Search dan Barcode
        public string CariProduk { get; set; } = string.Empty;
        public string ScanBarcode { get; set; } = string.Empty;
        [Required(ErrorMessage = "Pilih Metode Pembayaran !")]
        public int IdMetodeBayar { get; set; }
        [Required(ErrorMessage = "Pilih Jenis Custumer !")]
        public int IdPelanggan { get; set; }
        public decimal NominalBayar { get; set; }
        public decimal Kembalian { get; set; }
        public decimal TotalBelanja { get; set; }
        // 🌟 PROPERTI PAGINASI 🌟
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        // Properti Navigasi Pembantu (Opsional, tapi direkomendasikan)
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
