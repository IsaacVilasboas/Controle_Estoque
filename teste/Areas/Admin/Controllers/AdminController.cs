using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace teste.Areas.GM.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Gerente")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
