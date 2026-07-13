using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POS_System.Models;
namespace POS_System.DbCtx
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<PengaturanToko> PengaturanTokos { get; set; }
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<Karyawan> Karyawans { get; set; }
        public DbSet<Pelanggan> Pelanggans { get; set; }
        public DbSet<JenisPelanggan> JenisPelanggans { get; set; }
        public DbSet<Kategori> Kategoris { get; set; }
        public DbSet<Produk> Produks { get; set; }
        public DbSet<ProdukGambar> ProdukGambars { get; set; }
        public DbSet<Inventori> Inventoris { get; set; }
        public DbSet<Transaksi> Transaksis { get; set; }
        public DbSet<AlurKas> AlurKass { get; set; }
        public DbSet<MetodeBayar> MetodeBayars { get; set; }
        public DbSet<LaporanKeuangan> LaporanKeuangans { get; set; }
        public DbSet<TipeStok> TipeStoks { get; set; }
        public DbSet<SumberStok> SumberStoks { get; set; }
        public DbSet<TransaksiDetail> TransaksiDetails { get; set; }
        public DbSet<TipeCashFlow> TipeCashFlows { get; set; }
        public DbSet<CashFlow> CashFlows { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);                        
            // 1. GLOBAL QUERY FILTER
            modelBuilder.Entity<ProductUnit>().HasQueryFilter(pu => !pu.IsDeleted);
            // Tambahkan ini untuk memfilter Produk yang tidak aktif secara global
            modelBuilder.Entity<Produk>().HasQueryFilter(p => p.StatusProduk != "Tidak Aktif");
            // 2. KATEGORI
            modelBuilder.Entity<Kategori>(entity =>
            {
                entity.ToTable("Kategoris");
                entity.HasIndex(x => x.NamaKategori)
                      .IsUnique()
                      .HasDatabaseName("UX_Kategori_NamaKategori");
            });
            // 3. PRODUCT UNIT
            modelBuilder.Entity<ProductUnit>()
                        .HasOne(pu => pu.Produk)
                        .WithMany(p => p.ProductUnits)
                        .HasForeignKey(pu => pu.IdProduk)
                        .OnDelete(DeleteBehavior.Cascade);
            // 4. TRANSAKSI & DETAIL
            modelBuilder.Entity<Transaksi>()
                        .HasMany(t => t.TransaksiDetails)
                        .WithOne(d => d.Transaksi)
                        .HasForeignKey(d => d.IdTransaksi);
            modelBuilder.Entity<TransaksiDetail>()
                        .HasOne(td => td.Produk)
                        .WithMany(p => p.TransaksiDetails)
                        .HasForeignKey(td => td.IdProduk)
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired(false);
            // 🔥 PERBAIKAN WARNING TRANSAKSI DETAIL: Amankan relasi ProductUnit yang memiliki Global Filter
            modelBuilder.Entity<TransaksiDetail>()
                        .HasOne(td => td.ProductUnit)
                        .WithMany() // Sesuaikan jika ada ICollection di model ProductUnit
                        .HasForeignKey(td => td.ProductUnitId) // Sesuaikan nama properti FK di model TransaksiDetail kamu
                        .IsRequired(false) // <--- Set Opsional agar tidak bentrok dengan Global Filter
                        .OnDelete(DeleteBehavior.Restrict);
            // 5. PRODUK GAMBAR
            modelBuilder.Entity<Produk>()
                        .HasMany(p => p.ProdukGambars)
                        .WithOne(pg => pg.Produk)
                        .HasForeignKey(pg => pg.IdProduk);
            // 6. INVENTORI
            modelBuilder.Entity<Inventori>()
                .HasOne(i => i.Produk)
                .WithMany(p => p.Inventoris)
                .HasForeignKey(i => i.IdProduk)
                .OnDelete(DeleteBehavior.Restrict);
            // 🔥 PERBAIKAN WARNING INVENTORI: Gabungkan dua blok redundan menjadi satu kesatuan yang utuh
            modelBuilder.Entity<Inventori>()
                .HasOne(i => i.ProductUnit)
                .WithMany(p => p.Inventoris)
                .HasForeignKey(i => i.ProductUnitId)
                .IsRequired(false) // <--- Tetap Opsional (Aman dari Global Filter)
                .OnDelete(DeleteBehavior.Restrict); // <--- Keamanan data terjaga
            // 7. CASH FLOW & ALUR KAS
            modelBuilder.Entity<AlurKas>()
                .HasOne(a => a.CashFlow)
                .WithMany()
                .HasForeignKey(a => a.IdCashFlow)
                .OnDelete(DeleteBehavior.Restrict);
            // 8. LOGS OPTIMIZATION
            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("Logs");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TimeStamp).HasDatabaseName("IX_Logs_TimeStamp");
                entity.HasIndex(e => e.Level).HasDatabaseName("IX_Logs_Level");
                entity.HasIndex(e => new { e.Level, e.TimeStamp }).HasDatabaseName("IX_Logs_Level_TimeStamp");
                entity.Property(e => e.Level).HasMaxLength(50);
                entity.Property(e => e.TimeStamp).IsRequired();
            });
        }
    }
}