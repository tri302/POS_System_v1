using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using POS_System.Models;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.ViewModels
{
    public class TransaksiDetailVm
    {
        public long IdProduk { get; set; }
        public string? Gambar { get; set; }
        public string? NamaProduk { get; set; }
        public int Qty { get; set; }
        public decimal HargaModal { get; set; }
        public decimal HargaJual { get; set; }
        public decimal Profit { get; set; }
        public int Stok { get; set; }
        //Foreign Key relation to ProductUnit
        public long ProductUnitId { get; set; }
        public ProductUnit? ProductUnit { get; set; }
        public string? UnitName { get; set; }// pcs, dus, pack
        [NotMapped]
        [ValidateNever]
        public int Conversion { get; set; }
    }
}
