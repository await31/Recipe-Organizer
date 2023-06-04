using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace CapstoneProject.Controllers {


    [Authorize]
    public class RecipesController : Controller {
        private readonly RecipeOrganizerContext _context;

        public RecipesController(RecipeOrganizerContext context) {
            _context = context;
        }

        // GET: Recipes
        public IActionResult Index(string SearchString) {
            //var recipeOrganizerContext = _context.Recipes.Include(r => r.FkRecipe).Include(r => r.FkRecipeCategory);
            //return View(await recipeOrganizerContext.ToListAsync());
            ViewData["CurrentFilter"] = SearchString;
            var recipes = from b in _context.Recipes
                          select b;
            if (!String.IsNullOrEmpty(SearchString)) {
                recipes = recipes.Where(b => b.Name.Contains(SearchString));
            }

            return View(recipes);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null || _context.Recipes == null) {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.FkRecipe)
                .Include(r => r.FkRecipeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null) {
                return NotFound();
            }

            return View(recipe);
        }

        // GET: Recipes/Create

        public IActionResult Create() {
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id");
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name");
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImgPath,Description,FkRecipeCategoryId,FkRecipeId,Nutrition,PrepTime,Difficult,FkUserId,Status,CreatedDate")] Recipe recipe) {
            if (ModelState.IsValid) {
                recipe.Status = false;
                int calories = Int32.Parse(Request.Form["Calories"]);
                int fat = Int32.Parse(Request.Form["Fat"]);
                int protein = Int32.Parse(Request.Form["Protein"]);
                int carbohydrate = Int32.Parse(Request.Form["Carbohydrate"]);
                int cholesterol = Int32.Parse(Request.Form["Cholesterol"]);
                recipe.Nutrition = $"Calories: {calories}kcal, Fat: {fat}g, Protein: {protein}g, Carbohydrate: {carbohydrate}g, Cholesterol: {cholesterol}mg";

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            return View(recipe);
        }
        [Authorize(Roles = "Admin")]
        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Recipes == null) {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) {
                return NotFound();
            }
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            return View(recipe);
        }
        [Authorize(Roles = "Admin")]
        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImgPath,Description,FkRecipeCategoryId,FkRecipeId,Nutrition,PrepTime,Difficult,FkUserId,Status,CreatedDate")] Recipe recipe) {
            if (id != recipe.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!RecipeExists(recipe.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            return View(recipe);
        }
        [Authorize(Roles = "Admin")]
        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null || _context.Recipes == null) {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.FkRecipe)
                .Include(r => r.FkRecipeCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null) {
                return NotFound();
            }

            return View(recipe);
        }
        [Authorize(Roles = "Admin")]
        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            if (_context.Recipes == null) {
                return Problem("Entity set 'RecipeOrganizerContext.Recipes'  is null.");
            }
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null) {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id) {
            return (_context.Recipes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        [Authorize(Roles = "Admin")]

        // POST: Recipes/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id) {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) {
                return NotFound();
            }

            recipe.Status = true; // Set the status to approved
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        // POST: Recipes/Deny
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deny(int id) {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) {
                return NotFound();
            }

            recipe.Status = false; // Set the status to denied
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}