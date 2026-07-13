using POS_System.ViewModels;
namespace POS_System.Services
{
    public interface IDashboardService
    {        
        Task<List<ProdukTerlarisVm>> SearchProdukTerlarisAsync(string search, DateTime start, DateTime end);
        Task<List<StatusStokProdukVm>> SearchStatusStokAsync(string search);
        public void ClearDashboardCache(DateTime start, DateTime end);
        Task<DashboardIndexVm> GetDashboardAsync(
            RentangWaktu rentang,
            DateTime? from,
            DateTime? to);
        (DateTime Start, DateTime End) HitungTanggalPublic(
       RentangWaktu rentang,
       DateTime? from,
       DateTime? to);
        Task<List<ChartItemVM>> GetAlurKasChartAsync(
            DateTime start,
            DateTime end);
        Task<List<ChartItemVM>> GetTransaksiPerMetodeBayarChartAsync(
           DateTime start,
           DateTime end);
    }
}