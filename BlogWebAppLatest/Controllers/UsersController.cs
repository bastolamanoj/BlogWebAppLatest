using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        public UsersController(ApplicationDbContext dbcontext) { 
            _dbcontext = dbcontext;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileUrl(string profileUrl)
        {
            if (ModelState.IsValid)
            {
                // Get the current user (you need to implement this part according to your authentication mechanism)
                var currentUser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);

                if (currentUser != null)
                {
                    // Update the ProfileUrl property
                    currentUser.ProfileUrl = profileUrl;

                    // Save changes to the database
                    await _dbcontext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Profile URL updated successfully.";
                    return RedirectToAction("Index", "Home"); // Redirect to the home page or any other appropriate page
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("EditProfileUrl");
                }
            }

            // If ModelState is not valid, return the view with errors
            return View(profileUrl);
        }
    }
}
