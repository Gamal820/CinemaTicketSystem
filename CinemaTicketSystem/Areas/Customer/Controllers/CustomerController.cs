using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTicketSystem.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CustomerController : Controller
    {
        private readonly ApplicationDBContext _context;
        private const int PageSize = 4; // 👈 عدد الأفلام في الصفحة الواحدة

        public CustomerController(ApplicationDBContext context)
        {
            _context = context;
        }

        // 🎬 عرض الأفلام + الفلاتر + التقسيم إلى صفحات
        public async Task<IActionResult> Index(string? name, int? categoryId, int? cinemaId,
                                               decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            var moviesQuery = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            // 🔍 الفلاتر
            if (!string.IsNullOrEmpty(name))
                moviesQuery = moviesQuery.Where(m => m.Name.Contains(name));

            if (categoryId != null)
                moviesQuery = moviesQuery.Where(m => m.CategoryId == categoryId);

            if (cinemaId != null)
                moviesQuery = moviesQuery.Where(m => m.CinemaId == cinemaId);

            if (minPrice != null)
                moviesQuery = moviesQuery.Where(m => m.Price >= minPrice);

            if (maxPrice != null)
                moviesQuery = moviesQuery.Where(m => m.Price <= maxPrice);

            // 📦 البيانات المساعدة
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.Cinemas = await _context.Cinemas.ToListAsync();

            // 📄 Pagination
            var totalMovies = await moviesQuery.CountAsync();
            var totalPages = (int)System.Math.Ceiling(totalMovies / (double)PageSize);

            var movies = await moviesQuery
                .OrderBy(m => m.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(movies);
        }

        // 📂 صفحة تعرض أفلام فئة معينة
        // 📂 صفحة تعرض أفلام فئة معينة مع Pagination
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            const int PageSize = 4;

            var query = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Where(m => m.CategoryId == id);

            var totalMovies = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalMovies / (double)PageSize);

            var movies = await query
                .OrderBy(m => m.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.CategoryName = (await _context.Categories.FindAsync(id))?.Name ?? "Unknown";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(movies);
        }

        // 🎥 التفاصيل
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .Include(m => m.MovieSubImages)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
                return NotFound();

            return View(movie);
        }
    }
}
