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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections;
using Microsoft.IdentityModel.Tokens;

namespace CapstoneProject.Controllers {

    public class UserRecipesController : Controller {
        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public UserRecipesController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        public JsonResult SearchAutoComplete(string term) {
            var result = (_context.Recipes.Where(t => t.Name.ToLower().Contains(term.ToLower()))
                 .Select(t => new { Name = t.Name }))
                 .ToList();
            return Json(result);
        }
        [HttpPost]
        public JsonResult IngredientsAutoComplete(string term) {
            var result = (_context.Ingredients.Where(t => t.Name.ToLower().Contains(term.ToLower()))
                 .Select(t => new { Name = t.Name }))
                 .ToList();
            return Json(result);
        }
        // GET: Recipes
        public async Task<IActionResult> Index() {
            var recipes = _context.Recipes.Include(b => b.Ingredients).Select(b => b);
            if (recipes != null) {
                string? searchString = Request.Query["SearchString"];
                string? prepTime = Request.Query["PrepTime"];
                string? recipeCategory = Request.Query["RecipeCategory"];
                string? difficulty = Request.Query["Difficulty"];
                string? sortBy = Request.Query["SortBy"];
                string? includeList = Request.Query["IncludeList"];
                string? excludeList = Request.Query["ExcludeList"];

                //Add all recipecategories
                if (String.IsNullOrEmpty(prepTime))
                    prepTime = "All";
                if (String.IsNullOrEmpty(difficulty))
                    difficulty = "All";
                if (String.IsNullOrEmpty(recipeCategory))
                    recipeCategory = "All";
                if (String.IsNullOrEmpty(sortBy))
                    sortBy = "SortPopular";
                ViewBag.FkRecipeCategoryId = new SelectList(_context.RecipeCategories, "Id", "Name", recipeCategory);
                ViewBag.SortBy = new SelectList(
                new List<SelectListItem>
                {
                new SelectListItem { Text = "Sort By Popularity", Value = "SortPopular"},
                new SelectListItem { Text = "Sort By Name", Value = "SortName"},
                new SelectListItem { Text = "Sort By Date", Value = "SortDate"},
                new SelectListItem { Text = "Sort By PrepTime", Value = "SortPrepTime"},
                }
                , "Value", "Text", sortBy);

                //View Data
                ViewData["FilterSearch"] = searchString;
                ViewData["FilterPrepTime"] = prepTime;
                ViewData["FilterDifficulty"] = difficulty;
                ViewData["FilterIncludeList"] = includeList;
                ViewData["FilterExcludeList"] = excludeList;
                //Include Ingr
                if (!prepTime.Equals("All")) {
                    recipes = recipes.Where(b => b.PrepTime <= int.Parse(prepTime));
                }
                if (!difficulty.Equals("All")) {
                    recipes = recipes.Where(b => b.Difficult == int.Parse(difficulty));
                }
                if (!recipeCategory.Equals("All")) {
                    recipes = recipes.Where(b => b.FkRecipeCategoryId == int.Parse(recipeCategory));
                }
                if (!String.IsNullOrEmpty(searchString)) {
                    recipes = recipes.Where(b => b.Name.ToLower().Contains(searchString.ToLower()));
                }
                //Sort
                switch (sortBy) {
                    case "SortDate":
                        recipes = recipes.OrderBy(b => b.CreatedDate);
                        break;
                    case "SortPrepTime":
                        recipes = recipes.OrderBy(b => b.PrepTime);
                        break;
                    case "SortName":
                        recipes = recipes.OrderBy(b => b.Name);
                        break;
                    default: // Sort by most popular
                        recipes = recipes.OrderByDescending(b => b.ViewCount);
                        break;
                }
                if (!String.IsNullOrEmpty(includeList)) {
                    var ingredientsList = includeList.Split(',');
                    ViewBag.IncludeList = ingredientsList;
                    foreach (var item in ingredientsList) {
                        recipes = recipes.Where(b => b.Ingredients.Any(i => i.Name.Equals(item)));
                    }
                } else
                    ViewBag.IncludeList = new List<String>();
                //Exclude
                if (!String.IsNullOrEmpty(excludeList)) {
                    var ingredientsList = excludeList.Split(',');
                    ViewBag.ExcludeList = ingredientsList;
                    foreach (var item in ingredientsList) {
                        recipes = recipes.Where(b => b.Ingredients.All(i => !i.Name.Equals(item)));
                    }
                } else
                    ViewBag.ExcludeList = new List<String>();
                //Favorite
                var currentUser = await _userManager.GetUserAsync(User);
                ViewBag.FavoriteList = null;
                if (currentUser != null) {
                    var favorite = _context.Favourites.Include(item => item.Recipes).FirstOrDefault(b => b.FavouriteId == currentUser.FavouriteId);
                    List<int> favoriteList = null;
                    if (favorite != null) {
                        favoriteList = favorite.Recipes.Select(r => r.Id).ToList();
                    }
                    ViewBag.FavoriteList = favoriteList;
                }
            }
            return View(recipes);
        }
        [HttpPost]
        public IActionResult Filter(string searchString, string recipeCategory, string prepTime, string difficulty,
            string sortBy, string includeIngredients, string includeIngredientsList, string excludeIngredients, string excludeIngredientsList) {
            string? includeList = includeIngredientsList;
            string? excludeList = excludeIngredientsList;
            if (!String.IsNullOrEmpty(includeIngredients)) {
                //Remove from exclude list if it has the include ingre inputed
                if (!String.IsNullOrEmpty(excludeList)) {
                    if (excludeList.IndexOf(includeIngredients, StringComparison.OrdinalIgnoreCase) != -1)
                        excludeList = RemoveFromStringWithComma(includeIngredients, excludeList);
                }
                if (String.IsNullOrEmpty(includeList)) {
                    includeList = includeIngredients.Trim();
                } else
                    includeList += "," + includeIngredients.Trim();
            }
            if (!String.IsNullOrEmpty(excludeIngredients)) {
                //Remove from include list if it has the exclude ingre inputed
                if (!String.IsNullOrEmpty(includeList)) {
                    if (includeList.IndexOf(excludeIngredients, StringComparison.OrdinalIgnoreCase) != -1)
                        includeList = RemoveFromStringWithComma(excludeIngredients, includeList);
                }
                if (String.IsNullOrEmpty(excludeList)) {
                    excludeList = excludeIngredients.Trim();
                } else
                    excludeList += "," + excludeIngredients.Trim();
            }
            var parameter = new RouteValueDictionary
            {
                { "SearchString", searchString },
                { "RecipeCategory", recipeCategory },
                { "PrepTime", prepTime },
                { "Difficulty", difficulty },
                { "SortBy", sortBy },
                { "IncludeList", includeList },
                { "ExcludeList", excludeList }
            };
            return RedirectToAction("Index", parameter);
        }
        public async Task<IActionResult> Favorite(int? id, string returnUrl, string parameters) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == id);
            if (entity != null) {
                var currentUser = await _userManager.GetUserAsync(User);
                var favourite = _context.Favourites.Include(item => item.Recipes).FirstOrDefault(item => item.FavouriteId == currentUser.FavouriteId);
                if (!favourite.Recipes.Contains(entity))
                    favourite.Recipes.Add(entity);
                else
                    favourite.Recipes.Remove(entity);

                await _context.SaveChangesAsync();
            }

