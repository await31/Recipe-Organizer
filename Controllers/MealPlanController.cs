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

namespace CapstoneProject.Controllers {
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

        private bool IsRecipeNameExists(string name) {
            return (_context.Recipes?.Any(e => e.Name == name)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult RecipeNameExists(string name) {
            bool exists = IsRecipeNameExists(name); // Call the existing private method
            return Json(exists);
        }

        [HttpPost]
        public JsonResult SearchAutoComplete(string term) {
            var result = (_context.Recipes.Where(t => t.Name.ToLower().Contains(term.ToLower()))
                 .Select(t => new { t.Name }))
                 .ToList();
            return Json(result);
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
            int recsCount = mealPlans.Count();
            int pageSize = 6;
            var pager = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = mealPlans.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;
            return View(data);
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
        public IActionResult Details() {
            return View();
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
        public async Task<IActionResult> Create(MealPlan mealplan,string[] RecipeNames) {
            if (ModelState.IsValid) {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null) {
                    if (RecipeNames != null) {
                        mealplan.FkUserId = await _userManager.GetUserIdAsync(currentUser);
                        mealplan.IsFullDay = false;
                        var allRecipes = _context.Recipes
                                               .Where(i => i.Status == true)
                                               .ToList();
                        var recipes = allRecipes
                                          .Where(i => RecipeNames
                                          .Any(input => i.Name
                                          .Equals(input)))
                                          .ToList();

                        mealplan.Recipes.AddRange(recipes);
                        _context.MealPlans.Add(mealplan);

                        await _context.SaveChangesAsync();
                        TempData["success"] = "Meal planning created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View(mealplan);
        }

    }
}
