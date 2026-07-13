namespace POS_System.ViewModels
{
    public class ProductUnitVm
    {
        public long ProductUnitId { get; set; }
        public string? UnitName { get; set; } // Asumsi ini adalah nama unit (misal: "pcs", "dus")
        public decimal HargaJual { get; set; }
        public int Stok { get; set; }
        public int Conversion { get; set; }
    }
}
