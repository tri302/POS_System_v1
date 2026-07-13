using System.ComponentModel;
namespace POS_System.ViewModels
{
    public class KasirVm
    {
        public long IdPembelian { get; set; }
        [DisplayName("Jumlah Barang")]
        public int Qty { get; set; }
        [DisplayName("Nama Barang")]
        public string? NamaBarang { get; set; }
        [DisplayName("Harga Barang")]
        public decimal HargaBarang { get; set; }
        [DisplayName("Sub Total")]
        public decimal SubTotal { get; set; }//SubTotal harga per produk
        [DisplayName("Total")]
        public decimal Total { get; set; }//Total jumlah harga semua produk
        [DisplayName("Total Belanja")]
        public decimal TotalBayar { get; set; }//Total semua produk yang dibayar
        [DisplayName("Kembalian")]
        public decimal Kembalian { get; set; }//Total semua produk yang dibayar
        [DisplayName("Nominal Bayar")]
        public decimal NominalBayar { get; set; }
        [DisplayName("Jenis Custumer")]
        public string? JenisPelanggan { get; set; }//List nya diambil dari tabel pelanggan
        [DisplayName("Metode Pembayaran")]
        public string? MetodePembayaran { get; set; }//List nya diambil dari tabel MetodeBayar
        public string CariProduk { get; set; } = string.Empty;
        public string ScanBarcode { get; set; } = string.Empty;
    }
}
