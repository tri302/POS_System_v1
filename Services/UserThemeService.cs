using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POS_System.Models;
namespace POS_System.Services
{
    public interface IUserThemeService
    {
        Task<string> GetThemeAsync(string userId);
        Task SetThemeAsync(string userId, string theme);
    }
    public class UserThemeService : IUserThemeService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserThemeService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<string> GetThemeAsync(string userId)
        {
            var user = await _userManager.Users
                .Where(x => x.Id == userId)
                .Select(x => x.Theme)
                .FirstOrDefaultAsync();
            return user ?? "default";
        }
        public async Task SetThemeAsync(string userId, string theme)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;
            user.Theme = theme;
            await _userManager.UpdateAsync(user);
        }
    }
}
