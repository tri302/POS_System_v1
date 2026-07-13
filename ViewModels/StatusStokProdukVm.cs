namespace POS_System.ViewModels
{
    public class StatusStokProdukVm
    {
        public long IdProduk { get; set; }
        public string? NamaProduk { get; set; }
        public string? UnitName { get; set; }// pcs, dus, pack
        public int Stok { get; set; }
        public string? Status { get; set; } // e.g.,"Aman", "Rendah", "Habis", etc.
        public string? GambarUrl { get; set; }
    }
}
