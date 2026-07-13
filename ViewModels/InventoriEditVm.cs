using System.ComponentModel;
namespace POS_System.ViewModels
{
    public class InventoriEditVm
    {
        [DisplayName("No Referensi/No Faktur")]
        public string? NoReferensi { get; set; }
        [DisplayName("Tipe Stok")]
        public string? TipeStok { get; set; }
        public string? Sumber { get; set; }
        public string? Notes { get; set; }
        [DisplayName("Total Modal")]
        public decimal TotalModal { get; set; }
        [DisplayName("Harga Modal")]
        public decimal HargaModal { get; set; }
        public int Jumlah { get; set; }
        public long IdInventori { get; set; }
        public long IdProduk { get; set; }
        public int Stok { get; set; }
        public List<InventoriVm> Items { get; set; } = new();
        // Tambahan untuk dropdown produk
        public List<ProdukVm> ProdukList { get; set; } = new();        
    }
}
