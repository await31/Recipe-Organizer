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

        [Breadcrumb("My Planning")]
        public IActionResult Index() {
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

    }
}
