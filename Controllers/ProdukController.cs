using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS_System.Models;
using POS_System.Pageing;
using POS_System.Repository;
using POS_System.ViewModels;
using System.Net;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class ProdukController : Controller
    {
        private readonly IProdukRepo _repo;
        public ProdukController(IProdukRepo repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> CekBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                return Json(new { exists = false });
            }            
            bool isExist = await _repo.IsBarcodeSudahAda(barcode);
            return Json(new { exists = isExist });
        }
        [HttpGet]
        public async Task<IActionResult> ExportToExcel(string filter = "all")
        {
            // 1. Ambil Data dari Repository
            var dataLaporan = await _repo.GetLaporanStokAsync(filter);
            // 2. Buat Workbook Excel dengan ClosedXML
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Laporan Stok Produk");
                int currentRow = 1;
                // Judul Laporan
                string filterTitle = filter?.ToLower() switch
                {
                    "banyak" => "STOK BANYAK (>= 50)",
                    "sedikit" => "STOK SEDIKIT (1-5)",
                    "habis" => "STOK HABIS (0)",
                    _ => "SEMUA STOK"
                };
                worksheet.Cell(currentRow, 1).Value = $"LAPORAN STOK PRODUK - FILTER: {filterTitle}";
                worksheet.Range(currentRow, 1, currentRow, 9).Merge().Style.Font.Bold = true;
                worksheet.Range(currentRow, 1, currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Range(currentRow, 1, currentRow, 9).Style.Fill.BackgroundColor = XLColor.LightBlue;
                currentRow += 2;
                // Header Tabel
                string[] headers = { "ID", "Nama Produk", "Kode Barcode", "SKU", "Satuan Unit", "Stok", "Harga Modal", "Harga Jual", "Status Produk" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cell(currentRow, i + 1).Value = headers[i];
                }
                // Styling Header (DISESUAIKAN: Range sampai kolom 9)
                var headerRange = worksheet.Range(currentRow, 1, currentRow, 9);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.Yellow;
                headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                currentRow++;
                // Data Baris (Diurutkan pas dari kolom 1 sampai 9)
                // Pastikan Anda menambahkan namespace ini di bagian paling atas file controller jika belum ada:
                // using System.Net;
                // Data Baris
                foreach (var item in dataLaporan)
                {
                    worksheet.Cell(currentRow, 1).Value = item.IdProduk;
                    //Lakukan HtmlDecode agar teks "&amp;" kembali menjadi "&" biasa
                    string namaProdukClean = WebUtility.HtmlDecode(item.NamaProduk);
                    worksheet.Cell(currentRow, 2).Value = namaProdukClean;
                    worksheet.Cell(currentRow, 3).Value = item.KodeBarcode;
                    worksheet.Cell(currentRow, 4).Value = item.SKU;
                    worksheet.Cell(currentRow, 5).Value = item.NamaSatuan;
                    worksheet.Cell(currentRow, 6).Value = item.Stok;
                    worksheet.Cell(currentRow, 7).Value = item.HargaModal;
                    worksheet.Cell(currentRow, 8).Value = item.HargaJual;
                    worksheet.Cell(currentRow, 9).Value = item.StatusProduk;
                    // Format Rupiah
                    worksheet.Cell(currentRow, 7).Style.NumberFormat.Format = "Rp #,##0.00";
                    worksheet.Cell(currentRow, 8).Style.NumberFormat.Format = "Rp #,##0.00";
                    // Highlight stok habis
                    if (item.Stok == 0)
                    {
                        var rowRange = worksheet.Range(currentRow, 1, currentRow, 9);
                        rowRange.Style.Fill.BackgroundColor = XLColor.Red;
                        rowRange.Style.Font.FontColor = XLColor.White;
                    }
                    currentRow++;
                }
                // Auto Fit Columns
                worksheet.Columns().AdjustToContents();
                // 3. Kembalikan File Excel
                using (var content = new MemoryStream())
                {
                    workbook.SaveAs(content);
                    var fileBytes = content.ToArray();
                    string fileName = $"Laporan_Stok_Produk_{filterTitle.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> FilterProduk(string filter, string searchTerm, int pageNumber = 1)
        {
            int pageSize = 5;
            // controller tinggal memanggil satu fungsi sakti dari repository ini:
            var query = _repo.GetFilteredProduksQuery(filter, searchTerm);
            var paginatedData = await PaginatedList<Produk>.CreateAsync(query, pageNumber, pageSize);
            var produkVmList = MapToProdukVmList(paginatedData);
            var produkIndexVm = new ProdukIndexVm
            {
                ProdukList = produkVmList,
                TotalPages = paginatedData.TotalPages,
                PageIndex = paginatedData.PageIndex,
                HasPreviousPage = paginatedData.HasPreviousPage,
                HasNextPage = paginatedData.HasNextPage
            };
            // Kembalikan Partial View untuk memperbarui tabel AJAX di produk.js
            return PartialView("_ProdukTablePartial", produkIndexVm);
        }
        [HttpPost]
        public async Task<IActionResult> GenerateBarcode(string mode, List<long> productIds, string filter, string searchTerm)
        {
            // Ambil base query dari repo
            var query = _repo.GetProduksWithFiltersQuery().Include(p => p.ProductUnits).AsQueryable();
            // Filter berdasarkan mode
            if (mode == "selected" && productIds != null && productIds.Any())
            {
                query = query.Where(p => productIds.Contains(p.IdProduk));
            }
            else if (mode == "filter")
            {
                // 2. Pastikan GetFilteredData menerima parameter filter & searchTerm yang benar
                query = GetFilteredData(query, filter, searchTerm);
            }
            else
            {
                query = query.Take(100);
            }
            var rawData = await query.ToListAsync();
            // Proteksi jika data kosong
            if (rawData == null || !rawData.Any())
            {
                return Content("Tidak ada data produk untuk dicetak.");
            }
            var dataToPrint = rawData.Select(p =>
            {
                var unitUtama = p.ProductUnits?.OrderBy(pu => pu.ProductUnitId).FirstOrDefault();
                // Proteksi jika KodeBarcode null/kosong agar library NetBarcode tidak crash
                string kode = string.IsNullOrEmpty(p.KodeBarcode) ? "000000" : p.KodeBarcode;
                var barcode = new NetBarcode.Barcode(kode, NetBarcode.Type.Code128, true, 400, 100);
                return new BarcodeDisplayVm
                {
                    NamaProduk = p.NamaProduk,
                    KodeBarcode = kode,
                    HargaJual = unitUtama != null ? unitUtama.HargaJual : 0,
                    BarcodeBase64 = barcode.GetBase64Image()
                };
            }).ToList();
            return View("PrintLayout", dataToPrint);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Ambil data produk beserta daftar unit/satuannya
            var produk = await _repo.GetByIdAsync(id);
            if (produk == null)
            {
                return Json(new { success = false, message = "Produk tidak ditemukan." });
            }
            try
            {
                //Loop semua unit yang terikat dengan produk ini dan ubah IsDeleted menjadi true
                if (produk.ProductUnits != null && produk.ProductUnits.Any())
                {
                    foreach (var unit in produk.ProductUnits)
                    {
                        unit.IsDeleted = true;
                        _repo.UpdateUnit(unit);
                    }
                }
                await _repo.SaveChangesAsync();
                return Json(new { success = true, message = "Produk dan unit berhasil dinonaktifkan." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gagal memproses permintaan: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _repo.GetProduksQuery()
                .OrderByDescending(p => p.IdProduk);
            var paginatedData = await PaginatedList<Produk>.CreateAsync(query, pageNumber, pageSize);
            var produkVmList = MapToProdukVmList(paginatedData);
            var produkIndexVm = new ProdukIndexVm
            {
                ProdukList = produkVmList,
                TotalPages = paginatedData.TotalPages,
                PageIndex = paginatedData.PageIndex,
                HasPreviousPage = paginatedData.HasPreviousPage,
                HasNextPage = paginatedData.HasNextPage
            };
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProdukTablePartial", produkIndexVm);
            }
            return View(produkIndexVm);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _repo.GetProduksQuery()
                .AsNoTracking()
                .Where(p => p.StatusProduk == "Aktif")
                .OrderBy(p => p.IdProduk);
            int count = await query.CountAsync();
            var paginated = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var items = MapToProdukVmList(paginated);
            var viewModel = new ProdukIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                ProdukList = items
            };
            return PartialView("~/Views/Produk/_ProdukTablePartial.cshtml", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var kategoris = await _repo.GetKategoriListAsync();
            var model = new ProdukVm
            {
                KategoriList = new SelectList(kategoris, "IdKategori", "NamaKategori")
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdukVm model)
        {
            if (model.UploadGambar == null)
            {
                ModelState.AddModelError(nameof(model.UploadGambar), "Wajib upload gambar saat create produk");
            }
            // ==========================================================
            // 💡 VALIDASI UNTUK CEK DUPLIKASI NAMA UNIT (VERSI TUPLE)
            // ==========================================================
            var (isDuplicate, duplicateNames) = CheckDuplicateUnits(model);
            if (!ModelState.IsValid || isDuplicate)
            {
                if (isDuplicate)
                {
                    TempData["SweetAlertMessage"] = $"Satuan Unit berikut dobel: {duplicateNames}. Harap perbaiki input Anda.";
                    TempData["SweetAlertType"] = "warning";
                }
                var kategoris = await _repo.GetKategoriListAsync();
                model.KategoriList = new SelectList(kategoris, "IdKategori", "NamaKategori");
                return View(model);
            }
            // Logic upload gambar (asumsi method ini ada di controller Anda)
            var (flowControl, value, fotoPath) = await UploadGambar(model);
            if (!flowControl) return value!;
            var produkBaru = new Produk
            {
                IdKategori = model.IdKategori,
                NamaProduk = model.NamaProduk,
                Deskripsi = model.Deskripsi,
                KodeBarcode = model.KodeBarcode,
                SKU = model.SKU,
                Stok = model.Stok,
                StatusProduk = model.StatusProduk
            };
            await _repo.AddAsync(produkBaru);
            await _repo.SaveChangesAsync(); // Commit untuk dapat ID Produk
            if (model.ProductUnits != null && model.ProductUnits.Any())
            {
                foreach (var unitDto in model.ProductUnits)
                {
                    await _repo.AddUnitAsync(new ProductUnit
                    {
                        IdProduk = produkBaru.IdProduk,
                        UnitName = unitDto.UnitName,
                        ConversionValue = unitDto.ConversionValue,
                        Is_Default = unitDto.Is_Default,
                        //Stok = unitDto.Stok,
                        HargaModal = unitDto.HargaModal,
                        HargaJual = unitDto.HargaJual
                    });
                }
            }
            if (fotoPath != null)
            {
                await _repo.AddGambarAsync(new ProdukGambar
                {
                    IdProduk = produkBaru.IdProduk,
                    PathGambar = fotoPath
                });
            }
            await _repo.SaveChangesAsync();
            TempData["SuccessMessage"] = "Data berhasil disimpan!";
            TempData["SweetAlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var produkDb = await _repo.GetByIdAsync(id);
            if (produkDb == null) return NotFound();
            var model = new ProdukVm
            {
                IdProduk = produkDb.IdProduk,
                NamaProduk = produkDb.NamaProduk,
                Stok = produkDb.Stok,
                KodeBarcode = produkDb.KodeBarcode,
                SKU = produkDb.SKU,
                StatusProduk = produkDb.StatusProduk,
                Deskripsi = produkDb.Deskripsi,
                IdKategori = produkDb.IdKategori,
                FotoPath = produkDb.ProdukGambars?.OrderBy(pg => pg.IdProdukGambar).FirstOrDefault()?.PathGambar,
                ProductUnits = produkDb.ProductUnits?.Where(pu => !pu.IsDeleted).OrderBy(pu => pu.ProductUnitId).Select(pu => new ProductUnitDto
                {
                    ProductUnitId = pu.ProductUnitId,
                    UnitName = pu.UnitName,
                    ConversionValue = pu.ConversionValue,
                    Is_Default = pu.Is_Default,
                    //Stok = pu.Stok,
                    HargaModal = pu.HargaModal,
                    HargaJual = pu.HargaJual
                }).ToList()
            };
            var kategoris = await _repo.GetKategoriListAsync();
            model.KategoriList = new SelectList(kategoris, "IdKategori", "NamaKategori", model.IdKategori);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ProdukVm model)
        {
            ModelState.Remove(nameof(model.UploadGambar));
            var produkLama = await _repo.GetByIdAsync(id);
            if (produkLama == null) return NotFound();
            // ==========================================================
            // 💡 VALIDASI UNTUK CEK DUPLIKASI NAMA UNIT (VERSI TUPLE)
            // ==========================================================
            var (isDuplicate, duplicateNames) = CheckDuplicateUnits(model);
            if (!ModelState.IsValid || isDuplicate)
            {
                if (isDuplicate)
                {
                    TempData["SweetAlertMessage"] = $"Satuan Unit berikut dobel: {duplicateNames}. Harap perbaiki input Anda.";
                    TempData["SweetAlertType"] = "warning";
                }
                var kategoris = await _repo.GetKategoriListAsync();
                model.KategoriList = new SelectList(kategoris, "IdKategori", "NamaKategori", model.IdKategori);
                return View(model);
            }
            var (flowControl, value, fotoPath) = await UploadGambar(model);
            if (!flowControl) return value!;
            //Jika user mengunggah gambar baru saat edit
            if (fotoPath != null)
            {
                await _repo.AddGambarAsync(new ProdukGambar
                {
                    IdProduk = produkLama.IdProduk,
                    PathGambar = fotoPath
                });
            }
            // Update Header
            produkLama.NamaProduk = model.NamaProduk;
            produkLama.Stok = model.Stok;
            produkLama.Deskripsi = model.Deskripsi;
            produkLama.IdKategori = model.IdKategori;
            produkLama.KodeBarcode = model.KodeBarcode;
            produkLama.SKU = model.SKU;
            produkLama.StatusProduk = model.StatusProduk;
            // Sinkronisasi Units
            var unitLamaDb = produkLama.ProductUnits.ToList();
            var unitBaruView = model.ProductUnits ?? new List<ProductUnitDto>();
            // Soft Delete yang tidak ada di view
            var unitToSoftDelete = unitLamaDb
                .Where(oldUnit => !unitBaruView.Any(newUnit => newUnit.ProductUnitId == oldUnit.ProductUnitId && oldUnit.ProductUnitId != 0))
                .ToList();
            foreach (var unit in unitToSoftDelete)
            {
                unit.IsDeleted = true;
            }
            // Add atau Update unit produk
            foreach (var unitDto in unitBaruView)
            {
                if (unitDto.ProductUnitId == 0)
                {
                    await _repo.AddUnitAsync(new ProductUnit
                    {
                        IdProduk = produkLama.IdProduk,
                        UnitName = unitDto.UnitName,
                        ConversionValue = unitDto.ConversionValue,
                        Is_Default = unitDto.Is_Default,
                        //Stok = unitDto.Stok,
                        HargaModal = unitDto.HargaModal,
                        HargaJual = unitDto.HargaJual,
                        IsDeleted = false
                    });
                }
                else
                {
                    var existingUnit = unitLamaDb.FirstOrDefault(u => u.ProductUnitId == unitDto.ProductUnitId);
                    if (existingUnit != null)
                    {
                        existingUnit.UnitName = unitDto.UnitName;
                        existingUnit.ConversionValue = unitDto.ConversionValue;
                        existingUnit.Is_Default = unitDto.Is_Default;
                        //existingUnit.Stok = unitDto.Stok;
                        existingUnit.HargaModal = unitDto.HargaModal;
                        existingUnit.HargaJual = unitDto.HargaJual;
                    }
                }
            }
            _repo.UpdateProduk(produkLama);
            await _repo.SaveChangesAsync();
            TempData["SuccessMessage"] = "Data berhasil diubah!";
            TempData["SweetAlertType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        // --- HELPER METHODS UNTUK REFACTORING ---
        private List<ProdukVm> MapToProdukVmList(IEnumerable<Produk> produks)
        {
            return produks.Select(p => new ProdukVm
            {
                IdProduk = p.IdProduk,
                NamaProduk = p.NamaProduk ?? string.Empty,
                Stok = p.Stok,
                KodeBarcode = p.KodeBarcode ?? string.Empty,
                SKU = p.SKU ?? string.Empty,
                UnitName = p.ProductUnits
                .Where(x => x.Is_Default)
                .Select(x => x.UnitName)
                .FirstOrDefault(),
                StatusProduk = p.StatusProduk ?? string.Empty,
                FotoPath = p.ProdukGambars?.OrderBy(pg => pg.IdProdukGambar).Select(pg => pg.PathGambar).FirstOrDefault() ?? string.Empty,
                ProductUnits = p.ProductUnits?.Where(pu => !pu.IsDeleted).OrderBy(pu => pu.ProductUnitId).Select(pu => new ProductUnitDto
                {
                    ProductUnitId = pu.ProductUnitId,
                    UnitName=pu.UnitName,
                    ConversionValue = pu.ConversionValue,
                    Is_Default = pu.Is_Default,                    
                    HargaModal = pu.HargaModal,
                    HargaJual = pu.HargaJual
                }).ToList() ?? new List<ProductUnitDto>(),               
                HargaModal = p.ProductUnits?.OrderBy(pu => pu.ProductUnitId).Select(pu => pu.HargaModal).FirstOrDefault() ?? 0M,
                HargaJual = p.ProductUnits?.OrderBy(pu => pu.ProductUnitId).Select(pu => pu.HargaJual).FirstOrDefault() ?? 0M,
            }).ToList();
        }
        private (bool IsDuplicate, string DuplicateNames) CheckDuplicateUnits(ProdukVm model)
        {
            if (model.ProductUnits != null && model.ProductUnits.Count > 1)
            {
                var duplicateUnits = model.ProductUnits
                    .Where(u => !string.IsNullOrEmpty(u?.UnitName))
                    .Select(u => u.UnitName.Trim().ToLower())
                    .GroupBy(u => u)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key.ToUpper())
                    .ToList();
                if (duplicateUnits.Any())
                {
                    return (true, string.Join(", ", duplicateUnits));
                }
            }
            return (false, string.Empty);
        }
        private IQueryable<Produk> GetFilteredData(IQueryable<Produk> query, string filter, string searchTerm)
        {
            // Jika searchTerm kosong, langsung kembalikan query asal
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return query;
            }
            // Lakukan filter berdasarkan parameter 'filter' yang dipilih user
            if (filter == "nama")
            {
                query = query.Where(p => p.NamaProduk.Contains(searchTerm));
            }
            else if (filter == "kode")
            {
                query = query.Where(p => p.KodeBarcode.Contains(searchTerm));
            }
            else
            {
                // Default jika filter tidak spesifik (mencari di kedua kolom)
                query = query.Where(p => p.NamaProduk.Contains(searchTerm) || p.KodeBarcode.Contains(searchTerm));
            }
            return query;
        }
        private async Task<(bool flowControl, IActionResult? value, string? fotoPath)> UploadGambar(ProdukVm model)
        {
            // Jika tidak ada file yang diunggah
            if (model.UploadGambar == null || model.UploadGambar.Length == 0)
            {
                // Saat Create, UploadGambar wajib (sudah divalidasi di controller Anda)
                // Saat Edit, jika kosong berarti menggunakan gambar lama yang sudah ada di database
                return (true, null, null);
            }
            try
            {
                // Tentukan folder penyimpanan (misal: wwwroot/images/produk)
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "produk");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Buat nama file unik agar tidak bentrok
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.UploadGambar.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);
                // Simpan file ke server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadGambar.CopyToAsync(stream);
                }
                // Kembalikan path relatif untuk disimpan ke database (misal: /images/produk/namafile.jpg)
                string pathSimpanDb = $"/images/produk/{fileName}";
                return (true, null, pathSimpanDb);
            }
            catch (Exception ex)
            {
                // Jika gagal upload, isi TempData error dan hentikan flow di Controller
                TempData["SweetAlertMessage"] = $"Gagal mengunggah gambar: {ex.Message}";
                TempData["SweetAlertType"] = "error";
                // Mengembalikan flowControl = false agar controller mereturn View kembali
                var kategoris = await _repo.GetKategoriListAsync();
                model.KategoriList = new SelectList(kategoris, "IdKategori", "NamaKategori");
                return (false, new ViewResult { ViewName = model.IdProduk == 0 ? "Create" : "Edit", ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<ProdukVm>(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary()) { Model = model } }, null);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SoftDeleteUnit(int id)
        {
            var unit = await _repo.GetProdukUnitById(id);
            if (unit == null)
                return Json(new { success = false, message = "Data tidak ditemukan." });
            try
            {
                // ubah statusnya
                unit.IsDeleted = true;
                _repo.UpdateUnit(unit);
                return Json(new { success = true, message = "Unit berhasil dinonaktifkan." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gagal memproses permintaan." });
            }
        }
    }
}
