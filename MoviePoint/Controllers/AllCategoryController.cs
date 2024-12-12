using Microsoft.AspNetCore.Mvc;
using MoviePoint.Data;
using MoviePoint.Models;
using MoviePoint.Repos.IRepos;

namespace MoviePoint.Controllers
{
    public class AllCategoryController : Controller
    {
        ICategoryRepo categoryRepo;

        public AllCategoryController(ICategoryRepo categoryRepo)
        {
            this.categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            return View(categoryRepo.Get().ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Create(category);
                categoryRepo.Attemp();
                TempData["Success"] = "Category is successfully created";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Edit(int id)
        {
            var category = categoryRepo.GetOne(filter: e => e.Id == id);
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                categoryRepo.Edit(category);
                categoryRepo.Attemp();
                TempData["Success"] = "Category is successfully edited";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Delete(int id)
        {
            var category = categoryRepo.GetOne(filter: e => e.Id == id);
            categoryRepo.Delete(category);
            categoryRepo.Attemp();
            TempData["Success"] = "Category is successfully deleted";
            return RedirectToAction(nameof(Index));
        }
    }
}
