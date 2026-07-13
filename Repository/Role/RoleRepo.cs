using Microsoft.AspNetCore.Identity;
namespace POS_System.Repository
{
    public class RoleRepo : IRoleRepo
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleRepo(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public List<IdentityRole> GetAllRoles()
        {
            return _roleManager.Roles.ToList();
        }
        public async Task<IdentityRole?> FindByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }
        public async Task<IdentityResult> CreateAsync(string name)
        {
            return await _roleManager.CreateAsync(new IdentityRole(name.Trim()));
        }
        public async Task<IdentityResult> UpdateAsync(IdentityRole role, string newName)
        {
            role.Name = newName.Trim();
            role.NormalizedName = newName.Trim().ToUpper();
            return await _roleManager.UpdateAsync(role);
        }
        public async Task<IdentityResult> DeleteAsync(IdentityRole role)
        {
            return await _roleManager.DeleteAsync(role);
        }
    }
}