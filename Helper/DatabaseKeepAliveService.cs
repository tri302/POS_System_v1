using MySql.Data.MySqlClient;
namespace POS_System
{
    public class DatabaseKeepAliveService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DatabaseKeepAliveService> _logger;
        private readonly string? _connectionString;
        // Diubah ke 5 menit agar koneksi tidak diputus duluan oleh firewall / load balancer cloud
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
        public DatabaseKeepAliveService(IConfiguration configuration, ILogger<DatabaseKeepAliveService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Database Keep-Alive Service telah dimulai (Interval: {Interval} menit).", _interval.TotalMinutes);
            // Validasi connection string sejak awal aplikasi berjalan
            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogCritical("Database Keep-Alive gagal dijalankan: Connection string 'DefaultConnection' tidak ditemukan.");
                return;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Mengirim ping ke MySQL Aiven pada: {time}", DateTimeOffset.Now);
                    using (var connection = new MySqlConnection(_connectionString))
                    {
                        // Melemparkan stoppingToken ke OpenAsync
                        await connection.OpenAsync(stoppingToken);
                        using (var command = new MySqlCommand("SELECT 1;", connection))
                        {
                            await command.ExecuteScalarAsync(stoppingToken);
                        }
                    }
                    _logger.LogInformation("Ping Berhasil. Database tetap terjaga.");
                }
                catch (OperationCanceledException)
                {
                    // Diabaikan jika exception dilempar karena aplikasi sedang shutdown/stop
                    _logger.LogInformation("Database Keep-Alive Service sedang dihentikan.");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Gagal mengirim ping ke database. Akan mencoba kembali dalam {Interval} menit.", _interval.TotalMinutes);
                }
                try
                {
                    // Menunggu dengan aman sesuai interval yang baru
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Keluar dari loop dengan aman saat token dibatalkan selama masa tunggu delay
                    break;
                }
            }
            _logger.LogInformation("Database Keep-Alive Service telah sepenuhnya berhenti.");
        }
    }
}