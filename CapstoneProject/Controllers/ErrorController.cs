using Microsoft.AspNetCore.Mvc;

namespace CapstoneProject.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/404")]
        public IActionResult NotFound(string errorMessage)
        {
            ViewData["Error"] = errorMessage;
            return View();
        }
    }
}
