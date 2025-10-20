using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MoviesController : Controller
    {
        private readonly ApplicationDBContext _context;

        public MoviesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // ================= INDEX ==================
        public IActionResult Index(string? name, int? categoryId, int? cinemaId, int page = 1)
        {
            var query = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            #region Filters
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(m => m.Name.Contains(name.Trim()));
                ViewBag.name = name;
            }

            if (categoryId is not null)
            {
                query = query.Where(m => m.CategoryId == categoryId);
                ViewBag.categoryId = categoryId;
            }

            if (cinemaId is not null)
            {
                query = query.Where(m => m.CinemaId == cinemaId);
                ViewBag.cinemaId = cinemaId;
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            #endregion

            #region Pagination
            const int pageSize = 4;
            int totalMovies = query.Count();
            ViewBag.TotalPages = Math.Ceiling(totalMovies / (double)pageSize);
            ViewBag.CurrentPage = page;
            #endregion

            var movies = query
                .OrderBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieVM
                {
                    Id = m.Id,
                    Name = m.Name,
                    CategoryName = m.Category.Name,
                    CinemaName = m.Cinema.Name,
                    //  لو مفيش صورة، استخدم واحدة افتراضية
                    MainImg = string.IsNullOrEmpty(m.MainImg) ? "no-image.jpg" : m.MainImg,
                    Price = m.Price,
                    Status = m.Status
                })
                .ToList();

            return View(movies);
        }

        // ================= CREATE (GET) ==================
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new MovieVM
            {
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                Actors = _context.Actors.ToList()
            };

            return View(vm);
        }

        // ================= CREATE (POST) ==================
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile? MainImgFile, List<IFormFile>? SubImgs, List<int>? SelectedActors)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                #region Main Image
                if (MainImgFile != null && MainImgFile.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(MainImgFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        MainImgFile.CopyTo(stream);
                    }

                    movie.MainImg = fileName;
                }
                #endregion

                _context.Movies.Add(movie);
                _context.SaveChanges();

                #region Sub Images
                if (SubImgs != null && SubImgs.Any())
                {
                    foreach (var item in SubImgs)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            item.CopyTo(stream);
                        }

                        _context.MovieSubImages.Add(new MovieSubImage
                        {
                            MovieId = movie.Id,
                            Img = fileName
                        });
                    }
                    _context.SaveChanges();
                }
                #endregion

                #region MovieActors
                if (SelectedActors != null && SelectedActors.Any())
                {
                    foreach (var actorId in SelectedActors)
                    {
                        _context.MovieActors.Add(new MovieActor
                        {
                            MovieId = movie.Id,
                            ActorId = actorId
                        });
                    }
                    _context.SaveChanges();
                }
                #endregion

                transaction.Commit();
                TempData["success-notification"] = "Movie added successfully!";
            }
            catch
            {
                transaction.Rollback();
                TempData["error-notification"] = "Error while saving movie.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT (GET) ==================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieSubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return RedirectToAction("NotFoundPage", "Home");

            var vm = new MovieVM
            {
                Movie = movie,
                Categories = _context.Categories.ToList(),
                Cinemas = _context.Cinemas.ToList(),
                Actors = _context.Actors.ToList(),
                SelectedActors = movie.MovieActors.Select(a => a.ActorId).ToList()
            };

            return View(vm);
        }

        // ================= EDIT (POST) ==================
        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile? MainImgFile, List<IFormFile>? SubImgs, List<int>? SelectedActors)
        {
            var movieInDb = _context.Movies
                .Include(m => m.MovieActors)
                .Include(m => m.MovieSubImages)
                .FirstOrDefault(m => m.Id == movie.Id);

            if (movieInDb == null)
                return RedirectToAction("NotFoundPage", "Home");

            //  نحدّث باقي القيم
            _context.Entry(movieInDb).CurrentValues.SetValues(movie);

            #region Update Main Image
            if (MainImgFile != null && MainImgFile.Length > 0)
            {
                // حذف الصورة القديمة
                if (!string.IsNullOrEmpty(movieInDb.MainImg))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movieInDb.MainImg);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                // رفع الصورة الجديدة
                var fileName = Guid.NewGuid() + Path.GetExtension(MainImgFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    MainImgFile.CopyTo(stream);
                }

                movieInDb.MainImg = fileName;
            }
            //  لو المستخدم ما رفعش صورة جديدة، نحتفظ بالقديمة
            else
            {
                movieInDb.MainImg = movieInDb.MainImg;
            }
            #endregion

            _context.SaveChanges();

            #region Sub Images
            if (SubImgs != null && SubImgs.Any())
            {
                foreach (var item in SubImgs)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        item.CopyTo(stream);
                    }

                    _context.MovieSubImages.Add(new MovieSubImage
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }
                _context.SaveChanges();
            }
            #endregion

            #region MovieActors
            _context.MovieActors.RemoveRange(movieInDb.MovieActors);
            if (SelectedActors != null)
            {
                foreach (var actorId in SelectedActors)
                {
                    _context.MovieActors.Add(new MovieActor
                    {
                        MovieId = movie.Id,
                        ActorId = actorId
                    });
                }
                _context.SaveChanges();
            }
            #endregion

            TempData["success-notification"] = "Movie updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE ==================
        public IActionResult Delete(int id)
        {
            var movie = _context.Movies
                .Include(m => m.MovieSubImages)
                .FirstOrDefault(m => m.Id == id);

            if (movie == null)
                return RedirectToAction("NotFoundPage", "Home");

            var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movie.MainImg);
            if (System.IO.File.Exists(mainPath))
                System.IO.File.Delete(mainPath);

            foreach (var item in movie.MovieSubImages)
            {
                var subPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", item.Img);
                if (System.IO.File.Exists(subPath))
                    System.IO.File.Delete(subPath);
            }

            _context.Movies.Remove(movie);
            _context.SaveChanges();

            TempData["success-notification"] = "Movie deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE SUB IMAGE ==================
        public IActionResult DeleteSubImg(int movieId, string img)
        {
            if (string.IsNullOrEmpty(img))
                return RedirectToAction(nameof(Edit), new { id = movieId });

            var subImg = _context.MovieSubImages.FirstOrDefault(s => s.MovieId == movieId && s.Img == img);

            if (subImg == null)
                return RedirectToAction(nameof(Edit), new { id = movieId });

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", subImg.Img);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _context.MovieSubImages.Remove(subImg);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = movieId });
        }
    }
}
