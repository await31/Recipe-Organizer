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

            IEnumerable<Account> accounts = _context.Accounts
                .ToList();


            //Get counts of ingredients, recipes, and pending request
            ViewData["Contact"] = contact;
            ViewData["accCount"] = accounts.Count();
            ViewData["IngredientsCount"] = _context.Ingredients
                .Count();
            ViewData["RecipesCount"] = _context.Recipes
                .Where(a => a.Status == true)
                .Count();
            ViewData["PendingRequestsCount"] = objRecipe
                .Count();
            ViewData["IngredientsPending"] = pendingIngredients;

       
            var ingredientCounts = _context.Ingredients
                .Join(
                    _context.IngredientCategories,
                    ingredient => ingredient.FkCategoryId,
                    category => category.Id,
                    (ingredient, category) => new { Ingredient = ingredient, Category = category }
                )
                .GroupBy(
                    pair => new { CategoryName = pair.Category.Name },
                    (key, group) => new
                    {
                        CategoryName = key.CategoryName,
                        IngredientCount = group.Count()
                    }
                )
                .ToList();

            var ingredientChartData = new List<Object>();
            foreach (var item in ingredientCounts)
            {
                ingredientChartData.Add(new { Label = $"{item.CategoryName}", Value = item.IngredientCount });
            }
            ViewBag.IngredientChartData = Newtonsoft.Json.JsonConvert.SerializeObject(ingredientChartData);


            var recipeCounts = _context.Recipes
                .Join(
                    _context.RecipeCategories,
                    recipe => recipe.FkRecipeCategoryId,
                    category => category.Id,
                    (recipe, category) => new { Recipe = recipe, Category = category }
                )
                .GroupBy(
                    pair => new { CategoryName = pair.Category.Name },
                    (key, group) => new
                    {
                        CategoryName = key.CategoryName,
                        RecipesCount = group.Count()
                    }
                )
                .ToList();
            var recipeChartData = new List<Object>();
            foreach (var item in recipeCounts)
            {
                recipeChartData.Add(new { Label = $"{item.CategoryName}", Value = item.RecipesCount });
            }
            ViewBag.RecipeChartData = Newtonsoft.Json.JsonConvert.SerializeObject(recipeChartData);

            return View(objRecipe);
        }

       
    }
}
