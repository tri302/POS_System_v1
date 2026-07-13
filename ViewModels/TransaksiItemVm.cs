using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.ViewModels
{
    public class TransaksiItemVm
    {
        public long IdProduk { get; set; }
        public string? NamaProduk { get; set; }
        [ValidateNever]
        public string? KodeBarcode { get; set; }
        public string? Gambar { get; set; }
        public string? SKU { get; set; }
        public int Qty { get; set; }
        public decimal Harga { get; set; }
        public decimal HargaModal { get; set; }
        public decimal HargaJual { get; set; }
        public decimal Profit { get; set; }
        public int Stok { get; set; }
        public long ProductUnitId { get; set; }
        public string? UnitName { get; set; }// pcs, dus, pack
                                             // Tambahkan list unit baru        
        [ValidateNever]
        public int Conversion { get; set; }
        [NotMapped]
        [ValidateNever]
        public List<TransaksiItemVm>? Items { get; set; } = new List<TransaksiItemVm>();
        public List<ProductUnitVm> AvailableUnits { get; set; } = new List<ProductUnitVm>();
        // Tambahkan properti ini untuk menampung daftar opsi satuan khusus produk ini
        [NotMapped]
        [ValidateNever]
        public List<ProductUnitOptionVm> AvailableUnitsOption { get; set; } = new List<ProductUnitOptionVm>();
    }
}
