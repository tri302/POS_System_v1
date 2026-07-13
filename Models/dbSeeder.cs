using Microsoft.AspNetCore.Identity;
namespace POS_System.Models
{
    public static class dbSeeder
    {
        public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Definisikan peran yang dibutuhkan
            string[] roleNames = { "Owner", "Kasir" };
            foreach (var roleName in roleNames)
            {
                // Cek apakah peran sudah ada
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    // Jika belum, buat peran tersebut
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Membuat pengguna Admin/Owner default pertama
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "PasswordAnda!123"); // GANTI dengan password kuat
                    await userManager.AddToRoleAsync(defaultUser, "Owner"); // Tetapkan sebagai Owner
                }
            }
        }
    }
}
