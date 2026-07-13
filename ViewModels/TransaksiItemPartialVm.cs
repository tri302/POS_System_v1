namespace POS_System.ViewModels
{
    public class TransaksiItemPartialVm
    {
        // Add this new property
        public int Index { get; set; }
        public ProdukVm? Items { get; set; }   // diganti ProdukVm
        public POS_System.ViewModels.TransaksiVm Item { get; set; } = new();
        public IEnumerable<POS_System.ViewModels.ProdukVm> ProdukList { get; set; } = new List<POS_System.ViewModels.ProdukVm>();
    }
}
