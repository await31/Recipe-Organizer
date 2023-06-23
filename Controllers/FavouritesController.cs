using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Models;
using Microsoft.AspNetCore.Identity;
using SmartBreadcrumbs.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace CapstoneProject.Controllers {

    public class FavouritesController : Controller {
        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public FavouritesController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<JsonResult> AddToFavourite(int favouriteId, int recipeId)
        {
            var favouriteList = _context.Favourites.Include(f => f.Recipes).FirstOrDefault(f => f.Id == favouriteId);
            var recipe = _context.Recipes.FirstOrDefault(f => f.Id == recipeId);
            favouriteList.Recipes.Add(recipe);
            await _context.SaveChangesAsync();
            return Json(true);
        }
        [HttpPost]
        public async Task<JsonResult> RemoveFromFavourite(int favouriteId,int recipeId)
        {
            var favouriteList= _context.Favourites.Include(f=>f.Recipes).FirstOrDefault(f=>f.Id==favouriteId);
            var recipe = _context.Recipes.FirstOrDefault(f => f.Id == recipeId);
            favouriteList.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return Json(true);
        }
        [HttpPost]
        public async Task<JsonResult> GetAllFavouriteRecipes(int[] recipeIds) {
            var favourites = new List<object>();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null) {
                foreach (var recipeId in recipeIds) {
                    var favorite = _context.Favourites.Where(a => a.Account == currentUser).Include(f => f.Recipes).SelectMany(c => c.Recipes).FirstOrDefault(f => f.Id == recipeId);

                    if (favorite != null) {
                        favourites.Add(new { recipeId = recipeId, isFavorite = true });
                    } else {
                        favourites.Add(new { recipeId = recipeId, isFavorite = false });
                    }
                }
            }

            return Json(favourites);
        }
        //This is for favourite's checkboxes
        [HttpPost]
        public async Task<JsonResult> GetAllFavouriteLists(int recipeId, int[] favouriteIds) {
            var favourites = new List<object>();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null) {
                foreach (var favouriteId in favouriteIds) {
                    var favorite = _context.Favourites.Where(a => a.Account == currentUser).Include(f => f.Recipes).FirstOrDefault(a => a.Id == favouriteId).Recipes.FirstOrDefault(f => f.Id == recipeId);

                    if (favorite != null) {
                        favourites.Add(new { id = favouriteId, isFavorite = true });
                    } else {
                        favourites.Add(new { id = favouriteId, isFavorite = false });
                    }
                }
            }

            return Json(favourites);
        }
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> AddRecipe(int recipeId, int[] favouriteIds, int[] allfavouriteIds) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == recipeId);
            if (entity != null) {
                foreach (var favouriteId in allfavouriteIds) {
                    var favourite = _context.Favourites.Include(a => a.Recipes).FirstOrDefault(a => a.Id == favouriteId);

                    if ((favouriteIds.Contains(favouriteId)) && (!favourite.Recipes.Contains(entity))) {
                        favourite.Recipes.Add(entity);
                        //TempData["success"] = "Add to favourites successfully";
                    } else
                    if (!(favouriteIds.Contains(favouriteId)) && (favourite.Recipes.Contains(entity))) {
                        favourite.Recipes.Remove(entity);
                        //TempData["success"] = "Removed from favourites successfully";
                    }
                }
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }
        // GET: Favourites
        [Breadcrumb("My Collections")]
        [Authorize]
        public async Task<IActionResult> Index() {
            var currentUser = await _userManager.GetUserAsync(User);
            var userFavourite = _context.Accounts
                .Include(u => u.Favourites)
                .ThenInclude(f => f.Recipes)
                .FirstOrDefault(u => u.Id == currentUser.Id)
                .Favourites.ToList();
            return View(userFavourite);
        }

        // GET: Favourites/Details/5
        [Breadcrumb("Collection Details")]
        public async Task<IActionResult> Details(int? id) {

            var favouriteList = _context.Favourites
                .Include(a => a.Account)
                .Include(a => a.Recipes)
                .ThenInclude(recipe => recipe.FkRecipeCategory)
                .FirstOrDefault(a => a.Id == id);

            ViewData["Name"] = favouriteList?.Name;
            ViewData["Description"] = favouriteList?.Description;
            ViewData["IsPrivate"] = favouriteList?.isPrivate;
            ViewData["Id"] = id;
            if (favouriteList?.Account != null)
                ViewData["UserId"] = favouriteList.Account.Id;

            var recipes = favouriteList?.Recipes
                .ToList();

            return View(recipes);
        }

        // GET: Favourites/Create
        [Authorize]
        public IActionResult Create() {
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,isPrivate")] Favourite favourite) {
            favourite.Account = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid) {
                _context.Add(favourite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(favourite);
        }

        // GET: Favourites/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id) {
            if (id == null || _context.Favourites == null) {
                return NotFound();
            }

            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite == null) {
                return NotFound();
            }
            return View(favourite);
        }

        // POST: Favourites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<JsonResult> Edit(int id, string name, string description, bool isPrivate)
        {
            var favourite = _context.Favourites.FirstOrDefault(f => f.Id == id);
            try
            {
                favourite.Name = name;
                favourite.Description = description;
                favourite.isPrivate = isPrivate;
                _context.Update(favourite);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavouriteExists(favourite.Id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return Json(new { name = name, description = description, isPrivate= isPrivate });
        }

        // GET: Favourites/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id) {
            if (id == null || _context.Favourites == null) {
                return NotFound();
            }

            var favourite = await _context.Favourites
                .FirstOrDefaultAsync(m => m.Id == id);
            if (favourite == null) {
                return NotFound();
            }

            return View(favourite);
        }

        // POST: Favourites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            if (_context.Favourites == null) {
                return Problem("Entity set 'RecipeOrganizerContext.Favourites'  is null.");
            }
            var favourite = await _context.Favourites.FindAsync(id);
            if (favourite != null) {
                _context.Favourites.Remove(favourite);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FavouriteExists(int id) {
            return (_context.Favourites?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}