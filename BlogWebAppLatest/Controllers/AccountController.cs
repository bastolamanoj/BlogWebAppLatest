using BlogWebApp.Models;
using BlogWebApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BlogWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email,model.Password,model.RememberMe,false);
                if (result.Succeeded) {
                    return RedirectToAction("Index", "Blog");
                }
                ModelState.AddModelError("", "Invalid Login Attempt");
                return View(model);
            }
           return View(model);
        }
         
        
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if(ModelState.IsValid)
            {
                User user = new() { 
                 DisplayName= model.Name,
                 UserName= model.Email,
                 Email= model.Email,
                 PasswordHash= model.Password

                };

                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded) {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Blog");
                }
               
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
           return View(model);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(BlogController.Index));
        }
    }
}
