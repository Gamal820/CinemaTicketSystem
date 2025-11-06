using CinemaTicketSystem.DataAccess;
using CinemaTicketSystem.Models;
using CinemaTicketSystem.ViewModels;
using CinemaTicketSystem.Repositories;  
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.IO;
using CinemaTicketSystem.Repositories.IRepositories;
using CinemaTicketSystem.Utitlies;
using Microsoft.AspNetCore.Authorization;

namespace CinemaTicketSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE},{SD.EMPLOYEE_ROLE},")]
    public class MoviesController : Controller
    {
        ApplicationDBContext _context; //= new();
        private readonly IRepository<Movie> _movieRepository;//=new MovieRepository() ;

        private readonly IRepository<Category> _categoryRepository;//= new MovieRepository();

        private readonly IRepository<Cinema> _cinemaRepository;//= new MovieRepository();
        private readonly IRepository<Actor> _actorRepository;//= new MovieRepository();
        private readonly IMovieActorRepository _movieActorRepository;//= new MovieRepository();
        private readonly IRepository<MovieSubImage> _movieSubImageRepository;//= new MovieRepository();

        public MoviesController(ApplicationDBContext context, IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository, 
            IMovieActorRepository movieActorRepository, IRepository<MovieSubImage> movieSubImageRepository)
        {
            _context = context;
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieActorRepository = movieActorRepository;
            _movieSubImageRepository = movieSubImageRepository;
        }




        // لا حاجة لـ Constructor الآن لتهيئة الـ Context أو Repositories

        // ================= INDEX ==================
        public async Task<IActionResult> Index(string? name, int? categoryId, int? cinemaId, int page = 1)
        {
            // نستخدم _context مباشرة للـ IQueryable المعقدة
            var moviesQuery = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Cinema)
                .AsQueryable();

            #region Filters
            if (!string.IsNullOrWhiteSpace(name))
            {
                moviesQuery = moviesQuery.Where(m => m.Name.Contains(name.Trim()));
                ViewBag.name = name;
            }

            if (categoryId is not null)
            {
                moviesQuery = moviesQuery.Where(m => m.CategoryId == categoryId);
                ViewBag.categoryId = categoryId;
            }

            if (cinemaId is not null)
            {
                moviesQuery = moviesQuery.Where(m => m.CinemaId == cinemaId);
                ViewBag.cinemaId = cinemaId;
            }

            ViewBag.Categories = (await _categoryRepository.GetAsync()).ToList();
            ViewBag.Cinemas = (await _cinemaRepository.GetAsync()).ToList();
            #endregion

            #region Pagination
            const int pageSize = 4;
            int totalMovies = await moviesQuery.CountAsync(); // استخدام Async
            ViewBag.TotalPages = Math.Ceiling(totalMovies / (double)pageSize);
            ViewBag.CurrentPage = page;
            #endregion

            var movies = await moviesQuery
                .OrderBy(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieVM
                {
                    Id = m.Id,
                    Name = m.Name,
                    CategoryName = m.Category.Name,
                    CinemaName = m.Cinema.Name,
                    MainImg = string.IsNullOrEmpty(m.MainImg) ? "no-image.jpg" : m.MainImg,
                    Price = m.Price,
                    Status = m.Status
                })
                .ToListAsync();

            return View(movies);
        }

        // ================= CREATE (GET) ==================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new MovieVM
            {
                Categories = (await _categoryRepository.GetAsync()).ToList(),
                Cinemas = (await _cinemaRepository.GetAsync()).ToList(),
                Actors = (await _actorRepository.GetAsync()).ToList()
            };

            return View(vm);
        }

        // ================= CREATE (POST) ==================
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile? MainImgFile, List<IFormFile>? SubImgs, List<int>? SelectedActors)
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
                        await MainImgFile.CopyToAsync(stream);
                    }

                    movie.MainImg = fileName;
                }
                #endregion

                await _movieRepository.AddAsync(movie);
                await _movieRepository.CommitAsync();

                #region Sub Images
                if (SubImgs != null && SubImgs.Any())
                {
                    var subImagesList = new List<MovieSubImage>();
                    foreach (var item in SubImgs)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await item.CopyToAsync(stream);
                        }

                        subImagesList.Add(new MovieSubImage
                        {
                            MovieId = movie.Id,
                            Img = fileName
                        });
                    }
                    _context.MovieSubImages.AddRange(subImagesList);
                    await _movieSubImageRepository.CommitAsync();
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
                    await _movieActorRepository.CommitAsync();
                }
                #endregion

                await transaction.CommitAsync();
                TempData["success-notification"] = "Movie added successfully!";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["error-notification"] = "Error while saving movie.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT (GET) ==================
        [HttpGet]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(int id)
        {
            // استخدام GetOneAsync من الـ Repository
            var movie = await _movieRepository.GetOneAsync(
                expression: m => m.Id == id,
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.MovieActors,
                    m => m.MovieSubImages
                });

            if (movie == null)
                return RedirectToAction("NotFoundPage", "Home");

            var vm = new MovieVM
            {
                Movie = movie,
                Categories = (await _categoryRepository.GetAsync()).ToList(),
                Cinemas = (await _cinemaRepository.GetAsync()).ToList(),
                Actors = (await _actorRepository.GetAsync()).ToList(),
                SelectedActors = movie.MovieActors.Select(a => a.ActorId).ToList()
            };

            return View(vm);
        }

        // ================= EDIT (POST) ==================
        [HttpPost]
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? MainImgFile, List<IFormFile>? SubImgs, List<int>? SelectedActors)
        {
            // استخدام GetOneAsync من الـ Repository
            var movieInDb = await _movieRepository.GetOneAsync(
                expression: m => m.Id == movie.Id,
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.MovieActors,
                    m => m.MovieSubImages
                });

            if (movieInDb == null)
                return RedirectToAction("NotFoundPage", "Home");

            // نحدّث باقي القيم (لا يمكن استخدام SetValues إلا مع Context مباشر)
            _context.Entry(movieInDb).CurrentValues.SetValues(movie);

            #region Update Main Image
            if (MainImgFile != null && MainImgFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(movieInDb.MainImg))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movieInDb.MainImg);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var fileName = Guid.NewGuid() + Path.GetExtension(MainImgFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await MainImgFile.CopyToAsync(stream);
                }

                movieInDb.MainImg = fileName;
            }
            else
            {
                movieInDb.MainImg = movieInDb.MainImg;
            }
            #endregion

            _movieRepository.Update(movieInDb, CancellationToken.None);
            await _movieRepository.CommitAsync();

            #region Sub Images
            if (SubImgs != null && SubImgs.Any())
            {
                foreach (var item in SubImgs)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await item.CopyToAsync(stream);
                    }

                    _context.MovieSubImages.Add(new MovieSubImage
                    {
                        Img = fileName,
                        MovieId = movie.Id
                    });
                }
                await _movieSubImageRepository.CommitAsync();
            }
            #endregion

            #region MovieActors
            _context.MovieActors.RemoveRange(movieInDb.MovieActors);
            await _movieActorRepository.CommitAsync();

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
                await _movieActorRepository.CommitAsync();
            }
            #endregion

            TempData["success-notification"] = "Movie updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE ==================
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(
                expression: m => m.Id == id,
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.MovieSubImages
                });

            if (movie == null)
                return RedirectToAction("NotFoundPage", "Home");

            // حذف الملفات (عملية نظام ملفات)
            var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies", movie.MainImg);
            if (System.IO.File.Exists(mainPath))
                System.IO.File.Delete(mainPath);

            foreach (var item in movie.MovieSubImages)
            {
                var subPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", item.Img);
                if (System.IO.File.Exists(subPath))
                    System.IO.File.Delete(subPath);
            }

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();

            TempData["success-notification"] = "Movie deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE SUB IMAGE ==================
        [Authorize(Roles = $"{SD.SUPER_ADMIN_ROLE},{SD.ADMIN_ROLE}")]
        public async Task<IActionResult> DeleteSubImg(int movieId, string img)
        {
            if (string.IsNullOrEmpty(img))
                return RedirectToAction(nameof(Edit), new { id = movieId });

            var subImg = await _movieSubImageRepository.GetOneAsync(s => s.MovieId == movieId && s.Img == img);

            if (subImg == null)
                return RedirectToAction(nameof(Edit), new { id = movieId });

            // حذف الملف
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/sub", subImg.Img);
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            _movieSubImageRepository.Delete(subImg);
            await _movieSubImageRepository.CommitAsync();

            return RedirectToAction(nameof(Edit), new { id = movieId });
        }
    }
}