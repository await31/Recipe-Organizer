﻿using CapstoneProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartBreadcrumbs.Nodes;
using System;
using Firebase.Auth;

namespace CapstoneProject.Controllers {

    [DefaultBreadcrumb]
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public HomeController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            //_logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index() {

            var recipes = _context.Recipes
                            .Where(a=> a.Status==true)
                            .OrderByDescending(b => b.CreatedDate)
                            .Take(6)
                            .Include(r => r.FkRecipeCategory)
                            .ToList();

            if (recipes != null) {
                var hotRecipe = _context.Recipes
                            .Where(a=> a.Status == true)
                            .Include(r => r.FkRecipeCategory)
                            .Include(b => b.FkUser)
                            .OrderByDescending(a => a.ViewCount)
                            .FirstOrDefault();
                ViewData["HotRecipe"] = hotRecipe;
            }

            //Favourite list
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
                ViewBag.FavouriteList = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.Select(f => new { f.Id, f.Name }).ToList();
            else
                ViewBag.FavouriteList = null;

            return View(recipes);
        }

        [Breadcrumb("Privacy", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Privacy() {
            return View();
        }


        [Breadcrumb("View Ingredients", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult ViewIngredient(int id, int pg = 1) {

            IEnumerable<Ingredient> obj = _context.Ingredients
                .Where(i => i.Status == true)
                .Where(i => i.FkCategoryId == id).ToList();


            const int pageSize = 10; // Number of ingredients in 1 page

            if (pg < 1)
                pg = 1;

            int recsCount = obj.Count();

            var pager = new Pager(recsCount, pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data = obj.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);
        }
        public async Task<IActionResult> FavouriteList() {
            var currentUser = await _userManager.GetUserAsync(User);
            var userFavouriteList = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.ToList();
            List<int> recipeIds = _context.Favourites.Where(a => userFavouriteList.Contains(a)).Include(a => a.Recipes).SelectMany(c => c.Recipes).Select(r => r.Id).ToList();

            var recipes = await _context.Recipes
                .Where(a=>a.Status == true)
                .Where(r => recipeIds.Contains(r.Id))
                .Include(r => r.FkRecipe)
                .Include(r => r.FkRecipeCategory)
                .Include(r => r.FkUser).ToListAsync();
            return View(recipes);
        }
        public async Task<IActionResult> Favorite(int? id, int? favouriteId, string returnUrl, string parameters) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == id);
            if (entity != null) {
                var currentUser = await _userManager.GetUserAsync(User);
                var userFavouriteId = _context.Accounts.Include(u => u.Favourites).FirstOrDefault(u => u.Id == currentUser.Id).Favourites.ToArray();
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
            return LocalRedirect(returnUrl + parameters);
        }

        public async Task<IActionResult> IngredientDetails(int? id, int? ingredientId) {
            var childNode1 = new MvcBreadcrumbNode("ViewIngredient", "Home", "View Ingredients", false) {
                RouteValues = new { id } //this comes in as a param into the action
            };
            var childNode2 = new MvcBreadcrumbNode("IngredientDetails", "Home", "Ingredient Details") {
                OverwriteTitleOnExactMatch = true,
                Parent = childNode1
            };

            if (id == null || _context.Ingredients == null) {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(b => b.FkCategory)
                .FirstOrDefaultAsync(m => m.Id == ingredientId);
            if (ingredient == null) {
                return NotFound();
            }

            ViewData["BreadcrumbNode"] = childNode2;
            return View(ingredient);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}