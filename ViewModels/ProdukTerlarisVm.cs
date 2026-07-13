namespace POS_System.ViewModels
{
    public class ProdukTerlarisVm
    {
        public long IdProduk { get; set; }
        public string? UnitName { get; set; }// pcs, dus, pack
        public string NamaProduk { get; set; } = string.Empty;
        public int TotalTerjual { get; set; }
        public string? GambarUrl { get; set; }
    }
}
