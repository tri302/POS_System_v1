namespace POS_System.Models
{
    public class TipeStok
    {
        public int Id { get; set; } 
        public string Nama { get; set; } = string.Empty;
        public ICollection<SumberStok>? SumberStoks { get; set; }        
    }
}
