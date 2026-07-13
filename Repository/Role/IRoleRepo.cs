using Microsoft.AspNetCore.Identity;
namespace POS_System.Repository
{
    public interface IRoleRepo
    {
        List<IdentityRole> GetAllRoles();
        Task<IdentityRole?> FindByIdAsync(string id);
        Task<IdentityResult> CreateAsync(string name);
        Task<IdentityResult> UpdateAsync(IdentityRole role, string newName);
        Task<IdentityResult> DeleteAsync(IdentityRole role);
    }
}