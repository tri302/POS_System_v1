using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POS_System.Services;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class DashboardController(IDashboardService _dashboard) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> FilterProdukTerlaris(string search, DateTime start, DateTime end)
        {
            // Cari langsung ke DB lewat service baru
            var hasilSearch = await _dashboard.SearchProdukTerlarisAsync(search, start, end);
            // Bungkus ke ViewModel agar Partial View tidak error
            var vm = new DashboardIndexVm
            {
                ProdukTerlaris = hasilSearch
            };
            ViewBag.SearchLaris = search; // Agar text input tidak hilang setelah update
            return PartialView("_PartialProdukTerlaris", vm);
        }
        [HttpGet]
        public async Task<IActionResult> FilterStatusStok(string search, string rentang)
        {
            // Cari langsung ke DB lewat service baru
            var hasilSearch = await _dashboard.SearchStatusStokAsync(search);
            // Bungkus ke ViewModel agar Partial View tidak error
            var vm = new DashboardIndexVm
            {
                StatusStokProduk = hasilSearch
            };
            ViewBag.SearchStatusStok = search;
            return PartialView("_PartialStatusStokProduk", vm);
        }
        [HttpGet]
        public async Task<IActionResult> LoadArusKasChart(RentangWaktu rentang, DateTime? from, DateTime? to)
        {
            var (start, end) = _dashboard.HitungTanggalPublic(rentang, from, to);
            var data = await _dashboard.GetAlurKasChartAsync(start, end);
            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> LoadTransaksiPerMetodeBayarChart(
            RentangWaktu rentang = RentangWaktu.HariIni, DateTime? from = null,
            DateTime? to = null)
        {
            var (start, end) = _dashboard.HitungTanggalPublic(rentang, from, to);
            var data = await _dashboard.GetTransaksiPerMetodeBayarChartAsync(start, end);
            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> Index(string rentangWaktu = "hari")
        {
            var vm = await _dashboard.GetDashboardAsync(
            RentangWaktu.HariIni, null, null);
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> LoadDashboard(RentangWaktu rentang, DateTime? from, DateTime? to)
        {
            var vm = await _dashboard.GetDashboardAsync(rentang, from, to);
            return PartialView("_PartialDashboard", vm);
        }
    }
}
