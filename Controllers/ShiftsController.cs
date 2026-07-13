using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS_System.Models;
using POS_System.Repository;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner,Kasir")]
    public class ShiftsController : Controller
    {
        private readonly IShiftsRepo _shiftsRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITransaksiRepo _transaksiRepo;
        public ShiftsController(IShiftsRepo shiftsRepo, UserManager<ApplicationUser> userManager, ITransaksiRepo transaksiRepo)
        {
            _shiftsRepo = shiftsRepo;
            _userManager = userManager;
            _transaksiRepo = transaksiRepo;
        }
        // 1. HALAMAN UTAMA DASHBOARD SHIFT KASIR
        public async Task<IActionResult> Index()
        {            
            var userId = _userManager.GetUserId(User);
            var karyawan = await _transaksiRepo.GetKaryawanByUserIdAsync(userId);
            if (karyawan == null) return BadRequest(new { message = "Akun Anda belum terdaftar sebagai Karyawan/Staff." });
            // Cek apakah kasir ini sedang memiliki shift yang aktif
            var activeShift = await _shiftsRepo.GetActiveShiftAsync(karyawan.IdKaryawan);
            // Cek apakah shift aktif adalah shift dari hari sebelumnya
            if (activeShift != null && activeShift.WaktuBuka.Date < DateTime.Today)
            {
                ViewBag.IsExpiredShift = true; // Flag untuk view
                TempData["Warning"] = "Anda memiliki shift yang belum ditutup dari hari sebelumnya! Harap segera lakukan penutupan.";
            }
            // Kirim data shift aktif ke View (jika null, berarti kasir harus Open Shift)
            return View(activeShift);
        }
        // 2. ACTION: PROSES BUKA SHIFT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OpenShift(decimal saldoAwal)
        {
            var userId = _userManager.GetUserId(User);
            var karyawan = await _transaksiRepo.GetKaryawanByUserIdAsync(userId);
            if (karyawan == null)
                return Json(new { success = false, message = "Akun tidak terdaftar." });
            if (saldoAwal < 0)
                return Json(new { success = false, message = "Modal awal tidak boleh minus!" });
            var newShift = new Shift { IdKaryawan = karyawan.IdKaryawan, SaldoAwal = saldoAwal };
            bool isSuccess = await _shiftsRepo.OpenShiftAsync(newShift);
            if (isSuccess)
            {
                return Json(new { success = true, message = "Shift berhasil dibuka!", redirectUrl = Url.Action("Index", "Kasir") });
            }
            return Json(new { success = false, message = "Gagal membuka shift. Anda masih memiliki shift yang belum ditutup!" });
        }
        // 3. ACTION: PROSES TUTUP SHIFT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseShift(int idShift, decimal saldoAkhirFisik)
        {
            if (saldoAkhirFisik < 0)
            {
                TempData["Error"] = "Saldo fisik akhir tidak boleh minus!";
                return RedirectToAction(nameof(Index));
            }
            bool isSuccess = await _shiftsRepo.CloseShiftAsync(idShift, saldoAkhirFisik);
            if (isSuccess)
            {
                TempData["Success"] = "Shift Berhasil ditutup! Data rekonsiliasi kas telah disimpan.";
            }
            else
            {
                TempData["Error"] = "Gagal menutup shift. Data tidak ditemukan atau sudah pernah ditutup.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
