using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS_System.Repository;
namespace POS_System.Controllers
{
    [Authorize(Roles = "Owner")]
    public class RoleController : Controller
    {
        private readonly IRoleRepo _roleRepo;
        // Inject IRoleRepo di sini, bukan RoleManager langsung
        public RoleController(IRoleRepo roleRepo)
        {
            _roleRepo = roleRepo;
        }
        // GET: /Role/Index
        public IActionResult Index()
        {
            var roles = _roleRepo.GetAllRoles();
            return View(roles);
        }
        // GET: /Role/Create
        public IActionResult Create()
        {
            return View();
        }
        // POST: /Role/Create
        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(name))
            {
                var result = await _roleRepo.CreateAsync(name);
                if (result.Succeeded)
                {
                    TempData["SweetAlertSuccess"] = "Role berhasil dibuat.";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            TempData["SweetAlertError"] = "Gagal membuat Role. Nama tidak boleh kosong.";
            return View((object)name);
        }
        // GET: /Role/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();
            var role = await _roleRepo.FindByIdAsync(id);
            if (role == null) return NotFound();
            return View(role);
        }
        // POST: /Role/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, string name)
        {
            if (id == null || string.IsNullOrWhiteSpace(name))
            {
                TempData["SweetAlertError"] = "ID atau Nama Role tidak valid.";
                return RedirectToAction(nameof(Index));
            }
            var role = await _roleRepo.FindByIdAsync(id);
            if (role == null) return NotFound();
            // Cek apakah tidak ada perubahan nama
            if (role.Name.Trim().ToUpper() == name.Trim().ToUpper())
            {
                TempData["SweetAlertWarning"] = "Tidak ada perubahan pada nama Role.";
                return RedirectToAction(nameof(Index));
            }
            var result = await _roleRepo.UpdateAsync(role, name);
            if (result.Succeeded)
            {
                TempData["SweetAlertSuccess"] = $"Role '{name}' berhasil diperbarui.";
                return RedirectToAction(nameof(Index));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            TempData["SweetAlertError"] = "Gagal memperbarui Role.";
            return View(new IdentityRole { Id = id, Name = name });
        }
        // POST: /Role/Delete
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleRepo.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleRepo.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["SweetAlertSuccess"] = "Role berhasil dihapus.";
                }
                else
                {
                    TempData["SweetAlertError"] = "Gagal menghapus Role.";
                }
            }
            else
            {
                TempData["SweetAlertError"] = "Role tidak ditemukan.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
