using POS_System.Models;
namespace POS_System.Repository
{
    public interface ILaporanKeuanganRepo
    {
        // Tambahkan parameter tanggal di sini agar filter dilakukan di DB (SQL), bukan di RAM server
        Task<List<Transaksi>> GetLaporanPenjualanAsync(DateTime tglMulai, DateTime tglSelesai);
        Task<List<AlurKas>> GetLaporanKasAsync(string tipeLaporan, DateTime tglMulai, DateTime tglSelesai);        
        Task<(List<LaporanKeuangan> Data, int TotalCount)> GetPaginatedLaporanAsync(int pageNumber, int pageSize);
        Task<LaporanKeuangan> GetByIdAsync(string id);
        Task<string> GenerateKodeLaporanAsync();
        Task AddAsync(LaporanKeuangan laporan);
        Task UpdateAsync(LaporanKeuangan laporan);
        Task<bool> SaveChangesAsync();
    }
}
