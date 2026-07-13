namespace POS_System.ViewModels
{
    public class DashboardIndexVm
    {
        // Filter
        public RentangWaktu RentangWaktu { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        // Card
        public decimal Penjualan { get; set; }
        public decimal UangMasuk { get; set; }
        public decimal UangKeluar { get; set; }
        public decimal TotalUangMasuk { get; set; }
        public decimal TotalUangKeluar { get; set; }
        public decimal TotalUangToko { get; set; }
        // Table
        public List<ProdukTerlarisVm> ProdukTerlaris { get; set; } = new();
        public List<StatusStokProdukVm> StatusStokProduk { get; set; } = new();
        // Chart
        public List<ChartItemVM> ChartItem { get; set; } = new();
        public List<ChartItemVM> TransaksiPerMetode { get; set; } = new();
        //public List<ProdukTerlarisVM> ProdukTerlaris { get; set; } = new List<ProdukTerlarisVM>();
        //public List<ChartItemVM> ChartItem { get; set; } = new List<ChartItemVM>();
        //public List<StatusStokProdukVM> StatusStokProduk { get; set; } = new List<StatusStokProdukVM>();
        //public decimal TotalUangMasuk { get; set; }
        //public decimal TotalUangKeluar { get; set; }
        //public decimal TotalUangToko { get; set; }
        //public int TotalPenjualan { get; set; }
        //public int TotalTransaksi { get; set; }
        //public int TotalProdukTerjual { get; set; }
        //public int TotalPelanggan { get; set; }
    }
}
