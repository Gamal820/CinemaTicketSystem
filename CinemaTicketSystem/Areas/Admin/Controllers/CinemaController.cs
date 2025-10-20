using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly ApplicationDBContext _context;

        public CinemaController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 3; // عدد السينمات في كل صفحة
            if (page < 1) page = 1;

            int totalCinemas = _context.Cinemas.Count();
            int totalPages = (int)Math.Ceiling((double)totalCinemas / pageSize);

            if (page > totalPages && totalPages > 0)
                page = totalPages;

            var cinemas = _context.Cinemas
                .AsNoTracking()
                .OrderBy(c => c.CinemaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CinemaTicketSystem.ViewModels.CinemaVM
                {
                    CinemaId = c.CinemaId,
                    Name = c.Name,
                    Img = c.Img
                })
                .ToList();

            // تمرير بيانات الصفحات للـ View
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(cinemas);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cinema());
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile? ImgFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            if (ImgFile is not null && ImgFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CinemaImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }

                cinema.Img = fileName;
            }

            _context.Cinemas.Add(cinema);
            _context.SaveChanges();

            TempData["success-notification"] = "Cinema added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.CinemaId == id);

            if (cinema is null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(cinema);
        }

        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? NewImg)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(c => c.CinemaId == cinema.CinemaId);
            if (cinemaInDb is null)
                return RedirectToAction("NotFoundPage", "Home");

            if (NewImg is not null && NewImg.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NewImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CinemaImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    NewImg.CopyTo(stream);
                }

                // حذف الصورة القديمة
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CinemaImages", cinemaInDb.Img);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                cinema.Img = fileName;
            }
            else
            {
                cinema.Img = cinemaInDb.Img;
            }

            _context.Cinemas.Update(cinema);
            _context.SaveChanges();

            TempData["success-notification"] = "Cinema updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(c => c.CinemaId == id);

            if (cinema is null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CinemaImages", cinema.Img);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();

            TempData["success-notification"] = "Cinema deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
