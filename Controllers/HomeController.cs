﻿using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CapstoneProject.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly RecipeOrganizerContext _context;
        public HomeController(ILogger<HomeController> logger, RecipeOrganizerContext context) {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() {
            var recipes = _context.Recipes.ToList();
            return View(recipes);
        }

        public IActionResult Privacy() {
            return View();
        }

        public IActionResult RecipeDetail() {
            return View();
        }

        public IActionResult IngredientCategory() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}