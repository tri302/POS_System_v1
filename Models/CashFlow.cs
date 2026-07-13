using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class CashFlow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key] 
        public int IdCashFlow { get; set; }
        public string? CashFlowName { get; set; }
        //Properti Navigasi TblTipeCashFlow
        public int IdTipeCashFlow { get; set; }
        [ForeignKey(nameof(IdTipeCashFlow))]
        public TipeCashFlow TipeCashFlow { get; set; } = new();        
    }
}
