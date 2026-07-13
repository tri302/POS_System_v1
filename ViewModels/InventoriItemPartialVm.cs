namespace POS_System.ViewModels
{
    public class InventoriItemPartialVm
    {
        public long IdInventori { get; set; }
        public int Index { get; set; }        
        public ProdukVm? Items { get; set; }   // diganti ProdukVm
        public InventoriVm Item { get; set; } = new();
        public ProductUnitDto? ProductUnit { get; set; }
        public IEnumerable<ProdukVm> ProdukList { get; set; } = new List<ProdukVm>();
    }
}
