using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Data;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class HomeMovieController : Controller
    {
        IMovieRepo MovieRepo;
        public HomeMovieController(IMovieRepo movieRepo)
        {
            MovieRepo = movieRepo;
        }

        public IActionResult Index()
        {
            var movies = MovieRepo.Get(Includation: [e => e.category, e => e.cinema]).ToList();
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
    }
}
