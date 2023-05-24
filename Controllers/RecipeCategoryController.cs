using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProject.Controllers {
    public class RecipeCategoryController : Controller {

        private readonly RecipeOrganizerContext _context;

        public RecipeCategoryController(RecipeOrganizerContext context) {
            _context = context;
        }

        public IActionResult Index() {
            IEnumerable<RecipeCategory> objRecipeCategoryList = _context.RecipeCategories.ToList();
            return View(objRecipeCategoryList);
        }


        //GET
        public IActionResult Create() {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RecipeCategory obj) {
            if (ModelState.IsValid) {
                _context.RecipeCategories.Add(obj);
                _context.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Edit(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }
            var categoryFromDb = _context.RecipeCategories.Find(id);
            if (categoryFromDb == null) {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RecipeCategory obj) {
            if (ModelState.IsValid) {
                _context.RecipeCategories.Update(obj);
                _context.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View(obj);
        }

        //GET
        public IActionResult Delete(int? id) {
            if (id == null || id == 0) {
                return NotFound();
            }
            var categoryFromDb = _context.RecipeCategories.Find(id);
            if (categoryFromDb == null) {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) {

            var obj = _context.RecipeCategories.Find(id);
            if (obj == null) {
                return NotFound();
            }
            _context.RecipeCategories.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
