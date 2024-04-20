using Microsoft.AspNetCore.Mvc;

namespace BlogWebApp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
    }
}
