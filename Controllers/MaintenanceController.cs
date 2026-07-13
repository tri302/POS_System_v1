using Microsoft.AspNetCore.Mvc;
namespace POS_System.Controllers
{
    public class MaintenanceController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Maintenance/Maintenance.cshtml");
    }
    }
}
