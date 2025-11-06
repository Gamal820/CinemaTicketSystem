using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Utitlies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
 using System.Threading;
using System.Threading.Tasks;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE},")]
    public class ActorController : Controller
    {

        private readonly IRepository<Actor> _actorRepository;
        public ActorController(IRepository<Actor> actorrepository)
        {
            _actorRepository = actorrepository;
        }
        public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
        {
            int pageSize = 6;
            if (page < 1) page = 1;

            var allActors = await _actorRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            int totalActors = allActors.Count();
            int totalPages = (int)Math.Ceiling((double)totalActors / pageSize);

            if (page > totalPages && totalPages > 0)
                page = totalPages;

            var actors = allActors
                .OrderBy(a => a.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
        public async Task<IActionResult> Create(Actor actor, IFormFile? ImgFile, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(actor);

            if (ImgFile != null && ImgFile.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(ImgFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImgFile.CopyToAsync(stream, cancellationToken);
                }

                actor.Img = fileName;
            }

            await _actorRepository.AddAsync(actor, cancellationToken);
            await _actorRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Actor added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetOneAsync(a => a.Id == id, tracked: false, cancellationToken: cancellationToken);
            if (actor == null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(actor);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]

        public async Task<IActionResult> Edit(Actor actor, IFormFile? NewImg, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(actor);

            var actorInDb = await _actorRepository.GetOneAsync(a => a.Id == actor.Id, tracked: false, cancellationToken: cancellationToken);
            if (actorInDb == null)
                return RedirectToAction("NotFoundPage", "Home");

            if (NewImg != null && NewImg.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(NewImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await NewImg.CopyToAsync(stream, cancellationToken);
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

            _actorRepository.Update(actor,cancellationToken);
            await _actorRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Actor updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var actor = await _actorRepository.GetOneAsync(a => a.Id == id, tracked: false, cancellationToken: cancellationToken);
            if (actor == null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ActorImages", actor.Img);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Actor deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
