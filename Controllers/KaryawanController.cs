using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POS_System.Models;
using POS_System.Pageing;
using POS_System.Repository;
using POS_System.ViewModels;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class KaryawanController : Controller
    {
        private readonly IKaryawanRepo _karyawanRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public KaryawanController(
            IKaryawanRepo karyawanRepo,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _karyawanRepo = karyawanRepo;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 5;
            var karyawanQuery = _karyawanRepo.GetAllQueryable();
            var paginatedData = await PaginatedList<Karyawan>.CreateAsync(karyawanQuery, pageNumber, pageSize);
            var paginatedVmList = paginatedData
                .Select(x => new KaryawanVm
                {
                    IdKaryawan = x.IdKaryawan,
                    NIK = x.NIK,
                    Name = x.Name,
                    TempatLahir = x.TempatLahir,
                    TanggalLahir = x.TanggalLahir,
                    AlamatLengkap = x.AlamatLengkap,
                    Email = x.Email,
                    NoHp = x.NoHp,
                    StatusNikah = x.StatusNikah,
                    StatusKerja = x.StatusKerja,
                    DateJoin = x.DateJoin,
                    Catatan = x.Catatan,
                    FotoPath = x.FotoPath,
                    StatusKaryawan = x.StatusKaryawan
                })
                .ToList();
            var KaryawanIndexModel = new KaryawanIndexVm
            {
                KaryawanList = paginatedVmList,
                TotalPages = paginatedData.TotalPages,
                PageIndex = paginatedData.PageIndex,
                HasPreviousPage = paginatedData.HasPreviousPage,
                HasNextPage = paginatedData.HasNextPage
            };
            return View(KaryawanIndexModel);
        }
        [HttpGet]
        public async Task<IActionResult> LoadPage(int pageNumber = 1)
        {
            int pageSize = 5;
            var karyawanQuery = _karyawanRepo.GetAllQueryable();
            int count = await karyawanQuery.CountAsync();
            var paginatedKaryawans = await karyawanQuery
                 .Skip((pageNumber - 1) * pageSize)
                 .Take(pageSize)
                 .ToListAsync();
            var items = paginatedKaryawans.Select(x => new KaryawanVm
            {
                IdKaryawan = x.IdKaryawan,
                NIK = x.NIK,
                Name = x.Name,
                TempatLahir = x.TempatLahir,
                TanggalLahir = x.TanggalLahir,
                AlamatLengkap = x.AlamatLengkap,
                Email = x.Email,
                NoHp = x.NoHp,
                StatusNikah = x.StatusNikah,
                StatusKerja = x.StatusKerja,
                DateJoin = x.DateJoin,
                Catatan = x.Catatan,
                FotoPath = x.FotoPath,
                StatusKaryawan = x.StatusKaryawan
            })
            .ToList();
            var viewModel = new KaryawanIndexVm
            {
                PageIndex = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                KaryawanList = items
            };
            return View("Index", viewModel);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.RoleList = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KaryawanVm model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    if (model.UploadFoto != null && model.UploadFoto.Length > 0)
                    {
                        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        var uniqName = Guid.NewGuid().ToString() + "_" + timestamp + "_" + Path.GetFileName(model.UploadFoto.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                        if (!Directory.Exists(uploadPath))
                            Directory.CreateDirectory(uploadPath);
                        var filePath = Path.Combine(uploadPath, uniqName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.UploadFoto.CopyToAsync(stream);
                        }
                        model.FotoPath = "/uploads/" + uniqName;
                    }
                    var karyawan = new Karyawan
                    {
                        UserId = user.Id,
                        NIK = model.NIK,
                        Name = model.Name,
                        TempatLahir = model.TempatLahir,
                        TanggalLahir = model.TanggalLahir,
                        AlamatLengkap = model.AlamatLengkap,
                        Email = model.Email,
                        NoHp = model.NoHp,
                        StatusNikah = model.StatusNikah,
                        StatusKerja = model.StatusKerja,
                        DateJoin = model.DateJoin,
                        Catatan = model.Catatan,
                        FotoPath = model.FotoPath,
                        StatusKaryawan = model.StatusKaryawan
                    };
                    await _karyawanRepo.AddAsync(karyawan);
                    await _karyawanRepo.SaveChangesAsync();
                    TempData["SweetAlertSuccess"] = "Data berhasil disimpan";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            var invalidRoles = _roleManager.Roles.ToList();
            ViewBag.RoleList = invalidRoles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var karyawan = await _karyawanRepo.GetByIdAsync(id);
            if (karyawan == null)
            {
                return NotFound();
            }
            string currentRole = "";
            var user = await _userManager.FindByEmailAsync(karyawan.Email);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                currentRole = userRoles.FirstOrDefault() ?? "";
            }
            var model = new KaryawanVm
            {
                IdKaryawan = karyawan.IdKaryawan,
                NIK = karyawan.NIK,
                Name = karyawan.Name,
                TempatLahir = karyawan.TempatLahir,
                TanggalLahir = karyawan.TanggalLahir,
                AlamatLengkap = karyawan.AlamatLengkap,
                Email = karyawan.Email,
                NoHp = karyawan.NoHp,
                StatusNikah = karyawan.StatusNikah,
                StatusKerja = karyawan.StatusKerja,
                DateJoin = karyawan.DateJoin,
                Catatan = karyawan.Catatan,
                FotoPath = karyawan.FotoPath,
                StatusKaryawan = karyawan.StatusKaryawan,
                Role = currentRole
            };
            var allRoles = _roleManager.Roles.ToList();
            ViewBag.RoleList = allRoles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KaryawanVm model)
        {
            var roles = _roleManager.Roles.ToList();
            ViewBag.RoleList = roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            }).ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var karyawan = await _karyawanRepo.GetByIdAsync(id);
            if (karyawan == null)
            {
                TempData["SweetAlertError"] = "Data Karyawan tidak ditemukan.";
                return RedirectToAction(nameof(Index));
            }
            var user = await _userManager.FindByEmailAsync(karyawan.Email);
            if (user != null)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                var oldRole = currentRoles.FirstOrDefault();
                var newRole = model.Role;
                if (oldRole != newRole)
                {
                    if (currentRoles.Any())
                    {
                        await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    }
                    if (!string.IsNullOrWhiteSpace(newRole))
                    {
                        var roleResult = await _userManager.AddToRoleAsync(user, newRole);
                        if (!roleResult.Succeeded)
                        {
                            TempData["SweetAlertWarning"] = "Data Karyawan berhasil diubah, tetapi Gagal memperbarui Role.";
                        }
                    }
                }
            }
            if (model.UploadFoto != null && model.UploadFoto.Length > 0)
            {
                if (!string.IsNullOrEmpty(karyawan.FotoPath) && karyawan.FotoPath != "/images/no-image.png")
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", karyawan.FotoPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var uniqName = Guid.NewGuid().ToString() + "_" + timestamp + "_" + Path.GetFileName(model.UploadFoto.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);
                var filePath = Path.Combine(uploadPath, uniqName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.UploadFoto.CopyToAsync(stream);
                }
                karyawan.FotoPath = "/uploads/" + uniqName;
            }
            else if (!string.IsNullOrEmpty(karyawan.FotoPath) && karyawan.FotoPath.Contains("no-image.png"))
            {
                karyawan.FotoPath = "/images/no-image.png";
            }
            if (user != null && karyawan.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (setEmailResult.Succeeded)
                {
                    await _userManager.SetUserNameAsync(user, model.Email);
                    karyawan.Email = model.Email;
                }
                else
                {
                    TempData["SweetAlertWarning"] = "Gagal memperbarui Email User Identity. Data Karyawan lainnya disimpan.";
                }
            }
            else
            {
                karyawan.Email = model.Email;
            }
            //if (user != null && !string.IsNullOrEmpty(model.Password))
            //{
            //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            //    var resetResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
            //    if (!resetResult.Succeeded)
            //    {
            //        TempData["SweetAlertWarning"] = "Gagal memperbarui Password User Identity. Data Karyawan lainnya disimpan.";
            //    }
            //}
            karyawan.NIK = model.NIK;
            karyawan.Name = model.Name;
            karyawan.TempatLahir = model.TempatLahir;
            karyawan.TanggalLahir = model.TanggalLahir;
            karyawan.AlamatLengkap = model.AlamatLengkap;
            karyawan.NoHp = model.NoHp;
            karyawan.StatusNikah = model.StatusNikah;
            karyawan.StatusKerja = model.StatusKerja;
            karyawan.DateJoin = model.DateJoin;
            karyawan.Catatan = model.Catatan;
            karyawan.StatusKaryawan = model.StatusKaryawan;
            _karyawanRepo.Update(karyawan);
            await _karyawanRepo.SaveChangesAsync();
            TempData["SweetAlertSuccess"] = "Data Karyawan dan Role berhasil diperbarui!";
            return RedirectToAction(nameof(Index));
        }
    }
}