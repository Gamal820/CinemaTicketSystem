using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE}")]
    public class PromotionController : Controller
    {
        private readonly ApplicationDBContext _context;

        public PromotionController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: Admin/Promotion
        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions.ToListAsync();
            return View(promotions);
        }

        // GET: Admin/Promotion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Promotion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Promotions.Add(promotion);
                await _context.SaveChangesAsync();
                TempData["success"] = "✅ Promotion created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        // GET: Admin/Promotion/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null) return NotFound();

            return View(promo);
        }

        // POST: Admin/Promotion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Promotions.Update(promotion);
                await _context.SaveChangesAsync();
                TempData["success"] = "✅ Promotion updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        // GET: Admin/Promotion/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null) return NotFound();

            _context.Promotions.Remove(promo);
            await _context.SaveChangesAsync();
            TempData["success"] = "🗑️ Promotion deleted!";
            return RedirectToAction(nameof(Index));
        }
        // GET: Admin/Promotion/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var promo = await _context.Promotions.FirstOrDefaultAsync(p => p.Id == id);

            if (promo == null)
                return NotFound();

            return View(promo);
        }

    }
}
