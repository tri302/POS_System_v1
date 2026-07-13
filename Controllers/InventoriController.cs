using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
using POS_System.Repository;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class InventoriController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IInventoriRepo _repo;
        public InventoriController(AppDbContext context, IInventoriRepo repo)
        {
            _context = context;
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> GetProductByBarcode(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
                return NotFound();
            barcode = barcode.Trim();
            var product = await _repo.GetProductByBarcodeAsync(barcode);
            if (product == null)
                return NotFound();
            return Json(new
            {
                idProduk = product.IdProduk,
                namaProduk = product.NamaProduk,
                stok = product.Stok,
                availableUnits = product.ProductUnits.Select(u => new
                {
                    productUnitId = u.ProductUnitId,
                    unitName = u.UnitName,
                    hargaModal = u.HargaModal
                }).ToList()
            });
        }
        [HttpGet]
        public async Task<IActionResult> GenerateInventoriExcel(DateTime startDate, DateTime endDate, int tipeStok)
        {
            try
            {
                // Tentukan label berdasarkan tipe stok
                string tipeLabel = tipeStok switch
                {
                    1 => "Masuk",
                    2 => "Keluar",
                    3 => "Penyesuaian",
                    _ => "Unknown"
                };
                // Ambil data dari repository
                var dataLaporan = await _repo.GetLaporanInventoriAsync(startDate, endDate, tipeStok.ToString(), tipeLabel);
                // Jika data kosong, kembalikan JSON agar bisa ditangkap oleh JavaScript (SweetAlert)
                if (!dataLaporan.Any())
                {
                    return Json(new { success = false, message = "Tidak ada data inventori ditemukan untuk kriteria yang dipilih." });
                }
                // Gunakan byte array untuk menyimpan file, agar tidak terjadi masalah stream di hosting
                byte[] fileContents;
                using (var workbook = new XLWorkbook())
                {
                    string sheetName = $"Laporan Stok {tipeLabel}";
                    var worksheet = workbook.Worksheets.Add(sheetName);
                    int currentRow = 1;
                    // Header Laporan
                    worksheet.Cell(currentRow, 1).Value = $"Laporan Stok: {tipeLabel}";
                    worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
                    worksheet.Range(currentRow, 1, currentRow, 9).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = $"Periode: {startDate:dd-MM-yyyy} - {endDate:dd-MM-yyyy}";
                    worksheet.Cell(currentRow, 1).Style.Font.Italic = true;
                    worksheet.Range(currentRow, 1, currentRow, 9).Merge().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    // Judul Kolom
                    currentRow += 2;
                    string[] headers = { "No. Ref", "Tanggal", "Tipe Stok", "Produk", "Satuan", "Jumlah", "Harga Modal", "Total Modal", "Sumber/Catatan" };
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cell(currentRow, i + 1).Value = headers[i];
                    }
                    var headerRange = worksheet.Range(currentRow, 1, currentRow, 9);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#ffc107");
                    // Isi Data
                    foreach (var item in dataLaporan)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = item.NoReferensi;
                        worksheet.Cell(currentRow, 2).Value = item.Tanggal.ToString("dd-MM-yyyy HH:mm");
                        worksheet.Cell(currentRow, 3).Value = item.TipeStok;
                        worksheet.Cell(currentRow, 4).Value = item.NamaProduk;
                        worksheet.Cell(currentRow, 5).Value = item.SatuanUnit;
                        worksheet.Cell(currentRow, 6).Value = item.JumlahProduk;
                        worksheet.Cell(currentRow, 7).Value = item.HargaModal;
                        worksheet.Cell(currentRow, 8).Value = item.TotalModal;
                        worksheet.Cell(currentRow, 9).Value = string.IsNullOrEmpty(item.Catatan) ? item.Sumber : item.Catatan;
                        // Format Angka
                        worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "#,##0.00";
                        worksheet.Cell(currentRow, 8).Style.NumberFormat.Format = "#,##0.00";
                    }
                    worksheet.Columns().AdjustToContents();
                    // Simpan workbook ke dalam MemoryStream dan konversi ke byte array
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        fileContents = stream.ToArray();
                    }
                }
                // Nama file dinamis
                string fileName = $"Laporan_Inventori_{tipeLabel}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                // Kirim file ke client
                return File(
                    fileContents,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error System: " + ex.Message + " | StackTrace: " + ex.StackTrace);
            }
        }
       
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var (paginated, totalCount) = await _repo.GetPaginatedInventoriAsync(pageNumber, pageSize);
            // Pastikan list ini tidak null, jika null, buat list kosong agar tidak error di LINQ
            var tipeStokList = await _repo.GetAllTipeStokAsync() ?? new List<TipeStok>();
            var sumberStokList = await _repo.GetAllSumberStokAsync() ?? new List<SumberStok>();
            // Gunakan pengecekan null yang ketat
            var grouped = paginated?.GroupBy(x => x.NoReferensi)
                .Select(g =>
                {
                    var first = g.FirstOrDefault();
                    if (first == null) return null;
                    try
                    {
                        return new InventoriEditVm
                        {
                            IdInventori = first.IdInventori,
                            NoReferensi = first.NoReferensi ?? "-",
                            // Pastikan pengecekan null di sini ketat
                            TipeStok = (tipeStokList?.FirstOrDefault(t => t.Id.ToString() == (first.TipeStok ?? ""))?.Nama) ?? first.TipeStok ?? "-",
                            Sumber = (sumberStokList?.FirstOrDefault(s => s.Id.ToString() == (first.Sumber ?? ""))?.Nama) ?? first.Sumber ?? "-",
                            Notes = first.Notes ?? "",
                            Jumlah = g.Sum(i => i?.Jumlah ?? 0),
                            TotalModal = g.Sum(i => i?.TotalModal ?? 0)
                        };
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                })
                .ToList() ?? new List<InventoriEditVm>();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var viewModel = new InventoriIndexVm
            {
                InventoriList = grouped,
                TotalPages = totalPages,
                PageIndex = pageNumber,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var (paginated, totalCount) = await _repo.GetPaginatedInventoriAsync(pageNumber, pageSize);
            var tipeStokList = await _repo.GetAllTipeStokAsync();
            var sumberStokList = await _repo.GetAllSumberStokAsync();
            var items = paginated.GroupBy(x => x.NoReferensi).Select(g =>
            {
                var first = g.First();
                return new InventoriEditVm
                {
                    IdInventori = first.IdInventori,
                    NoReferensi = first.NoReferensi,
                    TipeStok = tipeStokList.FirstOrDefault(t => t.Id.ToString() == first.TipeStok)?.Nama ?? first.TipeStok ?? "-",
                    Sumber = sumberStokList.FirstOrDefault(s => s.Id.ToString() == first.Sumber)?.Nama ?? first.Sumber ?? "-",
                    Notes = first.Notes,
                    Jumlah = g.Sum(i => i.Jumlah),
                    TotalModal = g.Sum(i => i.TotalModal)
                };
            }).ToList();
            var viewModel = new InventoriIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                InventoriList = items
            };
            return View("Index", viewModel);
        }        
        [HttpGet]
        public IActionResult Create() => View(new InventoriVm());
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoriVm model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _repo.BeginTransactionAsync();
                if (string.IsNullOrWhiteSpace(model.NoReferensi))
                {
                    string prefix = model.TipeStok switch { "2" => "KEL", "3" => "PEN", _ => "INV" };
                    var today = DateTime.Now.ToString("yyyyMMdd");
                    var countToday = await _repo.GetCountNoReferensiTodayAsync(prefix, today);
                    model.NoReferensi = $"{prefix}-{today}-{(countToday + 1):D2}";
                }
                foreach (var item in model.Items)
                {
                    var prod = await _repo.GetProdukWithUnitsAsync(item.IdProduk);
                    if (prod == null) return BadRequest($"Produk dengan Id {item.IdProduk} tidak ditemukan.");
                    var productUnitToUpdate = await _repo.GetProductUnitByIdAsync(item.ProductUnitId);
                    if (productUnitToUpdate == null) return BadRequest($"Unit Produk dengan ID {item.ProductUnitId} tidak ditemukan untuk Produk ini.");
                    // Tentukan jumlah riil sebelum disimpan ke history
                    int finalJumlah = item.Jumlah;
                    if (model.TipeStok == "2") // Jika tipe keluar
                    {
                        finalJumlah = -Math.Abs(item.Jumlah); // Paksa jadi minus
                    }
                    else if (model.TipeStok == "1") // Jika tipe masuk
                    {
                        finalJumlah = Math.Abs(item.Jumlah); // Paksa jadi positif
                    }
                    var entity = new Inventori
                    {
                        NoReferensi = model.NoReferensi,
                        TipeStok = model.TipeStok,
                        Sumber = model.Sumber,
                        Notes = model.Notes,
                        HargaModal = item.HargaModal,
                        Jumlah = finalJumlah, // Menggunakan nilai yang sudah disesuaikan tanda +/- nya
                        TotalModal = item.HargaModal * finalJumlah, // Total modal akan ikut minus jika stok keluar
                        IdProduk = item.IdProduk,
                        Tanggal = DateTime.Now,
                        ProductUnitId = productUnitToUpdate.ProductUnitId
                    };
                    //Simpan history transaksi inventori
                    await _repo.AddInventoriAsync(entity);
                    //Cukup panggil helper ini untuk memanipulasi stok produk di database                    
                    await ApplyNewStockCreate(entity);
                }
                await _repo.SaveChangesAsync();
                await _repo.CommitTransactionAsync();
                return Ok(new { success = true, noReferensi = model.NoReferensi });
            }
            catch (Exception ex)
            {
                await _repo.RollbackTransactionAsync();
                return StatusCode(500, new { message = "Terjadi kesalahan saat menyimpan inventori." });
            }
        }
        [HttpGet]
        [Route("Inventori/GetSumberByTipe/{tipeStokId}")]
        public async Task<IActionResult> GetSumberByTipe(int tipeStokId) => Json(await _repo.GetSumberByTipeAsync(tipeStokId));
        [HttpGet]
        public async Task<IActionResult> GetProdukDetail(int Id)
        {
            var produk = await _repo.GetProdukDetailJsonAsync(Id);
            return produk == null ? NotFound() : Json(produk);
        }
        [HttpGet]
        public async Task<IActionResult> SearchProduk(string term) => Json(await _repo.SearchProdukAsync(term));
        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var GetById = await _repo.GetInventoriByIdAsync(id);
            if (GetById == null) return NotFound();
            var ListInventori = await _repo.GetInventoriByNoReferensiAsync(GetById.NoReferensi);
            var allProduk = await _repo.GetAllProdukVmAsync();
            var VmList = ListInventori.Select(i =>
            {
                var produk = allProduk.FirstOrDefault(p => p.IdProduk == i.IdProduk);
                return new InventoriVm
                {
                    IdProduk = i.IdProduk,
                    ProductUnitId = i.ProductUnitId,
                    Stok = produk != null ? produk.Stok : 0,
                    NamaProduk = produk?.NamaProduk,
                    UnitName = i.ProductUnit?.UnitName,
                    IdInventori = i.IdInventori,
                    NoReferensi = i.NoReferensi,
                    TipeStok = i.TipeStok,
                    Sumber = i.Sumber,
                    Notes = i.Notes,
                    HargaModal = i.HargaModal,
                    Jumlah = i.Jumlah,
                    TotalModal = i.TotalModal
                };
            }).ToList();
            var model = new InventoriEditVm
            {
                NoReferensi = GetById.NoReferensi,
                TipeStok = GetById.TipeStok,
                Sumber = GetById.Sumber,
                Notes = GetById.Notes,
                TotalModal = VmList.Sum(x => x.HargaModal * x.Jumlah),
                Items = VmList,
                ProdukList = allProduk
            };
            ViewData["ProdukList"] = new SelectList(allProduk, "IdProduk", "NamaProduk", model.Items.FirstOrDefault()?.IdProduk);
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            // Cari 1 inventori dulu
            var GetById = await _context.Inventoris
                .FirstOrDefaultAsync(x => x.IdInventori == id);
            if (GetById == null)
            {
                return NotFound();
            }
            // 1. Perbaikan pada ProdukList: Hapus klausa .Where yang menyebabkan gagal translasi SQL
            var produkList = await _context.Produks
                .Select(p => new ProdukVm
                {
                    IdProduk = p.IdProduk,
                    NamaProduk = p.NamaProduk,
                    // Stok diakses dari ProductUnit PERTAMA (asumsi unit dasar)
                    Stok = p.ProductUnits
                               .OrderBy(pu => pu.ProductUnitId)
                               .Select(pu => (int?)pu.Stok) // Proyeksi ke int? agar null bisa dihandle
                               .FirstOrDefault() ?? 0
                })
                .ToListAsync();
            // 2. Ambil semua detail berdasarkan NoReferensi yg sama
            var ListInventori = await _context.Inventoris
                .Where(x => x.NoReferensi == GetById.NoReferensi)
                .Include(x => x.Produk)
                .Include(x => x.ProductUnit)
                .ToListAsync();
            // 3. Mapping ke ViewModel List
            var VmList = ListInventori.Select(i => new InventoriVm
            {
                IdInventori = i.IdInventori,
                NoReferensi = i.NoReferensi,
                TipeStok = i.TipeStok,
                Stok = i.Produk.Stok,
                KodeBarcode = i.Produk.KodeBarcode,
                Sumber = i.Sumber,
                Notes = i.Notes,
                HargaModal = i.HargaModal,
                Jumlah = i.Jumlah,
                TotalModal = i.TotalModal,
                IdProduk = i.IdProduk,
                NamaProduk = i.Produk?.NamaProduk,
                ProductUnitId = i.ProductUnitId,
                UnitName = i.ProductUnit?.UnitName
            }).ToList();
            // Bungkus dalam composite model agar bisa pegang header + list
            var model = new InventoriEditVm
            {
                NoReferensi = GetById.NoReferensi,
                TipeStok = GetById.TipeStok,
                Sumber = GetById.Sumber,
                Notes = GetById.Notes,
                TotalModal = VmList.Sum(x => (x.HargaModal * x.Jumlah)),
                Items = VmList,
                ProdukList = produkList
            };
            // ViewData untuk Dropdown Produk List
            ViewData["ProdukList"] = new SelectList(
                produkList,
                "IdProduk",
                "NamaProduk",
                model.Items.FirstOrDefault()?.IdProduk
            );
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] InventoriEditVm model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                await _repo.BeginTransactionAsync();
                foreach (var item in model.Items)
                {
                    var prod = await _repo.GetProdukWithUnitsAsync(item.IdProduk);
                    if (prod == null) return BadRequest($"Produk tidak ditemukan.");
                    var productUnitToUpdate = await _repo.GetProductUnitByIdAsync(item.ProductUnitId);
                    if (productUnitToUpdate == null) return BadRequest($"Unit Produk tidak ditemukan.");
                    // 1. CARI DATA LAMA DARI DATABASE UNTUK MENGHITUNG DELTA
                    // (Asumsi repo Anda memiliki method untuk mengambil data inventori berdasarkan ID)
                    var inventoriLama = await _context.Inventoris.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.IdInventori == item.IdInventori);
                    int jumlahLama = inventoriLama != null ? inventoriLama.Jumlah : 0;
                    var entity = new Inventori
                    {
                        IdInventori = item.IdInventori,
                        NoReferensi = model.NoReferensi,
                        TipeStok = model.TipeStok,
                        Sumber = model.Sumber,
                        Notes = model.Notes,
                        HargaModal = item.HargaModal,
                        Jumlah = item.Jumlah, // Nilai baru dari frontend
                        TotalModal = item.HargaModal * item.Jumlah,
                        IdProduk = item.IdProduk,
                        Tanggal = DateTime.Now,
                        ProductUnitId = productUnitToUpdate.ProductUnitId
                    };
                    _repo.UpdateInventoriEdit(entity);
                    // 2. KIRIM JUMLAH LAMA KE METHOD HELPER
                    await ApplyNewStockEdit(entity, jumlahLama);
                }
                await _repo.SaveChangesAsync();
                await _repo.CommitTransactionAsync();
                return Ok(new { success = true, noReferensi = model.NoReferensi });
            }
            catch (Exception ex)
            {
                await _repo.RollbackTransactionAsync();
                return StatusCode(500, new { message = ex.Message });
            }
        }
        // 3. HITUNG DELTA DI SINI
        private async Task ApplyNewStockEdit(Inventori item, int jumlahLama)
        {
            var produk = await _repo.GetProdukByIdAsync(item.IdProduk);
            if (produk == null) return;
            // DELTA = Jumlah Baru - Jumlah Lama
            int delta = item.Jumlah - jumlahLama;
            // Tambahkan delta ke stok global (bisa bernilai positif atau negatif)
            produk.Stok += delta;
            _repo.UpdateProducts(produk);
        }
        private async Task ApplyNewStockCreate(Inventori entity)
        {
            //Ambil data stok produk/unit saat ini dari database
            var productUnit = await _repo.GetProdukByIdAsync(entity.IdProduk);
            if (productUnit == null) throw new Exception("Unit produk tidak ditemukan.");
            //Tentukan nilai pengubah stok berdasarkan TipeStok
            int perubahanStok = entity.Jumlah;
            switch (entity.TipeStok)
            {
                case "1": // Stok Masuk
                          // Pastikan nilainya positif
                    perubahanStok = Math.Abs(entity.Jumlah);
                    break;
                case "2": // Stok Keluar
                          // Pastikan nilainya negatif untuk mengurangi stok
                    perubahanStok = -Math.Abs(entity.Jumlah);
                    break;
                case "3": // Penyesuaian (Opname)
                          // Biarkan apa adanya karena frontend sudah mengirim + atau - sesuai pilihan user
                    perubahanStok = entity.Jumlah;
                    break;
            }
            //Update nilai stok di database (misal nama kolomnya adalah 'Stok')
            productUnit.Stok += perubahanStok;
            //Update data unit produk tersebut
            _repo.UpdateProducts(productUnit);
        }
    }
}