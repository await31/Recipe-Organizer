using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CapstoneProject.Controllers {

    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
