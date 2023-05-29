using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CapstoneProject.Controllers {
    [Authorize(Roles = "Admin")]
    public class IngredientCategoryController : Controller {

        private readonly RecipeOrganizerContext _context;

        public IngredientCategoryController(RecipeOrganizerContext context) {
            _context = context;
        }
        public IActionResult Index() {
            IEnumerable<IngredientCategory> objRecipeCategoryList = _context.IngredientCategories.ToList();
            return View(objRecipeCategoryList);
        }

        //GET
        public IActionResult Create() {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IngredientCategory obj) {
            if (ModelState.IsValid) {
                _context.IngredientCategories.Add(obj);
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
            var categoryFromDb = _context.IngredientCategories.Find(id);
            if (categoryFromDb == null) {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(IngredientCategory obj) {
            if (ModelState.IsValid) {
                _context.IngredientCategories.Update(obj);
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
            var categoryFromDb = _context.IngredientCategories.Find(id);
            if (categoryFromDb == null) {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        //POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id) {

            var obj = _context.IngredientCategories.Find(id);
            if (obj == null) {
                return NotFound();
            }
            _context.IngredientCategories.Remove(obj);
            _context.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
