using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using POS_System.Models;
using POS_System.Services;
namespace POS_System.Controllers
{
    [Authorize]
    public class ThemeController(IUserThemeService _themeService, UserManager<ApplicationUser> _userManager) : Controller
    {
        // Ganti nama action ini (misalnya jadi UpdateTheme) dan gunakan logika ini
        [HttpPost]
        public async Task<IActionResult> SaveTheme(string theme)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            // Menggunakan IUserThemeService untuk menyimpan tema ke database
            var userId = _userManager.GetUserId(User);
            await _themeService.SetThemeAsync(userId, theme);
            return Json(new { success = true, theme = theme });
        }
    }
}
