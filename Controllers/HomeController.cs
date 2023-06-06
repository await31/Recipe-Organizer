using CapstoneProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;


namespace CapstoneProject.Controllers {

    [DefaultBreadcrumb]
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

        [Breadcrumb("Privacy", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult Privacy() {
            return View();
        }

        [Breadcrumb("Recipe Detail", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult RecipeDetail() {
            return View();
        }

        [Breadcrumb("View Ingredients", FromAction = "Index", FromController = typeof(HomeController))]
        public IActionResult ViewIngredient(int id, int pg=1) {

            IEnumerable<Ingredient> obj = _context.Ingredients.Where(i => i.FkCategoryId == id).ToList();

            const int pageSize = 10; // Number of ingredients in 1 page
            
            if (pg < 1) 
                pg = 1;

            int recsCount = obj.Count();

            var pager = new Pager(recsCount,pg, pageSize);

            int recSkip = (pg - 1) * pageSize;

            var data= obj.Skip(recSkip).Take(pager.PageSize).ToList();

            this.ViewBag.Pager = pager;

            return View(data);   
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}