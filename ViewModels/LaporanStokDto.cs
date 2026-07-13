namespace POS_System.ViewModels
{
    public class LaporanStokDto
    {
        public long IdProduk { get; set; }
        public string NamaProduk { get; set; }
        public string Kategori { get; set; }
        public string KodeBarcode { get; set; }
        public string SKU { get; set; }
        public string NamaSatuan { get; set; }
        public int Stok { get; set; }
        public decimal HargaModal { get; set; }
        public decimal HargaJual { get; set; }
        public string StatusProduk { get; set; }
    }
}
