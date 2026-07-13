using Serilog;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using POS_System.DbCtx;
using POS_System.Models;
using POS_System.Services;
using QuestPDF.Infrastructure;
using POS_System.Middleware;
using POS_System.Repository;
using POS_System;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHostedService<DatabaseKeepAliveService>();
builder.Services.AddScoped<IShiftsRepo, ShiftsRepo>();
builder.Services.AddScoped<IReportRepo, ReportRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<IAlurKasRepo, AlurKasRepo>();
builder.Services.AddScoped<IInventoriRepo, InventoriRepo>();
builder.Services.AddScoped<IKaryawanRepo, KaryawanRepo>();
builder.Services.AddScoped<IKasirRepo, KasirRepo>();
builder.Services.AddScoped<IKategoriRepo, KategoriRepo>();
builder.Services.AddScoped<ILaporanKeuanganRepo, LaporanKeuanganRepo>();
builder.Services.AddScoped<IMetodeBayarRepo, MetodeBayarRepo>();
builder.Services.AddScoped<IPelanggansRepo, PelanggansRepo>();
builder.Services.AddScoped<IPengaturanRepo, PengaturanRepo>();
builder.Services.AddScoped<IProdukRepo, ProdukRepo>();
builder.Services.AddScoped<IRoleRepo, RoleRepo>();
builder.Services.AddScoped<ITransaksiRepo, TransaksiRepo>();
// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
// Paksa aplikasi menyimpan kunci enkripsi di folder proyek, bukan di sistem OS hosting
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "PersistedKeys")));
//Konfigurasi logger
// handle global error handling pakai Logger
Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddScoped<IUserThemeService, UserThemeService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddScoped<IDashboardService, DashboardService>();
// Konfigurasi QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;
//Tambahkan layanan SignalR
builder.Services.AddSignalR();
//Konfigurasi koneksi ke db
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
    );
});
//Configure Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password policy
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
        // Lockout
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        // User
        options.User.RequireUniqueEmail = true;
        // Signin
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
//Gunakan PostConfigure untuk mengonfigurasi Cookie yang dibuat oleh Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Error/Forbidden";    
    options.LogoutPath = "/Account/Logout";
    //options.AccessDeniedPath = "/Account/Login"; // Jika tidak punya akses, kunci ke login saja
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    //options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    // UBAH BAGIAN INI: Jangan Always, ganti jadi SameAsRequest
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    // UBAH BAGIAN INI: Dari Strict ganti ke Lax agar redirect antar domain/proxy tidak memblokir cookie
    options.Cookie.SameSite = SameSiteMode.Lax;
    //options.Cookie.SameSite = SameSiteMode.Strict;
});
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
var app = builder.Build();
// Configure the HTTP request pipeline.
// 🔴 GLOBAL ERROR HANDLER
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    //app.UseExceptionHandler("/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Error/{0}"); // handle global error handling
app.UseExceptionHandler("/Error/ServerError"); // handle global error handling
app.UseMiddleware<GlobalExceptionMiddleware>(); // handle global error handling
app.UseMiddleware<MaintenanceMiddleware>(); // handle global error handling
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Jalankan Session setelah Routing
app.UseSession();
app.MapStaticAssets();
app.UseAuthentication(); // Middleware untuk memproses cookie/token login
app.UseAuthorization();  // Middleware untuk memeriksa izin
app.MapControllerRoute(
    name: "default",    
    //pattern: "{controller=Dashboard}/{action=Index}/{id?}");
    pattern: "{controller=Account}/{action=Login}/{id?}");
//SignalR
// Map the Hub endpoint
app.MapHub<CashFlowHub>("/cashFlowHub");
// Dapatkan service scope
// Dbseeder administrator
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // Panggil Seeder
        await dbSeeder.SeedRolesAsync(userManager, roleManager);
        await dbSeeder.SeedAdminUserAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.Run();
