using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
using System.Data;

namespace CapstoneProject.Controllers {

    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller {

        private readonly RecipeOrganizerContext _context;

        public DashboardController(RecipeOrganizerContext context) {
            _context = context;
        }

        [Breadcrumb("Dashboard")]
        public IActionResult Index() {
            IEnumerable<Recipe> objRecipe = _context.Recipes
                .Where(b=>b.Status == false)
                .ToList();

            //Get counts of ingredients, recipes, and pending request
            ViewData["IngredientsCount"] = _context.Ingredients
                .Count();
            ViewData["RecipesCount"] = _context.Recipes
                .Where(a => a.Status == true)
                .Count();
            ViewData["PendingRequestsCount"] = objRecipe
                .Count();
            return View(objRecipe);
        }
    }
}
