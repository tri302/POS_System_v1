using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class SumberStok
    {
        public int Id { get; set; }
        public string Nama { get; set; } = string.Empty;
        [ForeignKey(nameof(TipeStok))]
        public int TipeStokId { get; set; }
        public TipeStok? TipeStok { get; set; }
    }
}
