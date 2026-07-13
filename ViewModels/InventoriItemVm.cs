namespace POS_System.ViewModels
{
    public class InventoriItemVm
    {
        public long IdProduk { get; set; }
        public string NamaProduk { get; set; } = string.Empty;
        public decimal HargaModal { get; set; }
        public int Jumlah { get; set; }
        public long ProductUnitId { get; set; } 
    }
}
