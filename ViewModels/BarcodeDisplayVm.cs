namespace POS_System.ViewModels
{
    public class BarcodeDisplayVm
    {
        public string NamaProduk { get; set; } = string.Empty;
        public string KodeBarcode { get; set; } = string.Empty;
        public decimal HargaJual { get; set; }
        public string BarcodeBase64 { get; set; } = string.Empty; // Untuk menyimpan gambar barcode
    }
}
