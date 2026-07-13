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
    public class KategoriController : Controller
    {
        private readonly IKategoriRepo _kategoriRepo;
        // Menggunakan primary constructor (.NET 8+) sesuai gaya kode awal Anda
        public KategoriController(IKategoriRepo kategoriRepo)
        {
            _kategoriRepo = kategoriRepo;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _kategoriRepo.GetAllQuery();
            var paginatedData = await PaginatedList<Kategori>.CreateAsync(query, pageNumber, pageSize);
            var data = paginatedData.Select(k => new KategoriVm
            {
                IdKategori = k.IdKategori,
                NamaKategori = k.NamaKategori
            }).ToList();
            var kategoriIndexVm = new KategoriIndexVm
            {
                KategoriList = data,
                TotalPages = paginatedData.TotalPages,
                PageIndex = paginatedData.PageIndex,
                HasPreviousPage = paginatedData.HasPreviousPage,
                HasNextPage = paginatedData.HasNextPage
            };
            return View(kategoriIndexVm);
        }
        // AJAX load halaman selain halaman pertama
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var query = _kategoriRepo.GetAllQuery();
            int count = await query.CountAsync();
            var paginated = await query
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
            var items = paginated.Select(k => new KategoriVm
            {
                IdKategori = k.IdKategori,
                NamaKategori = k.NamaKategori
            }).ToList();
            var viewModel = new KategoriIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                KategoriList = items
            };
            return View("Index", viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var model = new KategoriVm();
            return PartialView("~/Views/Kategori/Create.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KategoriVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Kategori/Create.cshtml", model);
            }
            bool exists = await _kategoriRepo.IsNameExistsAsync(model.NamaKategori);
            if (exists)
            {
                return Json(new { success = false, message = "Kategori dengan nama tersebut sudah ada", isWarning = true });
            }
            var kategori = new Kategori
            {
                NamaKategori = model.NamaKategori
            };
            await _kategoriRepo.AddAsync(kategori);
            await _kategoriRepo.SaveChangesAsync();
            return Json(new { success = true, message = "Data berhasil disimpan!" });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var entity = await _kategoriRepo.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            var model = new KategoriVm
            {
                IdKategori = entity.IdKategori,
                NamaKategori = entity.NamaKategori
            };
            return PartialView("~/Views/Kategori/Edit.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(KategoriVm model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Views/Kategori/Edit.cshtml", model);
            }
            var namaInput = model.NamaKategori?.Trim();
            if (string.IsNullOrEmpty(namaInput))
            {
                return Json(new { success = false, message = "Nama kategori wajib diisi" });
            }
            bool exists = await _kategoriRepo.IsNameExistsAsync(namaInput, model.IdKategori);
            if (exists)
            {
                return Json(new { success = false, isWarning = true, message = "Kategori dengan nama tersebut sudah ada" });
            }
            var entity = await _kategoriRepo.GetByIdAsync(model.IdKategori);
            if (entity == null) return NotFound();
            entity.NamaKategori = namaInput;
            await _kategoriRepo.UpdateAsync(entity);
            await _kategoriRepo.SaveChangesAsync();
            return Json(new { success = true, message = "Data berhasil disimpan!" });
        }
        [HttpGet]
        public IActionResult Details() => View();
        [HttpGet]
        public IActionResult Delete() => View();
    }
}
