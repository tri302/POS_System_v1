using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace POS_System.ViewModels
{
    public class ProductUnitDto
    {
        [JsonPropertyName("productunitid")]
        public long ProductUnitId { get; set; }
        [JsonPropertyName("unitName")]
        public string? UnitName { get; set; }// pcs, dus, pack       
        [JsonPropertyName("hargaJual")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaJual { get; set; }
        [JsonPropertyName("hargaModal")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HargaModal { get; set; }
        [JsonPropertyName("stok")]
        public int Stok { get; set; }
        [JsonPropertyName("conversionValue")]
        public int ConversionValue { get; set; }
        [JsonPropertyName("is_Default")]
        public bool Is_Default { get; set; }
    }
}
