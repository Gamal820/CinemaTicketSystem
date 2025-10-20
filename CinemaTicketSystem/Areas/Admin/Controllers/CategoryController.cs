using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CategoryController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var categories = _context.Categories
                                     .AsNoTracking()
                                     .AsQueryable();

            return View(categories.AsEnumerable());
        }

         
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            TempData["success-notification"] = "Category added successfully!";
            return RedirectToAction(nameof(Index));
        }

       
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (category is null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(category);
        }
 
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            _context.Categories.Update(category);
            _context.SaveChanges();

            TempData["success-notification"] = "Category updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);

            if (category is null)
                return RedirectToAction("NotFoundPage", "Home");

            _context.Categories.Remove(category);
            _context.SaveChanges();

            TempData["success-notification"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