            return LocalRedirect(returnUrl + parameters);
        }
        private string RemoveFromStringWithComma(string remove, string full) {
            full = full.Replace("," + remove, "");
            full = full.Replace(remove + ",", "");
            full = full.Replace(remove, "");
            return full;
        }
        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == id);
            if (entity != null) {
                entity.ViewCount++;
                await _context.SaveChangesAsync();
            }

            ViewBag.Title = "Create recipe";
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
        public async Task<IActionResult> Create([Bind("Id,Name,ImgPath,Description,FkRecipeCategoryId,FkRecipeId,Nutrition,PrepTime,Difficult,FkUserId,CreatedDate")] Recipe recipe) {
            if (ModelState.IsValid) {
                if (recipe.CreatedDate == null)
                    recipe.CreatedDate = DateTime.Now;
                if (recipe.ViewCount == null)
                    recipe.ViewCount = 0;
                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FkRecipeId"] = new SelectList(_context.Recipes, "Id", "Id", recipe.FkRecipeId);
            ViewData["FkRecipeCategoryId"] = new SelectList(_context.RecipeCategories, "Id", "Name", recipe.FkRecipeCategoryId);
            return View(recipe);
        }

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

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImgPath,Description,FkRecipeCategoryId,FkRecipeId,Nutrition,PrepTime,Difficult,FkUserId,CreatedDate")] Recipe recipe) {
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

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            ViewBag.Title = "Create recipe";
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
    }
}