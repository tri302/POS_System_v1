using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POS_System.Models;
using POS_System.Pageing;
using POS_System.Repository;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class MetodeBayarController : Controller
    {
        private readonly IMetodeBayarRepo _repo;
        // Inject interface repository melalui constructor
        public MetodeBayarController(IMetodeBayarRepo repo)
        {
            _repo = repo;
        }
        // GET: MetodeBayar
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _repo.GetAllQuery();
            var paginatedData = await PaginatedList<MetodeBayar>.CreateAsync(query, pageNumber, pageSize);
            var data = paginatedData.Select(m => new MetodeBayarVmCreate
            {
                IdMetodeBayar = m.IdMetodeBayar,
                NamaMetodeBayar = m.NamaMetodeBayar,
                UrlGambar = m.UrlGambar
            }).ToList();
            var viewModel = new MetodeBayarIndexVm
            {
                MetodeBayarList = data,
                TotalPages = paginatedData.TotalPages,
                PageIndex = paginatedData.PageIndex,
                HasPreviousPage = paginatedData.HasPreviousPage,
                HasNextPage = paginatedData.HasNextPage
            };
            return View(viewModel);
        }
        // AJAX load halaman selain halaman pertama
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _repo.GetAllQuery();
            int count = await query.CountAsync();
            var paginated = await query
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
            var items = paginated.Select(m => new MetodeBayarVmCreate
            {
                IdMetodeBayar = m.IdMetodeBayar,
                NamaMetodeBayar = m.NamaMetodeBayar,
                UrlGambar = m.UrlGambar
            }).ToList();
            var viewModel = new MetodeBayarIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                MetodeBayarList = items
            };
            return View("Index", viewModel);
        }
        // GET: MetodeBayar/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var query = _repo.GetAllQuery();
            var metodeBayar = await query.FirstOrDefaultAsync(m => m.IdMetodeBayar == id);
            if (metodeBayar == null) return NotFound();
            return View(metodeBayar);
        }
        // GET: MetodeBayar/Create
        public IActionResult _Create()
        {
            var model = new MetodeBayarVmCreate();
            return PartialView("~/Views/MetodeBayar/_Create.cshtml", model);
        }
        // POST: MetodeBayar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Create(MetodeBayarVmCreate metodeBayar)
        {
            if (ModelState.IsValid)
            {
                var file = metodeBayar.UploadGambar;
                if (file != null && file.Length > 0)
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(nameof(metodeBayar.UploadGambar), "Format file tidak valid.");
                        return PartialView("~/Views/MetodeBayar/_Create.cshtml", metodeBayar);
                    }
                    var maxFileSize = 2 * 1024 * 1024;
                    if (file.Length > maxFileSize)
                    {
                        ModelState.AddModelError(nameof(metodeBayar.UploadGambar), "Ukuran file terlalu besar (Max 2MB).");
                        return PartialView("~/Views/MetodeBayar/_Create.cshtml", metodeBayar);
                    }
                    var uniqueFileName = Guid.NewGuid().ToString() + extension;
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/metodebayar");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    metodeBayar.UrlGambar = "/images/metodebayar/" + uniqueFileName;
                }
                var model = new MetodeBayar
                {
                    NamaMetodeBayar = metodeBayar.NamaMetodeBayar,
                    UrlGambar = metodeBayar.UrlGambar
                };
                await _repo.AddAsync(model);
                await _repo.SaveChangesAsync();
                return Json(new { success = true, message = "Metode Pembayaran berhasil ditambahkan!" });
            }
            return PartialView("~/Views/MetodeBayar/_Create.cshtml", metodeBayar);
        }
        // GET: MetodeBayar/Edit/5
        [HttpGet]
        public async Task<IActionResult> _Edit(int? id)
        {
            if (id == null) return NotFound();
            var metodeBayar = await _repo.GetByIdAsync(id.Value);
            if (metodeBayar == null) return NotFound();
            var model = new MetodeBayarVmEdit
            {
                IdMetodeBayar = metodeBayar.IdMetodeBayar,
                NamaMetodeBayar = metodeBayar.NamaMetodeBayar,
                UrlGambar = metodeBayar.UrlGambar
            };
            return View(model);
        }
        // POST: MetodeBayar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _Edit(int id, [Bind("IdMetodeBayar,NamaMetodeBayar,UrlGambar,UploadGambarEdit")] MetodeBayarVmEdit metodeBayar)
        {
            if (id != metodeBayar.IdMetodeBayar) return NotFound();
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/MetodeBayar/_Edit.cshtml", metodeBayar);
            }
            var file = metodeBayar.UploadGambarEdit;
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("UploadGambar", "Format file tidak valid.");
                    return PartialView("~/Views/MetodeBayar/_Edit.cshtml", metodeBayar);
                }
                var maxFileSize = 2 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    ModelState.AddModelError("UploadGambar", "Ukuran file terlalu besar.");
                    return PartialView("~/Views/MetodeBayar/_Edit.cshtml", metodeBayar);
                }
            }
            try
            {
                var query = _repo.GetAllQuery();
                if (file != null && file.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/metodebayar");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    // Cari data lama (AsNoTracking) untuk menghapus file fisik yang lama
                    var existingMetodeBayarForFile = await query.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.IdMetodeBayar == metodeBayar.IdMetodeBayar);
                    if (existingMetodeBayarForFile != null && !string.IsNullOrEmpty(existingMetodeBayarForFile.UrlGambar))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingMetodeBayarForFile.UrlGambar.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    metodeBayar.UrlGambar = "/images/metodebayar/" + uniqueFileName;
                }
                else
                {
                    var existingMetodeBayar = await query.AsNoTracking()
                        .FirstOrDefaultAsync(m => m.IdMetodeBayar == metodeBayar.IdMetodeBayar);
                    metodeBayar.UrlGambar = existingMetodeBayar?.UrlGambar;
                }
                var model = new MetodeBayar
                {
                    IdMetodeBayar = metodeBayar.IdMetodeBayar,
                    NamaMetodeBayar = metodeBayar.NamaMetodeBayar,
                    UrlGambar = metodeBayar.UrlGambar
                };
                _repo.Update(model);
                await _repo.SaveChangesAsync();
                return Json(new { success = true, message = "Metode Pembayaran berhasil diubah!" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _repo.ExistsAsync(metodeBayar.IdMetodeBayar))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        // GET: MetodeBayar/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var query = _repo.GetAllQuery();
            var metodeBayar = await query.FirstOrDefaultAsync(m => m.IdMetodeBayar == id);
            if (metodeBayar == null) return NotFound();
            return View(metodeBayar);
        }
        // POST: MetodeBayar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var metodeBayar = await _repo.GetByIdAsync(id);
            if (metodeBayar != null)
            {
                _repo.Delete(metodeBayar);
                await _repo.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
