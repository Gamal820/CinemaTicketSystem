using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketSystem.Areas.Identity.Controllers
{
    public class AccountController : Controller
    {
        [Area("Identity")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
