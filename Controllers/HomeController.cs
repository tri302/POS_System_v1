using Microsoft.AspNetCore.Mvc;
namespace POS_System.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // Endpoint khusus untuk cron-job.org
        [HttpGet("/ping-endpoint")]
        public IActionResult Ping()
        {
            _logger.LogInformation("Web diping oleh cron-job.org pada: {Time}", System.DateTime.Now);
            // Mengembalikan respon teks sederhana 
            return Content("Aplikasi Aktif!", "text/plain");
        }
    }
}
