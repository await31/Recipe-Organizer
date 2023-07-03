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

            //chart 

            //Ingredient pie chart data
            var ingredientCounts = _context.Ingredients
                .Where(b => b.Status == true)
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

            //Recipe doughnut chart data
            var recipeCounts = _context.Recipes
                .Where(b => b.Status == true)
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

            //Recipe line chart daata

            var recipeMonth = _context.Recipes
                .Where(b => b.Status == true)
                .GroupBy(r => r.CreatedDate.Value.Month)
                 .Select(g => new
                 {
                     MonthNumber = g.Key,
                     RecipeCount = g.Count()
                 })
                .OrderBy(g => g.MonthNumber)
                .ToList();

            var recipeMonthChartData = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (var item in recipeMonth)
            {
                recipeMonthChartData[item.MonthNumber - 1] = item.RecipeCount;

            }
            var labels = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

            ViewBag.RecipeLineChartLabels = Newtonsoft.Json.JsonConvert.SerializeObject(labels);
            ViewBag.RecipeLineChartData = Newtonsoft.Json.JsonConvert.SerializeObject(recipeMonthChartData);
            return View(objRecipe);
        }
    }
}
