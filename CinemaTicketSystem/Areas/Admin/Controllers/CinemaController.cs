using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.Repositories;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Utitlies;
using CinemaTicketSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE},")]
    public class CinemaController : Controller
    {
        //Repository<Cinema> _cinemaRepository = new();
      private readonly  IRepository<Cinema> _cinemaRepository;//=new Repository<Cinema> () ;
        public CinemaController(IRepository<Cinema> cinemarepository)
        {
            _cinemaRepository= cinemarepository;
        }
        public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
        {
            int pageSize = 3; // عدد السينمات في كل صفحة
            if (page < 1) page = 1;

            var allCinemas = await _cinemaRepository.GetAsync(tracked: false, cancellationToken: cancellationToken);
            int totalCinemas = allCinemas.Count();
            int totalPages = (int)Math.Ceiling((double)totalCinemas / pageSize);

            if (page > totalPages && totalPages > 0)
                page = totalPages;

            var cinemas = allCinemas
                .OrderBy(c => c.CinemaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CinemaVM
                {
                    CinemaId = c.CinemaId,
                    Name = c.Name,
                    Img = c.Img
                })
                .ToList();

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
        public async Task<IActionResult> Create(Cinema cinema, IFormFile? ImgFile, CancellationToken cancellationToken = default)
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
                    await ImgFile.CopyToAsync(stream);
                }

                cinema.Img = fileName;
            }

            await _cinemaRepository.AddAsync(cinema, cancellationToken);
            await _cinemaRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Cinema added successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.CinemaId == id, tracked: false, cancellationToken: cancellationToken);

            if (cinema is null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(cinema);
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? NewImg, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return View(cinema);
            }

            var cinemaInDb = await _cinemaRepository.GetOneAsync(c => c.CinemaId == cinema.CinemaId, tracked: false, cancellationToken: cancellationToken);
            if (cinemaInDb is null)
                return RedirectToAction("NotFoundPage", "Home");

            if (NewImg is not null && NewImg.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(NewImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CinemaImages", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await NewImg.CopyToAsync(stream);
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

            _cinemaRepository.Update(cinema,cancellationToken);
            await _cinemaRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Cinema updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var cinema = await _cinemaRepository.GetOneAsync(c => c.CinemaId == id, tracked: false, cancellationToken: cancellationToken);

            if (cinema is null)
                return RedirectToAction("NotFoundPage", "Home");

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CinemaImages", cinema.Img);
            if (System.IO.File.Exists(oldPath))
                System.IO.File.Delete(oldPath);

            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync(cancellationToken);

            TempData["success-notification"] = "Cinema deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
