using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using POS_System.Models;
using POS_System.Repository;
using POS_System.Services;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner,Kasir")]
    public class KasirController : Controller
    {
        private readonly ILogger<Transaksi> _logger;
        private readonly IDashboardService _dashboard;
        private readonly IKasirRepo _kasirRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShiftsRepo _shiftsRepo;
        private const int PageSize = 8;
        public KasirController(
            ILogger<Transaksi> logger,
            IDashboardService dashboard,
            IKasirRepo kasirRepo,
            UserManager<ApplicationUser> userManager,
            IShiftsRepo shiftsRepo)
        {
            _logger = logger;
            _dashboard = dashboard;
            _kasirRepo = kasirRepo;
            _userManager = userManager;
            _shiftsRepo = shiftsRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetUnitDetails(int productId)
        {
            var productUnits = await _kasirRepo.GetUnitDetailsAsync(productId);
            return Json(productUnits);
        }
        [HttpGet]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return NotFound();
            barcode = barcode.Trim();
            var product = await _kasirRepo.GetProductByBarcodeAsync(barcode) as Produk;
            if (product == null)
                return NotFound();
            return Json(new
            {
                idProduk = product.IdProduk,
                namaProduk = product.NamaProduk,
                gambar = product.ProdukGambars.FirstOrDefault()?.PathGambar ?? "/images/no-image.png",
                stok=product.Stok,
                availableUnits = product.ProductUnits.Select(u => new
                {
                    productUnitId = u.ProductUnitId,
                    unitName = u.UnitName,
                    hargaJual = u.HargaJual,
                    //stok = u.Stok
                }).ToList()
            });
        }
        [HttpGet]
        public async Task<IActionResult> SearchProducts(int page = 1, string query = "", bool isBarcode = false, int? categoryId = null)
        {
            var (count, paginatedProducts) = await _kasirRepo.SearchProductsAsync(page, PageSize, query, categoryId);
            var items = paginatedProducts.Cast<Produk>().Select(p =>
            {
                var defaultUnit = p.ProductUnits.OrderBy(u => u.HargaJual).FirstOrDefault();
                return new TransaksiItemVm
                {
                    IdProduk = p.IdProduk,
                    NamaProduk = p.NamaProduk,
                    SKU = p.SKU,
                    Stok =p.Stok,
                    ProductUnitId = defaultUnit?.ProductUnitId ?? 0,
                    HargaJual = defaultUnit?.HargaJual ?? 0,
                    //Stok = defaultUnit?.Stok ?? 0,
                    AvailableUnits = p.ProductUnits.Select(u => new ProductUnitVm
                    {
                        ProductUnitId = u.ProductUnitId,
                        UnitName = u.UnitName,
                        HargaJual = u.HargaJual,
                        Conversion = u.ConversionValue
                    }).ToList(),
                    Gambar = p.ProdukGambars.FirstOrDefault()?.PathGambar ?? "/images/no-image.png"
                };
            }).ToList();
            var viewModel = new KasirIndexVm
            {
                PageIndex = page,
                TotalPages = (int)Math.Ceiling(count / (double)PageSize),
                ProdukItems = items
            };
            return PartialView("_ProductList", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            var (count, paginatedProducts) = await _kasirRepo.LoadPageAsync(pageNumber, PageSize);
            var items = paginatedProducts.Cast<Produk>().Select(p =>
            {
                var defaultUnit = p.ProductUnits.OrderBy(u => u.HargaJual).FirstOrDefault();
                var vm = new TransaksiItemVm
                {
                    IdProduk = p.IdProduk,
                    NamaProduk = p.NamaProduk,
                    SKU = p.SKU,
                    Stok=p.Stok,
                    AvailableUnits = p.ProductUnits.Select(u => new ProductUnitVm
                    {
                        ProductUnitId = u.ProductUnitId,
                        UnitName = u.UnitName,
                        HargaJual = u.HargaJual,
                        Conversion = u.ConversionValue
                    }).ToList(),
                    ProductUnitId = defaultUnit?.ProductUnitId ?? 0,
                    HargaJual = defaultUnit?.HargaJual ?? 0,
                    Gambar = p.ProdukGambars.FirstOrDefault()?.PathGambar ?? "/images/no-image.png"
                };
                return vm;
            }).ToList();
            var viewModel = new KasirIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)PageSize),
                ProdukItems = items
            };
            return PartialView("_ProductList", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchString, int pageNumber = 1)
        {
            var userId = _userManager.GetUserId(User);
            var karyawan = await _kasirRepo.GetKaryawanByUserIdAsync(userId) as Karyawan;
            if (karyawan == null)
            {
                return BadRequest(new { message = "Karyawan untuk user ini tidak ditemukan." });
            }
            // ================= PENCEGAHAN SHIFT AKTIF =================
            var activeShift = await _shiftsRepo.GetActiveShiftAsync(karyawan.IdKaryawan);
            if (activeShift == null)
            {
                // Lempar pesan error ke TempData agar muncul alert merah di halaman shift
                TempData["Error"] = "Anda harus Buka Laci / Mulai Shift terlebih dahulu sebelum melayani transaksi penjualan!";
                // Paksa kasir diarahkan ke halaman ShiftsController dan membuka laci kasir
                return RedirectToAction("Index", "Shifts");
            }
            // Jika implementasi PaginatedList membutuhkan IQueryable, 
            // disarankan logic pagination dilempar ke repository.
            // Di sini diasumsikan Anda melakukan load produk dasar dari repo
            var (count, baseProducts) = await _kasirRepo.LoadPageAsync(pageNumber, PageSize);
            var produkItems = baseProducts.Cast<Produk>().Select(t =>
            {
                var defaultUnit = t.ProductUnits.OrderBy(pu => pu.HargaJual).FirstOrDefault();
                return new TransaksiItemVm
                {
                    IdProduk = t.IdProduk,
                    NamaProduk = t.NamaProduk,
                    Stok=t.Stok,
                    ProductUnitId = defaultUnit?.ProductUnitId ?? 0,
                    HargaJual = defaultUnit?.HargaJual ?? 0,                    
                    Gambar = t.ProdukGambars.Select(pg => pg.PathGambar).FirstOrDefault() ?? "/images/no-image.png",
                    AvailableUnits = t.ProductUnits.Select(pu => new ProductUnitVm
                    {
                        ProductUnitId = pu.ProductUnitId,
                        UnitName = pu.UnitName,
                        HargaJual = pu.HargaJual,
                        Conversion =pu.ConversionValue
                    }).ToList()
                };
            }).ToList();
            var viewModel = new KasirIndexVm
            {
                ProdukItems = produkItems,
                CariProduk = searchString,
                ScanBarcode = string.Empty,
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)PageSize)
            };
            ViewBag.PelangganList = new SelectList(await _kasirRepo.GetPelangganListAsync(), "Id", "Nama");
            ViewBag.MetodeBayarList = new SelectList(await _kasirRepo.GetMetodeBayarListAsync(), "IdMetodeBayar", "NamaMetodeBayar");
            ViewBag.ProdukUnits = new SelectList(await _kasirRepo.GetProductUnitsDropdownAsync(), "ProductUnitId", "UnitName");
            foreach (var p in produkItems)
            {
                Console.WriteLine($"========== {p.NamaProduk} ==========");

                foreach (var u in p.AvailableUnits)
                {
                    Console.WriteLine(
                        $"Unit={u.UnitName}, Harga={u.HargaJual}, Conversion={u.Conversion}");
                }
            }
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([FromBody] TransaksiVm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { message = "Validasi gagal: " + string.Join("; ", errors) });
            }
            var userId = _userManager.GetUserId(User);
            var karyawan = await _kasirRepo.GetKaryawanByUserIdAsync(userId) as Karyawan;
            if (karyawan == null)
            {
                return BadRequest(new { message = "Karyawan untuk user ini tidak ditemukan." });
            }
            var activeShift = await _shiftsRepo.GetActiveShiftAsync(karyawan.IdKaryawan);
            if (activeShift == null)
            {
                return BadRequest(new { message = "Tidak ada shift aktif! Silakan buka shift terlebih dahulu." });
            }
            // Deklarasi transaksi di dalam scope 'using' memastikan database bersih dari deadlock jika keluar prematur
            using var transaction = await _kasirRepo.BeginTransactionAsync();
            try
            {
                var waktuSekarang = DateTime.Now;
                var transaksi = new Transaksi
                {
                    IdPelanggan = model.IdPelanggan,
                    IdKaryawan = karyawan.IdKaryawan,
                    IdShift = activeShift.IdShift,
                    TotalBelanja = model.TotalBelanja,
                    IdMetodeBayar = model.IdMetodeBayar,
                    Status = "Berhasil",
                    NominalBayar = model.NominalBayar,
                    Kembalian = model.NominalBayar - model.TotalBelanja,
                    NoTransaksi = "TRX-" + waktuSekarang.ToString("yyyyMMddHHmmss"),
                    TanggalTransaksi = waktuSekarang
                };
                await _kasirRepo.AddTransaksiAsync(transaksi);
                foreach (var item in model.Items)
                {
                    //Ambil detail unit untuk mendapatkan harga modal & memastikan unit valid
                    var productUnit = await _kasirRepo.GetProductUnitByIdAsync(item.ProductUnitId);
                    if (productUnit == null)
                    {
                        return BadRequest(new { message = $"Satuan Produk (ID: {item.ProductUnitId}) tidak ditemukan." });
                    }
                    //AMBIL STOK ASLI LANGSUNG DARI TABEL PRODUK UTAMA
                    var produkUtama = await _kasirRepo.GetProdukByIdAsync(item.IdProduk);
                    if (produkUtama == null)
                    {
                        return BadRequest(new { message = $"Produk dengan ID {item.IdProduk} tidak ditemukan di database." });
                    }
                    //Hitung kebutuhan stok riil dalam basis satuan terkecil (Pcs)
                    // Contoh: Beli 1 Dus dengan konversi 40 -> Kebutuhan = 1 * 40 = 40 Pcs
                    var kebutuhanStokRiil = item.Qty * (item.Conversion > 0 ? item.Conversion : 1);

                    //VALIDASI: Bandingkan kebutuhan riil dengan stok asli di tabel produk
                    if (produkUtama.Stok < kebutuhanStokRiil)
                    {
                        return BadRequest(new { message = $"Stok untuk unit produk tidak mencukupi. (Dibutuhkan: {kebutuhanStokRiil} Pcs, Tersedia: {produkUtama.Stok} Pcs)" });
                    }
                    var detail = new TransaksiDetail
                    {
                        Transaksi = transaksi,
                        IdProduk = item.IdProduk,
                        ProductUnitId = productUnit.ProductUnitId,
                        Qty = item.Qty,
                        HargaJual = item.HargaJual,
                        HargaModal = productUnit.HargaModal,                                                
                        Conversion=item.Conversion
                    };
                    await _kasirRepo.AddTransaksiDetailAsync(detail);
                    produkUtama.Stok -= kebutuhanStokRiil;
                    _kasirRepo.UpdateProduk(produkUtama);
                }
                // Simpan semua data sekaligus ke database fisik
                await _kasirRepo.SaveChangesAsync();
                await transaction.CommitAsync();
                // Gunakan waktuSekarang karena model.TanggalTransaksi kosong dari kiriman client-side
                _dashboard.ClearDashboardCache(waktuSekarang, waktuSekarang);
                return Json(new
                {
                    message = "Transaksi berhasil disimpan",
                    noTransaksi = transaksi.NoTransaksi,
                    idTransaksi = transaksi.IdTransaksi
                });
            }
            catch (Exception ex)
            {
                // Rollback hanya dieksekusi jika terjadi kegagalan system/I/O error yang sesungguhnya
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Gagal menyimpan transaksi. UserId: {UserId}", userId);
                return BadRequest(new { message = $"Terjadi error di server: {ex.Message}" });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> PrintStruk(int id)
        {
            var transaksi = await _kasirRepo.GetTransaksiForPrintAsync(id);
            if (transaksi == null) return NotFound();
            ViewBag.TokoSettings = await _kasirRepo.GetPengaturanTokoAsync();
            return View("PrintStruk", transaksi);
        }
    }
}