namespace POS_System.Models
{
    public class LaporanProduk
    {
        public string? KodeLaporanProduk { get; set; }        
        public string? TipeLaporan { get; set; } //e.g. Stok Habis, Stok Tersedia, Stok Menipis, Stok Banyak       
        public DateTime? TanggalMulai { get; set; }
        public DateTime? TanggalSelesai { get; set; }
    }
}
