using BlogWebApp.Models.IdentityModel;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlogWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly ApplicationDbContext _dbcontext;
        //private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager,
            IEmailSender emailSender, ILogger<AccountController> logger, RoleManager<Role> roleManager,
               ApplicationDbContext dbcontext) 
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
            _roleManager = roleManager;
            _dbcontext = dbcontext;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Index(string? aa)
        {
            return RedirectToAction("/");
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public IActionResult Login()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("/"); 
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                //var user = await _userManager.FindByNameAsync(model.Email);
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (result.Succeeded)
                {
                    //HttpContext.Session.SetString("UserName", user.DisplayName);
                    return RedirectToAction("Index", "Blog");
                }
                else if (result.IsNotAllowed)
                {
                    // Optionally, you can check if the user is locked out or needs email confirmation.
                    // For example:
                   
                    if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
                    {
                       
                        // Redirect to a page where the user can confirm their email.
                        return RedirectToAction("EmailConfirmation", "Account");
                    }
                    else
                    {
                        // Handle other cases where the user is not allowed to sign in.
                        ModelState.AddModelError("", "You are not allowed to sign in. Please contact support.");
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                    return View(model);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            return RedirectToAction("login");
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
                 EmailConfirmed=true
                 //PasswordHash= model.Password

                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded) {
                    // Assign Default Role Admin to first registar; rest is user
                    var checkAdmin = await _roleManager.FindByNameAsync("Admin");
                    if (checkAdmin is null)
                    {
                        await _roleManager.CreateAsync(new Role() { Name = "Admin", AliasName = "Admin" });
                        await _userManager.AddToRoleAsync(user, "Admin");
                        //return new GeneralResponse(true, "Account Created.");
                    }
                    else
                    {
                        var checkUser = await _roleManager.FindByNameAsync("Blogger");
                        if (checkUser is null)
                            await _roleManager.CreateAsync(new Role() { Name = "Blogger", AliasName = "Blogger" });
                        await _userManager.AddToRoleAsync(user, "Blogger");
                        //return new GeneralResponse(true, "Account Created.");
                    }

                    //await _signInManager.SignInAsync(user, false);
                    TempData["SuccessMessage"] = "Your account has been created.";
                    return RedirectToAction("Login", "Account");
                }
               
                foreach(var error in result.Errors)
                {
                    TempData["ErrorMessage"] = "Railed to created account";
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddAdmin(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    DisplayName = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Address="",
                    EmailConfirmed=true

                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin"); // Add user to the "Admin" role

                    //await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("manageadmin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                ModelState.AddModelError("", "Validation Error");
            }
           
            return View(model);
        }

        [HttpGet("addadmin")]
        public IActionResult AddAdmin()
        {
            return View();
        }

        [HttpGet("manageadmin")]
        public async Task<IActionResult> ManageAdmin()
        {
            //_roleManager
            return View( await _userManager.GetUsersInRoleAsync("Admin"));
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("/");
            }
            return View();
        }

        [HttpGet("forgetpassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Blog");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            //await _signInManager.RefreshSignInAsync(user);
            //return RedirectToAction("Index", "Home");
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");

        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string code)
        {
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Code = code
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation Error.";
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User Not Valid.";
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Your Password has been reset successfully.";
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = token }, HttpContext.Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                        $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                TempData["SuccessMessage"] = "Email Has been sent. please check your inbox";
                }
                else
                {
                TempData["ErrorMessage"] = "Your email doesn't match.";
                }

                //return RedirectToAction("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //this method for deleting the admin user
        public async Task<IActionResult> DeleteUser(string? id)
        {
            var userId = id;
            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User ID cannot be null or empty.");
                //return BadRequest("User ID cannot be null or empty.");
            }

            // Retrieve the user from the database using the provided userId
            var user = await _userManager.FindByIdAsync(userId);

            // Check if the user exists
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                //return NotFound("User not found.");
            }

            var totalUsersCount = await (
               from users in _dbcontext.Users
               join roleUser in _dbcontext.UserRoles on users.Id equals roleUser.UserId
               join role in _dbcontext.Roles on roleUser.RoleId equals role.Id
               where role.Name == "Admin"
               select user
            ).CountAsync();


            if (totalUsersCount <= 1)
            {
                TempData["ErrorMessage"] = "Cannot delete the only admin.";
                ModelState.AddModelError("", "Cannot delete the only user in the database.");
                return RedirectToAction("manageadmin", "Account");
            }

            // Remove the user from the database using the UserManager
            var result = await _userManager.DeleteAsync(user);

            // Check if the user deletion was successful
            if (result.Succeeded)
            {
                // Redirect the user to a suitable action, such as a list of users or another appropriate page
                return RedirectToAction("AddAdmin", "Account"); // Redirect to a user management page
            }
            else
            {
                // If the user deletion failed, display an error message
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                ModelState.AddModelError("","Failed");
                return RedirectToAction("AddAdmin", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBlogger(string? id)
        {
            var userId = id;
            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                //TempData["SuccessMessage"] = unreadlatestnoti.Body;
                TempData["ErrorMessage"] = "User ID cannot be null or empty.";
                //return BadRequest("User ID cannot be null or empty.");
            }
            var user = await _userManager.FindByIdAsync(userId);
            var userrole= await _userManager.GetRolesAsync(user);
            
            if (userrole.FirstOrDefault() == "Blogger")
            {
                var result = await _userManager.DeleteAsync(user);
                // Check if the user deletion was successful
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Blog");
                }
                
            }
            else
            {
                var totalUsersCount = await (
                 from users in _dbcontext.Users
                 join roleUser in _dbcontext.UserRoles on users.Id equals roleUser.UserId
                 join role in _dbcontext.Roles on roleUser.RoleId equals role.Id
                 where role.Name == "Admin"
                 select user
                  ).CountAsync();


                if (totalUsersCount <= 1)
                {
                    TempData["ErrorMessage"] = "Cannot delete the only admin.";
                    ModelState.AddModelError("", "Cannot delete the only user in the database.");
                    return RedirectToAction("Index", "Blog");
                }

                // Remove the user from the database using the UserManager
                var result = await _userManager.DeleteAsync(user);

                // Check if the user deletion was successful
                if (result.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Index", "Blog");
                }
                else
                {
                    // If the user deletion failed, display an error message
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    ModelState.AddModelError("", "Failed");
                    return RedirectToAction("AddAdmin", "Account");
                }
            }
                return RedirectToAction("Index", "Blog");


        }
    }
}
