using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using POS_System.Models;
using POS_System.Repository;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class PengaturanController(
    IPengaturanRepo _repo,
    IWebHostEnvironment _env,
    ICompositeViewEngine _viewEngine) : Controller
    {
        // Index page
        public async Task<IActionResult> Index()
        {
            var list = await _repo.GetAllAsync();
            return View(list);
        }
        // Partial: table body (to reload after CRUD)
        public async Task<PartialViewResult> _TablePartial()
        {
            var list = await _repo.GetAllAsync();
            return PartialView("_TablePartial", list);
        }
        // GET: Create form (partial)
        public IActionResult Create()
        {
            var model = new PengaturanToko();
            return PartialView("_FormPartial", model);
        }
        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create([FromForm] PengaturanToko model, IFormFile? LogoFile)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validasi gagal", errors = ModelStateErrors() });
            if (LogoFile != null)
            {
                var uploadResult = await SaveLogoFile(LogoFile);
                if (!uploadResult.Success)
                    return Json(new { success = false, message = uploadResult.Error });
                model.LogoToko = uploadResult.Url;
            }
            await _repo.AddAsync(model);
            await _repo.SaveChangesAsync();
            var html = await RenderPartialViewToStringAsync("_TablePartial", await _repo.GetAllAsync());
            return Json(new { success = true, message = "Data berhasil ditambahkan", html });
        }
        // GET: Edit form (partial)
        public async Task<IActionResult> Edit(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return PartialView("_FormPartial", entity);
        }
        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Edit(int id, [FromForm] PengaturanToko model, IFormFile? LogoFile, bool removeLogo = false)
        {
            if (id != model.Id) return Json(new { success = false, message = "Id tidak cocok" });
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validasi gagal", errors = ModelStateErrors() });
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return Json(new { success = false, message = "Data tidak ditemukan" });
            // Update fields
            entity.NamaToko = model.NamaToko;
            entity.AlamatToko = model.AlamatToko;
            entity.NomorTelepon = model.NomorTelepon;
            if (removeLogo)
            {
                if (!string.IsNullOrEmpty(entity.LogoToko))
                    SafeDeleteFile(entity.LogoToko);
                entity.LogoToko = null;
            }
            if (LogoFile != null)
            {
                var uploadResult = await SaveLogoFile(LogoFile);
                if (!uploadResult.Success)
                    return Json(new { success = false, message = uploadResult.Error });
                if (!string.IsNullOrEmpty(entity.LogoToko))
                    SafeDeleteFile(entity.LogoToko);
                entity.LogoToko = uploadResult.Url;
            }
            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            var html = await RenderPartialViewToStringAsync("_TablePartial", await _repo.GetAllAsync());
            return Json(new { success = true, message = "Data berhasil disimpan", html });
        }
        // POST: Delete (Uncommented & updated)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return Json(new { success = false, message = "Data tidak ditemukan" });
            if (!string.IsNullOrEmpty(entity.LogoToko))
                SafeDeleteFile(entity.LogoToko);
            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
            var html = await RenderPartialViewToStringAsync("_TablePartial", await _repo.GetAllAsync());
            return Json(new { success = true, message = "Data berhasil dihapus", html });
        }
        #region Helpers (Tetap sama)
        private IEnumerable<string> ModelStateErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
        }
        private async Task<(bool Success, string Url, string Error)> SaveLogoFile(IFormFile file)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                return (false, "", "Format file tidak didukung. Hanya .jpg, .jpeg, .png");
            const long maxBytes = 2 * 1024 * 1024;
            if (file.Length > maxBytes)
                return (false, "", "Ukuran file maksimal 2 MB");
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "logo");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var url = $"/uploads/logo/{fileName}";
            return (true, url, "");
        }
        private void SafeDeleteFile(string url)
        {
            try
            {
                var relative = url.TrimStart('/');
                var full = Path.Combine(_env.WebRootPath, relative.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(full))
                    System.IO.File.Delete(full);
            }
            catch { }
        }
        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            var actionContext = new ActionContext(HttpContext, RouteData, ControllerContext.ActionDescriptor, ModelState);
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(actionContext, viewName, false);
                if (viewResult.View == null)
                    throw new ArgumentNullException($"View {viewName} not found.");
                var viewDictionary = new ViewDataDictionary(new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(), ModelState)
                {
                    Model = model
                };
                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    sw,
                    new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
                );
                await viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
        #endregion
    }
}