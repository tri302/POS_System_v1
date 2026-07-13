namespace POS_System.Models
{
    public class JenisPelanggan
    {
        public int Id { get; set; }
        public string NamaJenis { get; set; } = string.Empty;
        public ICollection<Pelanggan>? Pelanggans { get; set; }
    }
}
