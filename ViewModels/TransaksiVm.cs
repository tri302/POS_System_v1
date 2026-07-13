using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using POS_System.Models;
namespace POS_System.ViewModels
{
    public class TransaksiVm
    {
        public long IdTransaksi { get; set; }
        public string? NamaMetodeBayar { get; set; }
        public DateTime TanggalTransaksi { get; set; }
        public DateTime CreatedAt { get; set; }
        [DisplayName("No Transaksi")]
        public string? NoTransaksi { get; set; }
        [DisplayName("Nama Pelanggan")]
        public string? NamaPemesan { get; set; }
        [Required(ErrorMessage = "The Nama Customer field is required.")]
        public int IdPelanggan { get; set; }
        public int IdKaryawan { get; set; }
        [DisplayName("Total Belanja")]
        public decimal TotalBelanja { get; set; }
        [DisplayName("Total Bayar")]
        public decimal Bayar { get; set; }
        [DisplayName("Email")]
        //[Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        //[Required(ErrorMessage = "Phone is required")]
        [DisplayName("Phone")]
        public string? Phone { get; set; }
        [DisplayName("Nama Produk")]
        public string? NamaProduk { get; set; }
        [Required(ErrorMessage = "Qty is required")]
        [DisplayName("Qty")]
        public int Qty { get; set; } = 0;
        [Required(ErrorMessage = "Harga Modal is required")]
        [DisplayName("Harga Modal")]
        public int HargaModal { get; set; }
        [Required(ErrorMessage = "Harga Jual is required")]
        [DisplayName("Harga Jual")]
        public decimal HargaJual { get; set; }
        [Required(ErrorMessage = "Profit is required")]
        [DisplayName("Profit")]
        public int Profit { get; set; }
        [DisplayName("Notes")]
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Metode Pembayaran is required")]
        [DisplayName("Metode Pembayaran")]
        public int IdMetodeBayar { get; set; }
        [Required(ErrorMessage = "Nominal Bayar is required")]
        [DisplayName("Nominal Bayar")]
        public decimal NominalBayar { get; set; }
        [DisplayName("Kembalian")]
        public decimal Kembalian { get; set; }
        public string Status { get; set; } = string.Empty;
        public long IdProduk { get; set; }
        public string CariProduk { get; set; } = string.Empty;
        public string ScanBarcode { get; set; } = string.Empty;
        public string? FotoPath { get; set; }
        public int Stok { get; set; }
        // List produk yang dipesan
        public List<TransaksiItemVm>? Items { get; set; } = new();
        public List<TransaksiDetailVm> TransaksiDetails { get; set; } = new();
        public MetodeBayar? MetodeBayar { get; set; }
        [NotMapped]
        [ValidateNever]
        public int Conversion { get; set; }
    }   
}
