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

        public IActionResult Index() {
            var recipes = _context.Recipes.Include(r => r.FkRecipeCategory).ToList();
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
        public IActionResult ViewIngredient(int id, int pg=1) {

            IEnumerable<Ingredient> obj = _context.Ingredients.Where(i => i.FkCategoryId == id).ToList();

            const int pageSize = 10; // Number of ingredients in 1 page
            
            if (pg < 1) 
                pg = 1;

            int recsCount = obj.Count();

            var pager = new Pager(recsCount,pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data= obj.Skip(recSkip).Take(pager.PageSize).ToList();

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}