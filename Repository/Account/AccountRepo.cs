using Microsoft.AspNetCore.Identity;
using POS_System.Models;
namespace POS_System.Repository
{
    public class AccountRepo : IAccountRepo
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountRepo(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // Method mengembalikan SignInResult dan ApplicationUser agar Controller bisa cek Role & Status
        public async Task<(Microsoft.AspNetCore.Identity.SignInResult Result, ApplicationUser? User)> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return (Microsoft.AspNetCore.Identity.SignInResult.Failed, null);
            }
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);
            return (result, user);
        }
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }        
    }
}