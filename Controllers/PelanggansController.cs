using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using POS_System.Models;
using POS_System.Repository;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class PelanggansController : Controller
    {
        private readonly IPelanggansRepo _repo;
        private const int PageSize = 5;
        public PelanggansController(IPelanggansRepo repo)
        {
            _repo = repo;
        }
        // GET: Pelanggans
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var (paginatedData, totalCount) = await _repo.GetPaginatedAsync(pageNumber, PageSize);
            var data = paginatedData.Select(p => new PelangganVm
            {
                Id = p.Id,
                Nama = p.Nama,
                Alamat = p.Alamat,
                Email = p.Email,
                Phone = p.Phone,
                JenisPelangganId = p.JenisPelangganId,
                NamaJenis = p.JenisPelanggan?.NamaJenis
            }).ToList();
            int totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            var viewModel = new PelangganIndexVm
            {
                PelangganList = data,
                TotalPages = totalPages,
                PageIndex = pageNumber,
                HasPreviousPage = pageNumber > 1,
                HasNextPage = pageNumber < totalPages
            };
            return View(viewModel);
        }
        // AJAX load halaman selain halaman pertama
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            var (paginatedData, totalCount) = await _repo.GetPaginatedAsync(pageNumber, PageSize);
            var items = paginatedData.Select(p => new PelangganVm
            {
                Id = p.Id,
                Nama = p.Nama,
                Alamat = p.Alamat,
                Email = p.Email,
                Phone = p.Phone,
                JenisPelangganId = p.JenisPelangganId,
                NamaJenis = p.JenisPelanggan?.NamaJenis
            }).ToList();
            var viewModel = new PelangganIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize),
                PelangganList = items
            };
            return View("Index", viewModel);
        }
        // GET: Pelanggans/Create
        public async Task<IActionResult> Create()
        {
            var jenisPelanggan = await _repo.GetJenisPelanggansAsync();
            ViewData["JenisPelangganId"] = new SelectList(jenisPelanggan, "Id", "NamaJenis");
            return PartialView("~/Views/Pelanggans/_Create.cshtml");
        }
        // POST: Pelanggans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nama,Alamat,Email,Phone,JenisPelangganId")] PelangganVm pelanggan)
        {
            if (ModelState.IsValid)
            {
                var data = new Pelanggan
                {
                    Id = pelanggan.Id,
                    Nama = pelanggan.Nama,
                    Alamat = pelanggan.Alamat,
                    Email = pelanggan.Email,
                    Phone = pelanggan.Phone,
                    JenisPelangganId = pelanggan.JenisPelangganId
                };
                await _repo.AddAsync(data);
                await _repo.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil disimpan!" });
            }
            var jenisPelanggan = await _repo.GetJenisPelanggansAsync();
            ViewData["JenisPelangganId"] = new SelectList(jenisPelanggan, "Id", "NamaJenis", pelanggan.JenisPelangganId);
            return PartialView("~/Views/Pelanggans/_Create.cshtml", pelanggan);
        }
        // GET: Pelanggans/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var pelanggan = await _repo.GetByIdAsync(id);
            if (pelanggan == null) return NotFound();
            var data = new PelangganVm
            {
                Id = pelanggan.Id,
                Nama = pelanggan.Nama,
                IsPinned = pelanggan.IsPinned,
                Alamat = pelanggan.Alamat,
                Email = pelanggan.Email,
                Phone = pelanggan.Phone,
                JenisPelangganId = pelanggan.JenisPelangganId
            };
            var jenisPelanggan = await _repo.GetJenisPelanggansAsync();
            ViewData["JenisPelangganId"] = new SelectList(jenisPelanggan, "Id", "NamaJenis", pelanggan.JenisPelangganId);
            return PartialView("~/Views/Pelanggans/_Edit.cshtml", data);
        }
        // POST: Pelanggans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nama,Alamat,Email,Phone,JenisPelangganId,IsPinned")] PelangganVm pelanggan)
        {
            if (ModelState.IsValid)
            {
                var data = new Pelanggan
                {
                    Id = pelanggan.Id,
                    Nama = pelanggan.Nama,
                    IsPinned = pelanggan.IsPinned,
                    Alamat = pelanggan.Alamat,
                    Email = pelanggan.Email,
                    Phone = pelanggan.Phone,
                    JenisPelangganId = pelanggan.JenisPelangganId
                };
                _repo.Update(data);
                await _repo.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil disimpan!" });
            }
            var jenisPelanggan = await _repo.GetJenisPelanggansAsync();
            ViewData["JenisPelangganId"] = new SelectList(jenisPelanggan, "Id", "NamaJenis", pelanggan.JenisPelangganId);
            return PartialView("~/Views/Pelanggans/_Edit.cshtml", pelanggan);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Data tidak ditemukan." });
            }
            try
            {                
                var item = await _repo.GetByIdAsync(id);
                if (item == null) 
                    return Json(new { success = false, message = "Data tidak ditemukan." });
                _repo.Delete(item);
                await _repo.SaveChangesAsync();
                return Json(new { success = true, message = "Data berhasil dihapus." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // GET: Pelanggans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var pelanggan = await _repo.GetByIdAsync(id);
            if (pelanggan == null) return NotFound();
            return View(pelanggan);
        }
        // POST: Pelanggans/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var pelanggan = await _repo.GetByIdAsync(id);
        //    if (pelanggan != null)
        //    {
        //        _repo.Delete(pelanggan);
        //        await _repo.SaveChangesAsync();
        //    }
        //    return RedirectToAction(nameof(Index));
        //}
    }
}
