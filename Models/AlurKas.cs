using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace POS_System.Models
{
    public class AlurKas
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        [NotMapped]
        public int TipeAlurKas { get; set; }//Masuk,Keluar
        //Masuk
        //Penjualan, Restore Transaksi, Pembayaran Piutang, Modal Awal, Modal Tambahan, Pemasukan Lain-lain
        //Keluar
        //Penambahan Stok Barang, Gaji Karyawan, Biaya Operasional, Penarikan Oleh owner, Perawatan Alat, Return Pelanggan, Listrik, Sewa, Pengeluaran Lain-lain              
        [NotMapped]
        public string? SumberAlurKas { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        //Properti Navigasi TblCashFlow
        public int IdCashFlow { get; set; }
        [ForeignKey(nameof(IdCashFlow))]
        public CashFlow? CashFlow { get; set; }
    }
}
