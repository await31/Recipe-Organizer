using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

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
                            .OrderByDescending(b => b.CreatedDate)
                            .Take(6)
                            .Include(r => r.FkRecipeCategory)
                            .ToList();

            if (recipes != null) {
                var hotRecipe = _context.Recipes
                            .Include(r => r.FkRecipeCategory)
                            .Include(b => b.FkUser)
                            .OrderByDescending(a => a.CreatedDate)
                            .FirstOrDefault();
                ViewData["HotRecipe"] = hotRecipe;
            }

            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.FavoriteList = null;

            if (currentUser != null) {
                var favorite = _context
                    .Favourites
                    .Include(item => item.Recipes)
                    .FirstOrDefault(b => b.FavouriteId == currentUser.FavouriteId);

                List<int> favoriteList = null;
                if (favorite != null) {
                    favoriteList = favorite.Recipes.Select(r => r.Id).ToList();
                }
                ViewBag.FavoriteList = favoriteList;

                return View(recipes);
            }
            return View(recipes);
        }

        [Breadcrumb("Privacy", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Privacy() {
            return View();
        }

        [Breadcrumb("Recipe Detail", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult RecipeDetail() {
            return View();
        }

        [Breadcrumb("View Ingredients", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult ViewIngredient(int id, int pg = 1) {

            IEnumerable<Ingredient> obj = _context.Ingredients.Where(i => i.FkCategoryId == id).ToList();

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
            var favorite = _context.Favourites.Include(item => item.Recipes).FirstOrDefault(b => b.FavouriteId == currentUser.FavouriteId);
            var recipeIds = favorite.Recipes.Select(r => r.Id).ToList();

            var recipes = await _context.Recipes
                .Where(r => recipeIds.Contains(r.Id))
                .Include(r => r.FkRecipe)
                .Include(r => r.FkRecipeCategory)
                .Include(r => r.FkUser).ToListAsync();
            return View(recipes);
        }

        public async Task<IActionResult> Favorite(int? id, string returnUrl, string parameters) {
            var entity = _context.Recipes.FirstOrDefault(item => item.Id == id);
            if (entity != null) {
                var currentUser = await _userManager.GetUserAsync(User);
                var favourite = _context.Favourites.Include(item => item.Recipes).FirstOrDefault(item => item.FavouriteId == currentUser.FavouriteId);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}