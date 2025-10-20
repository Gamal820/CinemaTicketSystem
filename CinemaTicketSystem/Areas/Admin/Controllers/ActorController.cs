using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly ApplicationDBContext _context;

        public ActorController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page = 1)
        {
            int pageSize = 6;
            if (page < 1) page = 1;

            int totalActors = _context.Actors.Count();
            int totalPages = (int)Math.Ceiling((double)totalActors / pageSize);

            if (page > totalPages && totalPages > 0)
                page = totalPages;

            var actors = _context.Actors
                .AsNoTracking()
                .OrderBy(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new Actor
                {
                    Id = a.Id,
                    Name = a.Name,
                    Img = a.Img,
                    Bio = a.Bio
                })
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor());
        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile? ImgFile)
        {
            if (!ModelState.IsValid)
                return View(actor);

            if (ImgFile != null && ImgFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImgFile.CopyTo(stream);
                }

                actor.Img = fileName;
            }

            _context.Actors.Add(actor);
            _context.SaveChanges();

            TempData["success-notification"] = "Actor added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor == null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(actor);
        }

        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile? NewImg)
        {
            if (!ModelState.IsValid)
                return View(actor);

            var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(a => a.Id == actor.Id);
            if (actorInDb == null)
                return RedirectToAction("NotFoundPage", "Home");

            if (NewImg != null && NewImg.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(NewImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    NewImg.CopyTo(stream);
                }

                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", actorInDb.Img);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);

                actor.Img = fileName;
            }
            else
            {
                actor.Img = actorInDb.Img;
            }

            _context.Actors.Update(actor);
            _context.SaveChanges();

            TempData["success-notification"] = "Actor updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor == null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", actor.Img);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            _context.Actors.Remove(actor);
            _context.SaveChanges();

            TempData["success-notification"] = "Actor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
