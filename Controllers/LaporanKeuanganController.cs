using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using POS_System.Models;
using POS_System.Repository;
using POS_System.ViewModels;
//Untuk generate pdf file free dan open source
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using Azure;
namespace POS_System.Controllers
{
    public class LaporanKeuanganController : Controller
    {
        private readonly ILaporanKeuanganRepo _repo;
        public LaporanKeuanganController(ILaporanKeuanganRepo repo)
        {
            _repo = repo;
        }
        [HttpPost]
        public async Task<IActionResult> DownloadLaporan(string tipelaporan, DateTime? tglMulai, DateTime? tglSelesai)
        {
            // Fix Bug 1: Validasi parameter tanggal wajib diisi
            if (!tglMulai.HasValue || !tglSelesai.HasValue)
            {
                return Json(new { success = false, message = "Parameter tanggal mulai dan tanggal selesai wajib diisi." });
            }
            // Pastikan tanggal selesai mencakup akhir hari (23:59:59) jika tglSelesai tidak memakai jam
            var mulai = tglMulai.Value.Date;
            var selesai = tglSelesai.Value.Date.AddDays(1).AddTicks(-1);
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmm");
            if (tipelaporan == "Penjualan")
            {
                // Performa Fix: Kirim parameter tanggal ke repository
                var penjualanFiltered = await _repo.GetLaporanPenjualanAsync(mulai, selesai);
                if (penjualanFiltered == null || !penjualanFiltered.Any())
                {
                    return Json(new { success = false, message = $"Data laporan {tipelaporan} tidak ditemukan untuk periode ini." });
                }
                var fileName = $"Laporan-Penjualan-{timestamp}.pdf";
                // Mencegah Memory Leak dengan membungkus MemoryStream ke dalam scope 'using'
                using (var stream = new MemoryStream())
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(30);
                            page.Size(PageSizes.A4);
                            page.DefaultTextStyle(x => x.FontSize(11));
                            page.Header()
                                .AlignCenter()
                                .Column(col =>
                                {
                                    col.Item().Text($"Laporan {tipelaporan}").Bold().FontSize(16).AlignCenter();
                                    col.Item().Text($"Periode: {mulai:dd-MM-yyyy} s/d {selesai:dd-MM-yyyy}").FontSize(10).AlignCenter();
                                });
                            page.Content().PaddingVertical(10).Column(col =>
                            {
                                foreach (var transaksi in penjualanFiltered)
                                {
                                    col.Item().PaddingBottom(5).Column(headerCol =>
                                    {
                                        headerCol.Item().Text($"No. Transaksi: {transaksi.NoTransaksi}").Bold();
                                        headerCol.Item().Text($"Tanggal: {transaksi.TanggalTransaksi:dd-MM-yyyy HH:mm}");
                                    });
                                    col.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(c =>
                                        {
                                            c.RelativeColumn(5); // Produk
                                            c.RelativeColumn(2); // Qty
                                            c.RelativeColumn(2); // Unit
                                            c.RelativeColumn(3); // Harga
                                            c.RelativeColumn(3); // Sub Total                                        
                                        });
                                        table.Header(header =>
                                        {
                                            header.Cell().Background("#f0f0f0").Padding(4).BorderBottom(1).Text("Produk").Bold();
                                            header.Cell().Background("#f0f0f0").Padding(4).BorderBottom(1).AlignCenter().Text("Qty").Bold();
                                            header.Cell().Background("#f0f0f0").Padding(4).BorderBottom(1).AlignCenter().Text("Unit").Bold();
                                            header.Cell().Background("#f0f0f0").Padding(4).BorderBottom(1).AlignRight().Text("Harga Jual").Bold();
                                            header.Cell().Background("#f0f0f0").Padding(4).BorderBottom(1).AlignRight().Text("Sub Total").Bold();
                                        });
                                        foreach (var detail in transaksi.TransaksiDetails ?? Enumerable.Empty<TransaksiDetail>())
                                        {
                                            table.Cell().Padding(3).Text(detail.Produk?.NamaProduk ?? "N/A");
                                            table.Cell().Padding(3).AlignCenter().Text(detail.Qty.ToString());
                                            table.Cell().Padding(3).AlignCenter().Text(detail.ProductUnit?.UnitName?.ToString() ?? "-");
                                            table.Cell().Padding(3).AlignRight().Text($"Rp {detail.HargaJual:N0}");
                                            table.Cell().Padding(3).AlignRight().Text($"Rp {(detail.Qty * detail.HargaJual):N0}");
                                        }
                                    });
                                    col.Item().PaddingTop(3).LineHorizontal(0.5f);
                                    col.Item().AlignRight().PaddingTop(2).PaddingRight(5).Text($"Total Belanja: Rp {transaksi.TotalBelanja:N0}").Bold();
                                    col.Item().PaddingVertical(8).Row(row =>
                                    {
                                        row.ConstantItem(500).BorderBottom(0.5f).BorderColor("#bbbbbb");
                                    });
                                }
                                col.Item().AlignRight().PaddingTop(10).Text($"Grand Total Penjualan: Rp {penjualanFiltered.Sum(x => x.TotalBelanja):N0}").Bold().FontSize(13);
                            });
                            page.Footer().AlignCenter().Text($"Generated otomatis by sistem - {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });
                    document.GeneratePdf(stream);
                    // Konversi menjadi Array Byte agar aman dialirkan keluar saat memory stream ditutup/di-dispose
                    return File(stream.ToArray(), "application/pdf", fileName);
                }
            }
            else if (tipelaporan == "Masuk" || tipelaporan == "Keluar")
            {
                var laporan = await _repo.GetLaporanKasAsync(tipelaporan, mulai, selesai);
                if (laporan == null || !laporan.Any())
                {
                    return Json(new { success = false, message = $"Data laporan {tipelaporan} tidak ditemukan." });
                }
                var fileName = $"Laporan-{tipelaporan}-{timestamp}.pdf";
                using (var stream = new MemoryStream())
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(30);
                            page.Size(PageSizes.A4);
                            page.DefaultTextStyle(x => x.FontSize(11));
                            page.Header()
                                .AlignCenter()
                                .Column(col =>
                                {
                                    col.Item().Text($"Laporan {tipelaporan}").Bold().FontSize(16).AlignCenter();
                                    col.Item().Text($"Periode: {mulai:dd-MM-yyyy} s/d {selesai:dd-MM-yyyy}").FontSize(10).AlignCenter();
                                });
                            page.Content().PaddingVertical(10).Column(col =>
                            {
                                col.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(c =>
                                    {
                                        c.RelativeColumn(2); // Tanggal
                                        c.RelativeColumn(2); // Tipe
                                        c.RelativeColumn(3); // Sumber
                                        c.RelativeColumn(3); // Total
                                        c.RelativeColumn(4); // Catatan
                                    });
                                    table.Header(header =>
                                    {
                                        header.Cell().Background("#e0e0e0").Padding(5).Text("Tanggal").Bold();
                                        header.Cell().Background("#e0e0e0").Padding(5).Text("Tipe").Bold();
                                        header.Cell().Background("#e0e0e0").Padding(5).Text("Sumber").Bold();
                                        header.Cell().Background("#e0e0e0").Padding(5).AlignRight().Text("Total").Bold();
                                        header.Cell().Background("#e0e0e0").Padding(5).Text("Catatan").Bold();
                                    });
                                    foreach (var item in laporan)
                                    {
                                        // Fix Bug 2: Safe navigation menggunakan ?. untuk menghindari NullReferenceException
                                        table.Cell().Padding(5).Text(item.Date.ToString("dd-MM-yyyy"));
                                        table.Cell().Padding(5).Text(item.CashFlow?.TipeCashFlow?.TipeCashFlowName ?? "-");
                                        table.Cell().Padding(5).Text(item.CashFlow?.CashFlowName ?? "-");
                                        table.Cell().Padding(5).AlignRight().Text($"Rp {item.Amount:N0}");
                                        table.Cell().Padding(5).Text(item.Notes ?? "-");
                                    }
                                    var totalKeseluruhan = laporan.Sum(x => x.Amount);
                                    // Fix Bug 3: Alignment kolom total yang benar agar nominal sejajar di bawah kolom "Total"
                                    table.Cell().ColumnSpan(3)
                                        .BorderTop(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5).Text("Total Keseluruhan:").Bold().AlignRight();
                                    table.Cell().ColumnSpan(1)
                                        .BorderTop(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5).AlignRight().Text($"Rp {totalKeseluruhan:N0}").Bold();
                                    table.Cell().ColumnSpan(1)
                                        .BorderTop(1).BorderColor(Colors.Grey.Lighten2)
                                        .Padding(5).Text("");
                                });
                            });
                            page.Footer().AlignCenter().Text($"Generated otomatis by sistem - {DateTime.Now:dd/MM/yyyy HH:mm}");
                        });
                    });
                    document.GeneratePdf(stream);
                    // Cara terstandar di ASP.NET Core untuk memicu mode 'inline' (View langsung di Browser) 
                    // tanpa memicu interupsi paksa dari IDM / download manager eksternal.
                    var contentDisposition = new ContentDispositionHeaderValue("inline")
                    {
                        FileName = fileName
                    };
                    Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
                    return File(stream.ToArray(), "application/pdf");
                }
            }
            return Json(new { success = false, message = "Tipe laporan tidak valid." });
        }
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            // Di kode asli Anda menggunakan PaginatedList.CreateAsync, 
            // di sini kita sesuaikan menggunakan data dari repo.
            var (paginatedData, totalCount) = await _repo.GetPaginatedLaporanAsync(pageNumber, pageSize);
            var data = paginatedData.Select(lk => new LaporanKeuanganVm
            {
                KodeLaporanKeuangan = lk.KodeLaporanKeuangan,
                TipeLaporan = lk.TipeLaporan,
                TanggalMulai = lk.TanggalMulai,
                TanggalSelesai = lk.TanggalSelesai
            }).ToList();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var model = new LaporanKeuanganIndexVm
            {
                LaporanKeuanganList = data,
                TotalPages = totalPages,
                PageIndex = pageNumber,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var (paginated, totalCount) = await _repo.GetPaginatedLaporanAsync(pageNumber, pageSize);
            var items = paginated.Select(lk => new LaporanKeuanganVm
            {
                KodeLaporanKeuangan = lk.KodeLaporanKeuangan,
                TipeLaporan = lk.TipeLaporan,
                TanggalMulai = lk.TanggalMulai,
                TanggalSelesai = lk.TanggalSelesai
            }).ToList();
            var viewModel = new LaporanKeuanganIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                LaporanKeuanganList = items
            };
            return View("Index", viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var data = await _repo.GetByIdAsync(id);
            if (data == null)
                return NotFound();
            var model = new LaporanKeuanganVm
            {
                KodeLaporanKeuangan = data.KodeLaporanKeuangan,
                TipeLaporan = data.TipeLaporan,
                TanggalMulai = data.TanggalMulai,
                TanggalSelesai = data.TanggalSelesai
            };
            return PartialView("~/Views/LaporanKeuangan/_Edit.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(LaporanKeuanganVm model)
        {
            var getById = await _repo.GetByIdAsync(model.KodeLaporanKeuangan);
            if (getById == null)
            {
                return PartialView("~/Views/LaporanKeuangan/_Edit.cshtml", model);
            }
            getById.TipeLaporan = model.TipeLaporan;
            getById.TanggalMulai = model.TanggalMulai;
            getById.TanggalSelesai = model.TanggalSelesai;
            await _repo.UpdateAsync(getById);
            await _repo.SaveChangesAsync();
            return Json(new { success = true, message = "Laporan Keuangan berhasil diupdate!" });
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new LaporanKeuanganVm();
            return PartialView("~/Views/LaporanKeuangan/_Create.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> Create(LaporanKeuanganVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/LaporanKeuangan/_Create.cshtml", model);
            }
            // Logika generate kode sudah dibungkus rapi di dalam repo
            string finalKodeLaporan = await _repo.GenerateKodeLaporanAsync();
            var models = new LaporanKeuangan
            {
                KodeLaporanKeuangan = finalKodeLaporan,
                TipeLaporan = model.TipeLaporan,
                TanggalMulai = model.TanggalMulai,
                TanggalSelesai = model.TanggalSelesai
            };
            await _repo.AddAsync(models);
            await _repo.SaveChangesAsync();
            return Json(new { success = true, message = "Laporan Keuangan berhasil dibuat!" });
        }
    }
}
