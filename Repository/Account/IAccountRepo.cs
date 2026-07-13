using POS_System.Models;
namespace POS_System.Repository
{
    public interface IAccountRepo
    {
        Task<(Microsoft.AspNetCore.Identity.SignInResult Result, ApplicationUser? User)> LoginAsync(LoginViewModel model);
        Task<bool> IsInRoleAsync(ApplicationUser user, string role);
    }
}
