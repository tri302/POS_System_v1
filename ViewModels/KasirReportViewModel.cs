namespace POS_System.ViewModels
{
    public class KasirReportViewModel
    {
        public class KasirReportSummary
        {
            public int IdKaryawan { get; set; }
            public int IdShift { get; set; }
            public string? NamaKaryawan { get; set; }
            public decimal TotalUangMasuk { get; set; }   // Sum(NominalBayar)
            public decimal TotalKembalian { get; set; }   // Sum(Kembalian)
            public decimal TotalPendapatanBersih { get; set; } // Sum(TotalBelanja)
            public decimal SaldoAwal { get; set; }
            public decimal Selisih { get; set; }
            public decimal SaldoAkhirSistem { get; set; }
            public decimal SaldoAkhirFisik { get; set; }
        }

        public class KasirReportDetail
        {
            public string? NoTransaksi { get; set; }
            public DateTime TanggalTransaksi { get; set; }
            public decimal TotalBelanja { get; set; }
            public decimal NominalBayar { get; set; }
            public decimal Kembalian { get; set; }
            public string? Status { get; set; }
        }

        // Bungkus kedua data di atas untuk dikirim ke View
        public class KasirReportResult
        {
            public KasirReportSummary Summary { get; set; } = new KasirReportSummary();
            public List<KasirReportDetail> Details { get; set; } = new List<KasirReportDetail>();
        }
    }
}
