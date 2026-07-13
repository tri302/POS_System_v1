using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class TipeCashFlow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int IdTipeCashFlow { get; set; }
        public string? TipeCashFlowName { get; set; }//Masuk atau Keluar
    }
}
