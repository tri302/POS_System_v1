using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Repository;
using static POS_System.ViewModels.KasirReportViewModel;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class ReportController : Controller
    {
        private readonly IReportRepo _reportRepo;
        private readonly AppDbContext _context;        
        public ReportController(IReportRepo reportRepo, AppDbContext context)
        {
            _reportRepo = reportRepo;
            _context = context;
        }
        [HttpPost] 
        public async Task<IActionResult> RevisiShift(int idShift, int idKaryawan, decimal saldoFisikBenar, string keterangan, DateTime tanggal)
        {
            var shift = await _context.Shifts.FindAsync(idShift);
            if (shift == null) return NotFound();
            // Hitung ulang selisih berdasarkan input baru dari Owner
            shift.IdKaryawan = idKaryawan;
            shift.SaldoAkhirFisikRevisi = saldoFisikBenar;
            shift.SelisihRevisi = saldoFisikBenar - shift.SaldoAkhirSistem;
            shift.KeteranganRevisi = keterangan;
            shift.TanggalRevisi = DateTime.Now;
            await _context.SaveChangesAsync();
            // PERBAIKAN: Arahkan kembali ke Laporan Kasir dengan parameter query string yang sesuai
            return RedirectToAction("KasirReport", "Report", new { idKaryawan = idKaryawan, tanggal = tanggal.ToString("yyyy-MM-dd") });
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(KasirReport));
        }
        // Action Utama untuk menampilkan halaman Laporan
        // Jika parameter kosong, default akan mengambil hari ini
        public async Task<IActionResult> KasirReport(int? idKaryawan, DateTime? tanggal)
        {
            // 1. Ambil data untuk dropdown Filter Karyawan/Kasir
            // Pastikan nama properti Id dan NamaKaryawan sesuai dengan model asli Anda
            var daftarKaryawan = await _context.Karyawans
                .Select(k => new { k.IdKaryawan, k.Name })
                .ToListAsync();
            // Masukkan ke ViewBag agar bisa dibaca oleh @Html.DropDownList atau <select> di View
            ViewBag.KaryawanList = new SelectList(daftarKaryawan, "IdKaryawan", "Name", idKaryawan);
            // 2. Tentukan default filter jika parameter null (Hari ini)
            DateTime filterTanggal = tanggal ?? DateTime.Today;
            ViewBag.TanggalSelected = filterTanggal.ToString("yyyy-MM-dd");
            ViewBag.IdKaryawanSelected = idKaryawan;
            // 3. Jika pengguna sudah memilih karyawan, tarik data dari repository
            if (idKaryawan.HasValue && idKaryawan.Value > 0)
            {
                var reportData = await _reportRepo.GetKasirReportAsync(idKaryawan.Value, filterTanggal);
                return View(reportData);
            }
            // Jika belum ada karyawan yang dipilih (saat pertama kali buka halaman), kembalikan model kosong
            return View(new KasirReportResult());
        }
    }
}