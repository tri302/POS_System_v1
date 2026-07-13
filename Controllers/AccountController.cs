using POS_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS_System.Repository;
namespace POS_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepo _accountRepo;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(IAccountRepo accountRepo, SignInManager<ApplicationUser> signInManager)
        {
            _accountRepo = accountRepo;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // ================= LOGIN =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);
            (Microsoft.AspNetCore.Identity.SignInResult result, ApplicationUser? user) = await _accountRepo.LoginAsync(model);
            if (result.Succeeded && user != null)
            {
                if (await _accountRepo.IsInRoleAsync(user, "Owner"))
                {
                    return LocalRedirect("/Dashboard/Index");
                }
                if (await _accountRepo.IsInRoleAsync(user, "Kasir"))
                {
                    return LocalRedirect("/Kasir/Index");
                }
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                TempData["ErrorMessage"] = "Akun Anda terkunci sementara, silakan coba beberapa saat lagi.";
                return View(model);
            }
            TempData["ErrorMessage"] = "Email atau kata sandi Anda salah.";
            return View(model);
        }
        // ================= LOGOUT =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(
                "Login",
                "Account");
        }     
    }
}
