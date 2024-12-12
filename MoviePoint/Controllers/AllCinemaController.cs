using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Data;
using MoviePoint.Models;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class AllCinemaController : Controller
    {
        ICinemaRepo cinemaRepo;
        public AllCinemaController(ICinemaRepo cinemaRepo)
        {
            this.cinemaRepo = cinemaRepo;
        }

        public IActionResult Index()
        {
            var cinemas = cinemaRepo.Get().ToList();
            return View(model: cinemas);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cinema cinema, IFormFile file)
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
                    cinema.CinemaLogo = fileName;
                }
                cinemaRepo.Create(cinema);
                cinemaRepo.Attemp();
                TempData["CSuccess"] = "Cinema is successfully created";
                return RedirectToAction(nameof(Index));
            }
            return View(model: cinema);
        }
        public IActionResult Edit(int id)
        {
            var cinemaa = cinemaRepo.GetOne(filter: e => e.Id == id);
            return View(model: cinemaa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cinema cinema, IFormFile file)
        {
            ModelState.Remove("file");
            var OldProduct = cinemaRepo.GetOne(filter: e => e.Id == cinema.Id, tracked: false);
            var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", OldProduct.CinemaLogo);
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
                    cinema.CinemaLogo = fileName;
                }
                else
                {
                    cinema.CinemaLogo = OldProduct.CinemaLogo;
                }

                cinemaRepo.Edit(cinema);
                cinemaRepo.Attemp();
                TempData["CSuccess"] = "Cinema is successfully edited";
                return RedirectToAction(nameof(Index));
            }
            return View(model: cinema);
        }

        public IActionResult Delete(int id)
        {
            var cinema = cinemaRepo.GetOne(filter: e => e.Id == id);
            var OldProduct = cinemaRepo.GetOne(filter: e => e.Id == cinema.Id, tracked: false);
            var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", OldProduct.CinemaLogo);
            if (System.IO.File.Exists(OldPath))
            {
                System.IO.File.Delete(OldPath);
            }
            cinemaRepo.Delete(cinema);
            cinemaRepo.Attemp();
            TempData["CSuccess"] = "Cinema is successfully deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
