using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace POS_System.ViewModels
{
    public class AlurKasVm
    {
        public long Id { get; set; }
        [DisplayName("Type")]
        [Required(ErrorMessage = "Tipe transaksi wajib dipilih.")]
        public int TipeAlurKas { get; set; }
        //Masuk
        //Penjualan, Restore Transaksi, Pembayaran Piutang, Modal Awal, Modal Tambahan, Pemasukan Lain-lain
        //Keluar
        //Penambahan Stok Barang, Gaji Karyawan, Biaya Operasional, Penarikan Oleh owner, Perawatan Alat, Return Pelanggan, Listrik, Sewa, Pengeluaran Lain-lain      
        [DisplayName("Source")]
        //[Required(ErrorMessage = "Source tidak boleh kosong.")]
        public string? SumberAlurKas { get; set; }
        [Required(ErrorMessage = "Amount wajib diisi.")]
        [Range(1, (double)999999999999999.99, ErrorMessage = "Amount harus lebih dari 0.")]
        public decimal Amount { get; set; }        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date tidak boleh kosong.")]
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        [DisplayName("Source")]
        public int IdCashFlow { get; set; }
        public string? CashFlowName { get; set; }
        public string? TypeName { get; set; }
        public int? IdTypeCashFlow { get; set; }
        public string? TotalModal { get; set; }
        public string? TotalUangMasuk { get; set; }
        public string? TotalUangKeluar { get; set; }
        public string? TotalUangToko { get; set; }
    }    
}
