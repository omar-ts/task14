using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Data;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class FindPageController : Controller
    {
        IMovieRepo movieRepo;
        public FindPageController(IMovieRepo movieRepo)
        {
            this.movieRepo = movieRepo;
        }

        public IActionResult Index(string MovieName)
        {
            var movies = movieRepo.Get(Includation: [e => e.category, e => e.cinema], filter: e => e.Name.Contains(MovieName)).ToList();
            if (!movies.Any())
            {
                return RedirectToAction(nameof(NotFoundPage));
            }
            return View(movies);
        }
        public IActionResult NotFoundPage()
        {
            return View();
        }
    }
}
