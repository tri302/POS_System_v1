using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    [Table("Logs")]
    public class Log
    {
        [Key]
        public int Id { get; set; }
        public string? Message { get; set; }
        public string? MessageTemplate { get; set; }
        [MaxLength(50)]
        public string? Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? Exception { get; set; }
        public string? Properties { get; set; }
    }
}
