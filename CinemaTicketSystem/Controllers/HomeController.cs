using System.Diagnostics;
using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicketSystem.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDBContext _context;// = new();

        public HomeController(ILogger<HomeController> logger, ApplicationDBContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
