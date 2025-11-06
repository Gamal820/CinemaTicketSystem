using Microsoft.AspNetCore.Mvc;
using CinemaTicketSystem.Services;
using CinemaTicketSystem.Utitlies;
using Microsoft.AspNetCore.Authorization;

namespace CinemaMoviesystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public IActionResult Index()
        {
            var viewModel = _dashboardService.GetDashboardData();
            return View(viewModel);
        }
    }
}
