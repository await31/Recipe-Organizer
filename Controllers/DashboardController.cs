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
                .Include(a => a.FkUser)
                .Where(b=>b.Status == false)
                .ToList();

            IEnumerable<Ingredient> pendingIngredients = _context.Ingredients
                 .Where(a => a.Status == false)
                 .ToList();

            IEnumerable<Contact> contact = _context.Contacts
                .ToList();

            //Get counts of ingredients, recipes, and pending request
            ViewData["Contact"] = contact;
            ViewData["IngredientsCount"] = _context.Ingredients
                .Count();
            ViewData["RecipesCount"] = _context.Recipes
                .Where(a => a.Status == true)
                .Count();
            ViewData["PendingRequestsCount"] = objRecipe
                .Count();
            ViewData["IngredientsPending"] = pendingIngredients;
            return View(objRecipe);
        }
    }
}
