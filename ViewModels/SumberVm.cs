namespace POS_System.ViewModels
{
    public class SumberVm
    {
        public List<string> SumberMasuk { get; set; } = new List<string>
    {
        "Penambahan Stok"
    };
        public List<string> SumberKeluar { get; set; } = new List<string>
    {
        "Barang Rusak",
        "Kadaluarsa"
    };
        public List<string> SumberPenyesuaian { get; set; } = new List<string>
    {
        "Penyesuaian"
    };
    }
}
