using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
using POS_System.Repository;
using POS_System.Services;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class TransaksiController : Controller
    {
        private readonly ILogger<TransaksiController> _logger;
        private readonly IDashboardService _dashboard;
        private readonly ITransaksiRepo _transaksiRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IShiftsRepo _shiftsRepo;
        public TransaksiController(
            ILogger<TransaksiController> logger,
            IDashboardService dashboard,
            ITransaksiRepo transaksiRepo,
            UserManager<ApplicationUser> userManager,
            IShiftsRepo shiftsRepo,
            AppDbContext context)
        {
            _logger = logger;
            _dashboard = dashboard;
            _transaksiRepo = transaksiRepo;
            _userManager = userManager;
            _context = context;
            _shiftsRepo = shiftsRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetProductUnits(long idProduk)
        {
            // Ambil semua unit/satuan yang dimiliki oleh produk ini
            var units = await _context.ProductUnits
                .Where(pu => pu.IdProduk == idProduk)
                .Select(pu => new
                {
                    productUnitId = pu.ProductUnitId,
                    unitName = pu.UnitName
                })
                .ToListAsync();
            return Json(units); // ✅ Mengembalikan data JSON ke Ajax JavaScript
        }
        [Route("Transaksi/GetPelangganDetail/{Id}")]
        [HttpGet]
        public async Task<IActionResult> GetPelangganDetail(int Id)
        {
            var produk = await _context.Pelanggans
                .Where(p => p.Id == Id)
                .Select(p => new
                {
                    p.Email,
                    p.Phone
                })
                .FirstOrDefaultAsync();
            return Json(produk);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var (data, totalCount) = await _transaksiRepo.GetPaginatedAsync(pageNumber, pageSize);
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var transaksiVmList = data.Select(item => new TransaksiVm
            {
                IdTransaksi = item.IdTransaksi,
                NoTransaksi = item.NoTransaksi,
                NamaPemesan = item.Pelanggan?.Nama ?? "Umum/Tanpa Nama",
                TotalBelanja = item.TotalBelanja,
                Status = item.Status,
                MetodeBayar = item.MetodeBayar != null ? new MetodeBayar { NamaMetodeBayar = item.MetodeBayar.NamaMetodeBayar } : null
            }).ToList();
            var viewModel = new TransaksiIndexVm
            {
                TransaksiList = transaksiVmList,
                PageIndex = pageNumber,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
            return PartialView("_TransaksiTable", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Print(long id)
        {
            var transaksi = await _transaksiRepo.GetWithDetailsAsync(id);
            if (transaksi == null) return NotFound();
            ViewBag.TokoSettings = await _transaksiRepo.GetPengaturanTokoAsync();
            return View(transaksi);
        }
        [HttpPost]
        public async Task<IActionResult> BatalkanTransaksi(long id)
        {
            await _transaksiRepo.BeginTransactionAsync();
            try
            {
                // 1. Ambil transaksi beserta detail dan produknya
                var transaksi = await _transaksiRepo.GetWithDetailsAsync(id);
                if (transaksi == null)
                {
                    TempData["Error"] = "Transaksi tidak ditemukan.";
                    return RedirectToAction(nameof(Index));
                }
                if (transaksi.Status == "Dibatalkan")
                {
                    TempData["Warning"] = $"Transaksi No {transaksi.NoTransaksi} sudah berstatus Dibatalkan.";
                    return RedirectToAction(nameof(Index));
                }
                // 2. Iterasi untuk mengembalikan stok
                foreach (var detail in transaksi.TransaksiDetails)
                {
                    var product = detail.Produk; // Pastikan relasi ke Produk sudah di-load
                    if (product == null)
                    {
                        throw new Exception($"Produk tidak ditemukan untuk Transaksi Detail ID: {detail.IdTransaksi}");
                    }
                    // AMBIL KONVERSI DARI RELASI PRODUCTUNIT (Bukan dari detail langsung)
                    // Berdasarkan log EF Core Anda, relasi unit dipetakan ke ProductUnit/ProductUnitId
                    var nilaiKonversi = 1;
                    // Sesuaikan dengan nama properti navigation di model TransaksiDetail Anda (misal: detail.ProductUnit atau detail.ProductUnits)
                    if (detail.ProductUnit != null && detail.ProductUnit.ConversionValue > 0)
                    {
                        nilaiKonversi = detail.ProductUnit.ConversionValue;
                    }
                    else if (detail.Conversion > 0) // Fallback jika Anda menyimpan duplikatnya di detail
                    {
                        nilaiKonversi = detail.Conversion;
                    }
                    // Kembalikan stok dengan konversi yang valid (minimal 1)
                    product.Stok += (detail.Qty * nilaiKonversi);
                    await _transaksiRepo.UpdateProduk(product);
                }
                // 3. Update status transaksi
                transaksi.Status = "Dibatalkan";
                // transaksi.TanggalTransaksi = DateTime.Now; // Opsional: biasanya tanggal transaksi asli dipertahankan
                _transaksiRepo.UpdateTransaksi(transaksi);
                await _transaksiRepo.SaveChangesAsync();
                await _transaksiRepo.CommitTransactionAsync();
                TempData["Success"] = $"Transaksi No {transaksi.NoTransaksi} berhasil dibatalkan.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _transaksiRepo.RollbackTransactionAsync();
                // Logging error (sebaiknya gunakan logger, bukan langsung throw exception ke UI)
                TempData["Error"] = "Pembatalan Transaksi gagal: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetUnitDetail(long productUnitId)
        {
            var unit = await _context.ProductUnits
                .Include(x => x.Produk)
                .Where(x => x.ProductUnitId == productUnitId)
                .Select(x => new
                {
                    productUnitId = x.ProductUnitId,
                    hargaModal = x.HargaModal,
                    hargaJual = x.HargaJual,
                    stok = x.Produk.Stok,
                    unitName = x.UnitName,
                    conversion = x.ConversionValue
                })
                .FirstOrDefaultAsync();
            if (unit == null)
                return Json(null);
            return Json(unit);
        }
        [HttpGet]
        public async Task<IActionResult> _LoadDetail(long idTransaksi)
        {
            // 1. Ambil detail transaksi terlebih dahulu
            var details = await _transaksiRepo.GetDetailsByTransaksiIdAsync(idTransaksi);
            // 2. Ambil semua IdProduk yang ada di transaksi ini untuk ditarik data satuannya sekaligus (efisien)
            var productIds = details.Select(td => td.IdProduk).Distinct().ToList();
            // 3. Ambil semua unit yang tersedia untuk produk-produk terkait
            var allUnits = await _context.ProductUnits
                .Where(pu => productIds.Contains(pu.IdProduk)) // Sesuaikan nama field IdProduk di table ProductUnits Anda
                .Select(pu => new
                {
                    pu.IdProduk,
                    pu.ProductUnitId,
                    pu.UnitName,
                    pu.ConversionValue // atau pu.Conversion sesuai field database Anda
                })
                .ToListAsync();
            // 4. Map ke ViewModel
            var vmItems = details.Select(td => new TransaksiItemVm
            {
                IdProduk = td.IdProduk,
                NamaProduk = td.Produk?.NamaProduk,
                Conversion = td.Conversion,
                Qty = td.Qty,
                ProductUnitId = td.ProductUnitId,
                HargaModal = td.ProductUnit?.HargaModal ?? 0,
                HargaJual = td.HargaJual,
                Stok = td.Produk?.Stok ?? 0,
                UnitName = td.ProductUnit?.UnitName,
                KodeBarcode = td.Produk?.KodeBarcode,
                // Filter satuan yang HANYA dimiliki oleh produk ini
                AvailableUnitsOption = allUnits
                    .Where(u => u.IdProduk == td.IdProduk)
                    .Select(u => new ProductUnitOptionVm
                    {
                        ProductUnitId = u.ProductUnitId,
                        UnitName = u.UnitName,
                        Conversion = u.ConversionValue
                    }).ToList()
            }).ToList();
            return PartialView("~/Views/Transaksi/_PartialTransaksiDetail.cshtml", vmItems);
        }
        [HttpGet]
        public async Task<IActionResult> Detail(long id)
        {
            var transaksi = await _transaksiRepo.GetWithDetailsAsync(id);
            if (transaksi == null) return NotFound();
            var vm = new TransaksiVm
            {
                IdTransaksi = transaksi.IdTransaksi,
                NoTransaksi = transaksi.NoTransaksi,
                NamaPemesan = transaksi.Pelanggan?.Nama ?? "Umum",
                IdMetodeBayar = transaksi.IdMetodeBayar,
                NamaMetodeBayar = transaksi.MetodeBayar?.NamaMetodeBayar ?? "-",
                TanggalTransaksi = transaksi.TanggalTransaksi != DateTime.MinValue ? transaksi.TanggalTransaksi : DateTime.Now,
                TransaksiDetails = transaksi.TransaksiDetails.Select(td =>
                {
                    var finalHargaModal = td.HargaModal > 0 ? td.HargaModal : (td.ProductUnit?.HargaModal ?? 0);
                    var finalHargaJual = td.HargaJual > 0 ? td.HargaJual : (td.ProductUnit?.HargaJual ?? 0);
                    return new TransaksiDetailVm
                    {
                        Gambar = td.Produk?.Gambar ?? "/img/no-image.png",
                        NamaProduk = td.Produk?.NamaProduk,
                        UnitName = td.ProductUnit?.UnitName,
                        Qty = td.Qty,
                        HargaModal = finalHargaModal,
                        HargaJual = finalHargaJual,
                        Profit = (finalHargaJual - finalHargaModal) * td.Qty
                    };
                }).ToList()
            };
            return View(vm);
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var (data, totalCount) = await _transaksiRepo.GetPaginatedAsync(pageNumber, pageSize);
            // Hitung total halaman terlebih dahulu
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            // 💡 PROSES MAPPING: Konversi dari List<Transaksi> ke List<TransaksiVm>
            var transaksiVmList = data.Select(item => new TransaksiVm
            {
                IdTransaksi = item.IdTransaksi,
                NoTransaksi = item.NoTransaksi,
                // Sesuaikan 'NamaPelanggan' dengan nama kolom asli di tabel pelanggan Anda
                NamaPemesan = item.Pelanggan != null ? item.Pelanggan.Nama : "Umum/Tanpa Nama",
                TotalBelanja = item.TotalBelanja,
                Status = item.Status,
                // Map properti relasi MetodeBayar jika ada
                MetodeBayar = item.MetodeBayar != null ? new MetodeBayar
                {
                    NamaMetodeBayar = item.MetodeBayar.NamaMetodeBayar
                } : null
            }).ToList();
            // Masukkan data yang sudah di-mapping ke ViewModel Utama
            var viewModel = new TransaksiIndexVm
            {
                TransaksiList = transaksiVmList, // Sekarang tipe datanya sudah match! ✅
                PageIndex = pageNumber,
                TotalPages = totalPages,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var pelangganList = await _context.Pelanggans
                .Select(p => new { p.Id, p.Nama })
                .ToListAsync();
            ViewBag.PelangganList = new SelectList(pelangganList, "Id", "Nama");
            var metodeBayarList = await _context.MetodeBayars
                .Select(m => new
                {
                    m.IdMetodeBayar,
                    m.NamaMetodeBayar
                })
                .ToListAsync();
            ViewBag.MetodeBayarList = new SelectList(metodeBayarList, "IdMetodeBayar", "NamaMetodeBayar");
            // Ambil data transaksi utama beserta relasi yang diperlukan (misal Pelanggan)
            var transaksi = await _transaksiRepo.GetWithDetailsAsync(id);
            if (transaksi == null)
            {
                return NotFound(); // Form 404 asli jika datanya memang tidak ada di DB
            }
            /// 2. Mapping dari Model (Transaksi) ke ViewModel (TransaksiVm) 💡
            var viewModel = new TransaksiVm
            {
                IdTransaksi = transaksi.IdTransaksi,
                NoTransaksi = transaksi.NoTransaksi,
                IdPelanggan = transaksi.IdPelanggan,
                TotalBelanja = transaksi.TotalBelanja,
                Status = transaksi.Status,
                IdMetodeBayar = transaksi.IdMetodeBayar,
                // Jika di ViewModel Anda membutuhkan nama customer atau nama metode bayar:
                NamaPemesan = transaksi.Pelanggan?.Nama ?? "Umum/Tanpa Nama",
                Email = transaksi.Pelanggan.Email,
                Phone = transaksi.Pelanggan.Phone,
                // 🔥 ISI MAP DETAIL BARANG DI SINI AGAR BERIKUT KESETURUTAN KONVERSI TIAP ITEM
                //Items = transaksi.TransaksiDetails.Select(d => new TransaksiItemVm // Sesuaikan nama class detail VM Anda
                //{
                //    IdProduk = d.IdProduk,
                //    NamaProduk = d.ProductUnit?.Produk?.NamaProduk, // Ambil nama produk dari relasi
                //    ProductUnitId = d.ProductUnitId,
                //    UnitName = d.ProductUnit?.UnitName, // Sesuaikan properti nama unit (Dus/Pcs)
                //    Qty = d.Qty,
                //    HargaJual = d.HargaJual,
                //    Profit = d.Profit,
                //    Conversion = d.Conversion // 🔥 Nilai Konversi dari DB masuk ke sini!
                //}).ToList()
            };
            // 3. Kirim viewModel yang tipenya sudah sesuai dengan kebutuhan Edit.cshtml
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] TransaksiVm model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Validasi gagal" });
            try
            {
                //Ambil data transaksi terlebih dahulu untuk cek status
                var transaksiExisting = await _transaksiRepo.GetWithDetailsAsync(model.IdTransaksi);
                if (transaksiExisting == null)
                    return NotFound(new { message = "Transaksi tidak ditemukan." });
                //PROTEKSI: Jika sudah dibatalkan, TIDAK BOLEH DI-EDIT
                if (transaksiExisting.Status == "Dibatalkan")
                {
                    return BadRequest(new { message = "Transaksi yang sudah dibatalkan tidak dapat diubah kembali." });
                }
                bool isSuccess = await _transaksiRepo.EditTransaksiWithDetailsAsync(model);
                if (!isSuccess)
                    return NotFound(new { message = "Transaksi gagal diperbarui." });
                return Json(new { message = "Transaksi berhasil diperbarui" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan sistem: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //List Pelanggan
            var pelangganList = await _context.Pelanggans
                .OrderByDescending(p => p.IsPinned)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Nama
                }).ToListAsync();
            //Tampung ke ViewBag
            ViewBag.PelangganList = new SelectList(pelangganList, "Id", "Name");
            //List Metode Bayar
            var metodeBayarList = await _context.MetodeBayars
                .Select(m => new
                {
                    m.IdMetodeBayar,
                    m.NamaMetodeBayar
                })
                .ToListAsync();
            //Tampung ke ViewBag
            ViewBag.MetodeBayarList = new SelectList(metodeBayarList, "IdMetodeBayar", "NamaMetodeBayar");
            var model = new POS_System.ViewModels.TransaksiVm();
            // Kirim objek model tersebut ke dalam View
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] TransaksiVm model)
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "Validasi gagal" });
            var userId = _userManager.GetUserId(User);
            var karyawan = await _transaksiRepo.GetKaryawanByUserIdAsync(userId);
            if (karyawan == null) return BadRequest(new { message = "Akun Anda belum terdaftar sebagai Karyawan/Staff." });
            var activeShift = await _shiftsRepo.GetActiveShiftAsync(karyawan.IdKaryawan);
            if (activeShift == null)
            {
                return BadRequest(new { message = "Tidak ada shift aktif! Silakan buka shift terlebih dahulu." });
            }
            await _transaksiRepo.BeginTransactionAsync();
            try
            {
                var transaksi = new Transaksi
                {
                    IdPelanggan = model.IdPelanggan,
                    IdKaryawan = karyawan.IdKaryawan,
                    IdShift = activeShift.IdShift,
                    NoTransaksi = "TRX-" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                    TanggalTransaksi = DateTime.Now,
                    IdMetodeBayar = model.IdMetodeBayar,
                    TotalBelanja = model.TotalBelanja,
                    NominalBayar = model.NominalBayar,
                    Kembalian = model.Kembalian,
                    Notes = model.Notes,
                    Status = "Berhasil"
                };
                await _transaksiRepo.AddTransaksiAsync(transaksi);
                await _transaksiRepo.SaveChangesAsync();
                foreach (var item in model.Items)
                {
                    // Hitung total fisik pcs yang keluar
                    int totalPcsKeluar = item.Qty * item.Conversion;
                    var produk = await _transaksiRepo.GetProdukByIdAsync(item.IdProduk);
                    // Cek stok berdasarkan total pcs
                    if (produk == null || produk.Stok < totalPcsKeluar)
                    {
                        await _transaksiRepo.RollbackTransactionAsync();
                        return BadRequest(new { message = $"Stok tidak cukup untuk {item.Qty} unit." });
                    }
                    var detail = new TransaksiDetail
                    {
                        IdTransaksi = transaksi.IdTransaksi,
                        IdProduk = item.IdProduk,
                        ProductUnitId = item.ProductUnitId,
                        Qty = item.Qty,
                        HargaJual = item.HargaJual,
                        HargaModal = item.HargaModal,
                        Conversion = item.Conversion,
                        Profit = item.Profit
                    };
                    await _transaksiRepo.AddTransaksiDetailAsync(detail);
                    produk.Stok -= totalPcsKeluar;
                }
                await _transaksiRepo.SaveChangesAsync();
                await _transaksiRepo.CommitTransactionAsync();
                _dashboard.ClearDashboardCache(transaksi.TanggalTransaksi, transaksi.TanggalTransaksi);
                return Json(new { message = "Transaksi berhasil disimpan", noTransaksi = transaksi.NoTransaksi });
            }
            catch (Exception ex)
            {
                await _transaksiRepo.RollbackTransactionAsync();
                var innerError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { message = "Gagal membuat transaksi. Error: " + innerError });
            }
        }
        [HttpGet]
        [Route("/Transaksi/SearchProdukAsync")]
        public async Task<IActionResult> SearchProdukAsync(string term)
        {
            var data = await _transaksiRepo.SearchProdukAsync(term ?? "");
            var results = data.Select(p => new { id = p.IdProduk, text = p.NamaProduk });
            return Json(new { results = results });
        }
        [HttpGet]
        [Route("/Transaksi/GetProdukDetail/{id}")]
        public async Task<IActionResult> GetProdukDetail(long id)
        {
            var data = await _transaksiRepo.GetProdukDetailAsync(id);
            if (data == null)
            {
                return NotFound(new { message = "Produk tidak ditemukan" });
            }
            return Json(data);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            var product = await _context.Produks
                .FirstOrDefaultAsync(x => x.KodeBarcode == barcode);
            if (product == null)
                return Json(null);
            return Json(new
            {
                idProduk = product.IdProduk
            });
        }
    }
}
