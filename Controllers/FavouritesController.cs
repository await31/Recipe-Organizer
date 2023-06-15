using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CapstoneProject.Models;
using Microsoft.AspNetCore.Identity;

namespace CapstoneProject.Controllers {
    public class FavouritesController : Controller {
        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public FavouritesController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            _context = context;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<JsonResult> GetFavourites(int[] recipeIds) {
            var favourites = new List<object>();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null) {
                var userFavouriteList = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.ToList();

                foreach (var recipeId in recipeIds) {
                    var favorite = _context.Favourites.Where(a => userFavouriteList.Contains(a)).Include(f => f.Recipes).SelectMany(c => c.Recipes).FirstOrDefault(f => f.Id == recipeId);

                    if (favorite != null) {
                        favourites.Add(new { recipeId = recipeId, isFavorite = true });
                    } else {
                        favourites.Add(new { recipeId = recipeId, isFavorite = false });
                    }
                }
            }

            return Json(favourites);
        }
        [HttpPost]
        public async Task<JsonResult> AddRecipe(int recipeId, int favouriteId) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == recipeId);
            if (entity != null) {
                var currentUser = await _userManager.GetUserAsync(User);
                //Get list of user favourites's id
                var userFavouriteId = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.ToArray();
                //Temp value
                favouriteId = userFavouriteId[0].Id;
                //Get user from dbContext which include favorites
                var favourite = _context.Favourites.Include(a => a.Recipes).FirstOrDefault(a => a.Id == favouriteId);

                if (!favourite.Recipes.Contains(entity)) {
                    favourite.Recipes.Add(entity);
                    TempData["success"] = "Add to favourites successfully";
                } else {
                    favourite.Recipes.Remove(entity);
                    TempData["success"] = "Removed from favourites successfully";
                }
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }
        // GET: Favourites
        public async Task<IActionResult> Index() {
            return _context.Favourites != null ?
                        View(await _context.Favourites.ToListAsync()) :
                        Problem("Entity set 'RecipeOrganizerContext.Favourites'  is null.");
        }

        // GET: Favourites/Details/5
        public async Task<IActionResult> Details(int? id) {
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

        // GET: Favourites/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Favourites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Favourite favourite) {
            if (ModelState.IsValid) {
                _context.Add(favourite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(favourite);
        }

        // GET: Favourites/Edit/5
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Favourite favourite) {
            if (id != favourite.Id) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(favourite);
                    await _context.SaveChangesAsync();
                } catch (DbUpdateConcurrencyException) {
                    if (!FavouriteExists(favourite.Id)) {
                        return NotFound();
                    } else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(favourite);
        }

        // GET: Favourites/Delete/5
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