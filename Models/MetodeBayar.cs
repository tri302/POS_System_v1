using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class MetodeBayar
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdMetodeBayar { get; set; }
        public string? NamaMetodeBayar { get; set; } //e.g. Cash, Transfer, Credit Card, etc.        
        public string? UrlGambar { get; set; }
    }
}
