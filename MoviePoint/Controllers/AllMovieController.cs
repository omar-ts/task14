using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Data;
using MoviePoint.Models;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class AllMovieController : Controller
    {
        IMovieRepo movieRepo;

        public AllMovieController(IMovieRepo movieRepo)
        {
            this.movieRepo = movieRepo;
        }

        public IActionResult Index()
        {
            var movies = movieRepo.Get(Includation: [e => e.category, e => e.cinema]).ToList();
            foreach (var item in movies)
            {
                if (DateTime.Now > item.EndDate)
                {
                    item.Status = MovieStatus.Expired;
                }
                else if (DateTime.Now < item.StartDate)
                {
                    item.Status = MovieStatus.Upcoming;
                }
                else if (DateTime.Now >= item.StartDate && DateTime.Now <= item.EndDate)
                {
                    item.Status = MovieStatus.Available;
                }
            }
            return View(model: movies);
        }
        public IActionResult Create()
        {
            ViewBag.Categories = movieRepo.GetAll<Category>().ToList();
            ViewBag.Cinemas = movieRepo.GetAll<Cinema>().ToList();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, IFormFile file)
        {
            ModelState.Remove("file");
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    movie.ImgUrl = fileName;
                }
                movieRepo.Create(movie);
                movieRepo.Attemp();
                TempData["MSuccess"] = "Movie is successfully created";
                return RedirectToAction(nameof(Index));
            }
            return View(model: movie);
        }
        public IActionResult Edit(int id)
        {
            var moviee = movieRepo.GetOne(e => e.Id == id);
            ViewBag.Categories = movieRepo.GetAll<Category>().ToList();
            ViewBag.Cinemas = movieRepo.GetAll<Cinema>().ToList();
            return View(model: moviee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile file)
        {
            ModelState.Remove("file");
            ViewBag.Categories = movieRepo.GetAll<Category>().ToList();
            ViewBag.Cinemas = movieRepo.GetAll<Cinema>().ToList();
            var OldProduct = movieRepo.GetOne(filter: e => e.Id == movie.Id, tracked: false);
            var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", OldProduct.ImgUrl);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }
                    if (System.IO.File.Exists(OldPath))
                    {
                        System.IO.File.Delete(OldPath);
                    }
                    movie.ImgUrl = fileName;
                }
                else
                {
                    movie.ImgUrl = OldProduct.ImgUrl;
                }

                movieRepo.Edit(movie);
                movieRepo.Attemp();
                TempData["MSuccess"] = "Movie is successfully edited";
                return RedirectToAction(nameof(Index));
            }
            return View(model: movie);
        }

        public IActionResult Delete(int id)
        {
            var movie = movieRepo.GetOne(e => e.Id == id);
            var OldProduct = movieRepo.GetOne(filter: e => e.Id == movie.Id, tracked: false);
            var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", OldProduct.ImgUrl);
            if (System.IO.File.Exists(OldPath))
            {
                System.IO.File.Delete(OldPath);
            }
            movieRepo.Delete(movie);
            movieRepo.Attemp();
            TempData["MSuccess"] = "Movie is successfully deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
