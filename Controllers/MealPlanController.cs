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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Collections;
using Microsoft.IdentityModel.Tokens;
using SmartBreadcrumbs.Attributes;
using NuGet.Packaging;
using Firebase.Auth;
using Firebase.Storage;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats;
using System.Text.Json;
using System.Drawing.Printing;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CapstoneProject.Controllers {

    [Authorize]
    public class MealPlanController : Controller {

        private readonly RecipeOrganizerContext _context;
        private readonly UserManager<Account> _userManager;

        public MealPlanController(RecipeOrganizerContext context, UserManager<Account> userManager) {
            _context = context;
            _userManager = userManager;
        }

        public JsonResult GetEvents() {
            var currentUser = _userManager.GetUserId(User);
            var mealplans = _context.MealPlans
                .Where(a => a.FkUserId == currentUser)
                .ToList();

            return Json(mealplans);
        }

        [HttpPost]
        public JsonResult RecipeNameExists(string recipe) {
            ExtractIntegerAndString(recipe, out int id, out string name);
            var exists = _context.Recipes?.Any(r => r.Id == id && r.Name.Equals(name)) ?? false;
            return Json(new { exists });
        }

        public static void ExtractIntegerAndString(string inputString, out int integer, out string stringValue) {
            Match integerMatch = Regex.Match(inputString, @"\#(\d+)");
            if (integerMatch.Success) {
                integer = int.Parse(integerMatch.Groups[1].Value);
            } else {
                integer = 0;
            }
            string strippedString = Regex.Replace(inputString, @"\(\#\d+\)", "").Trim();
            stringValue = strippedString;
        }

        [HttpPost]
        public JsonResult RecipesAutoComplete(string term) {
            var result = _context.Recipes
                .Where(t => t.Name.ToLower().Contains(term.ToLower()))
                .Select(t => new { Id = t.Id, Name = t.Name })
                .ToList();

            return Json(result);
        }

        [Breadcrumb("My Planning")]
        public IActionResult Index(int pg = 1) {
            var currentUser = _userManager.GetUserId(User);
            var mealPlans = _context.MealPlans
                .Where(a => a.FkUserId == currentUser)
            .ToList();

            return View(mealPlans);
        }

        [Breadcrumb("Schedule")]
        public IActionResult Schedule() {
            var currentUser = _userManager.GetUserId(User);
            var mealPlans = _context.MealPlans
                .Where(a => a.FkUserId == currentUser)
                .ToList();
            return View(mealPlans);
        }

        [Breadcrumb("Details")]
        public IActionResult Details(int? id) {

            var mealplan = _context.MealPlans
                .Include(r => r.FkUser)
                .Include(r => r.Recipes)
                .ThenInclude(r => r.RecipeIngredients)
                .ThenInclude(r => r.Ingredient)
                .FirstOrDefault(a => a.Id == id);

            if (mealplan == null) {
                return NotFound(); 
            }

            var mealPlanWithNutrition = _context.MealPlans
                .Include(r => r.Recipes)
                .ThenInclude(r => r.Nutrition)
                .FirstOrDefault(a => a.Id == id);


            var ingredientQuantities = mealplan.Recipes
        .SelectMany(r => r.RecipeIngredients)
        .GroupBy(ri => new { ri.IngredientId, ri.UnitOfMeasure })
        .Select(group => new RecipeIngredient {
            IngredientId = group.Key.IngredientId,
            Ingredient = group.FirstOrDefault().Ingredient,
            UnitOfMeasure = group.Key.UnitOfMeasure,
            Quantity = group.Sum(ri => ri.Quantity)
        })
        .ToList();

            var totalNutrition = new Nutrition() {
                Calories =  mealPlanWithNutrition.Recipes.Sum(r => r.Nutrition.Calories),
                Fat = mealPlanWithNutrition.Recipes.Sum(r => r.Nutrition.Fat),
                Protein = mealPlanWithNutrition.Recipes.Sum(r => r.Nutrition.Protein),
                Carbohydrate = mealPlanWithNutrition.Recipes.Sum(r => r.Nutrition.Carbohydrate),
                Cholesterol = mealPlanWithNutrition.Recipes.Sum(r => r.Nutrition.Cholesterol)
            };
            

            ViewData["TotalNutrition"] = totalNutrition;
            ViewData["GroupedIngredients"] = ingredientQuantities;



            return View(mealplan);
        }

        [Breadcrumb("Create")]
        public IActionResult Create() {
            var recipes = _context.Recipes
                       .Where(i => i.Status == true)
                       .ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealPlan mealplan, string[] RecipeNames) {
            if (ModelState.IsValid) {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null) {
                    if (RecipeNames != null) {
                        mealplan.FkUserId = await _userManager.GetUserIdAsync(currentUser);
                        mealplan.IsFullDay = false;
                        var allRecipes = _context.Recipes
                                               .Where(i => i.Status == true)
                                               .ToList();
                        foreach (var recipeName in RecipeNames) {
                            ExtractIntegerAndString(recipeName, out int id, out string name);
                            var recipe = allRecipes.FirstOrDefault(r => r.Id == id && r.Name == name);
                            if (recipe != null) {
                                mealplan.Recipes.Add(recipe);
                            }
                        }
                        _context.MealPlans.Add(mealplan);
                        _context.SaveChanges();
                        TempData["success"] = "Meal planning created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(mealplan);
        }


    }
}