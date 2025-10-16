using Microsoft.AspNetCore.Mvc;

namespace Lumen_Merch_Store.Areas.Admin.Controllers
{
    [Area("Admin")] // IMPORTANT: Specify the area
    public class DashboardController : Controller
    {
        // GET: /Admin/Dashboard
        public IActionResult Index()
        {
            return View();
        }
    }
}