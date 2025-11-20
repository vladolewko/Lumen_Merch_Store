using Microsoft.AspNetCore.Mvc;

namespace Lumen_Merch_Store.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}