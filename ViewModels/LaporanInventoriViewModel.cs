namespace POS_System.ViewModels
{
    public class LaporanInventoriViewModel
    {
        public string NoReferensi { get; set; }
        public string TipeStok { get; set; }
        public string Sumber { get; set; }
        public DateTime Tanggal { get; set; } // Tambahkan Tanggal untuk filtering dan laporan
        public string NamaProduk { get; set; }
        public string SatuanUnit { get; set; }
        public int JumlahProduk { get; set; }
        public decimal HargaModal { get; set; }
        public decimal TotalModal { get; set; }
        public string Catatan { get; set; }
    }
}
