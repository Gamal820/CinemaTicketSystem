using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Services
{
    public class DashboardService
    {
        private readonly ApplicationDBContext _context;

        public DashboardService(ApplicationDBContext context)
        {
            _context = context;
        }

        public DashboardVM GetDashboardData()
        {
            var viewModel = new DashboardVM
            {
                TotalMovies = _context.Movies.Count(),
                TotalActors = _context.Actors.Count(),
                TotalCategories = _context.Categories.Count(),
                TotalCinemas = _context.Cinemas.Count(),
                HighestPrice = _context.Movies.Any() ? _context.Movies.Max(t => t.Price) : 0,
                AveragePrice = _context.Movies.Any() ? _context.Movies.Average(t => t.Price) : 0,
                TotalOrders = _context.Movies.Count()
            };

            var totalMovies = _context.Movies.Count();

            if (totalMovies > 0)
            {
                viewModel.MarketShare = _context.Cinemas
                    .Select(c => new
                    {
                        c.Name,
                        Count = _context.Movies.Count(t => t.CinemaId == c.CinemaId)
                    })
                    .ToDictionary(
                        x => x.Name,
                        x => Math.Round((x.Count / (double)totalMovies) * 100, 2)
                    );
            }

            return viewModel;
        }
    }
}
