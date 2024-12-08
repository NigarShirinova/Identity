using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "ADMIN")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
