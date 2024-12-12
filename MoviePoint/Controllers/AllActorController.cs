using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviePoint.Data;
using MoviePoint.Models;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class AllActorController : Controller
    {
        IActorRepo actorRepo;
        public AllActorController(IActorRepo actorRepo)
        {
            this.actorRepo = actorRepo;
        }

        public IActionResult Index()
        {
            var actor = actorRepo.Get().ToList();
            return View(model: actor);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Actor actor, IFormFile file)
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
                    actor.ProfilePicture = fileName;
                }
                actorRepo.Create(actor);
                actorRepo.Attemp();
                TempData["ASuccess"] = "Actor is successfully created";
                return RedirectToAction(nameof(Index));
            }
            return View(model: actor);
        }
        public IActionResult Edit(int id)
        {
            var Actorr = actorRepo.GetOne(filter:e=>e.Id==id);
            return View(model: Actorr);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Actor actor, IFormFile file)
        {
            ModelState.Remove("file");
            var OldProduct = actorRepo.GetOne(filter: e => e.Id == actor.Id, tracked: false);
            var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", OldProduct.ProfilePicture);
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
                    actor.ProfilePicture = fileName;
                }
                else
                {
                    actor.ProfilePicture = OldProduct.ProfilePicture;
                }

                actorRepo.Edit(actor);
                actorRepo.Attemp();
                TempData["ASuccess"] = "Actor is successfully edited";
                return RedirectToAction(nameof(Index));
            }
            return View(model: actor);
        }
        public IActionResult Delete(int id)
        {
            var actor = actorRepo.GetOne(filter: e => e.Id == id);
            var OldProduct = actorRepo.GetOne(filter: e => e.Id == actor.Id, tracked: false);
            var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", OldProduct.ProfilePicture);
            if (System.IO.File.Exists(OldPath))
            {
                System.IO.File.Delete(OldPath);
            }
            actorRepo.Delete(actor);
            actorRepo.Attemp();
            TempData["ASuccess"] = "Actor is successfully deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
