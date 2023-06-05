using CapstoneProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CapstoneProject.Controllers {

    [Authorize]
    public class DashboardController : Controller {

        private readonly RecipeOrganizerContext _context;

        public DashboardController(RecipeOrganizerContext context) {
            _context = context;
        }

        public IActionResult Index() {
            IEnumerable<Recipe> objRecipe = _context.Recipes.ToList();
            return View(objRecipe);
        }
    }
}
