﻿using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CapstoneProject.Controllers {
    public class HomeController : Controller {

        private readonly ILogger<HomeController> _logger;

        private readonly RecipeOrganizerContext _context;

        public HomeController(RecipeOrganizerContext context) {
            //_logger = logger;
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        public IActionResult RecipeDetail() {
            return View();
        }

        [Route("Home/ViewIngredient/{id}")]
        public IActionResult ViewIngredient(int id) {
            ViewBag.Id = id;
            IEnumerable<Ingredient> obj = _context.Ingredients.ToList();
            return View(obj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}