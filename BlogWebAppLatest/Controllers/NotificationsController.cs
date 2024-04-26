using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BlogWebApp.Models;
using BlogWebAppLatest.Data;
using BlogWebApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using BlogWebApp.Models.IdentityModel;

namespace BlogWebApp.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public NotificationsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Notifications.Include(n => n.Blog);
            return View(await applicationDbContext.ToListAsync());
        }
       
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] NotificationVm notification)
        {
            var user = _userManager.GetUserAsync(User).Result;
            var userid = user.Id;
            Notification noti = new Notification()
            {
                Title=notification.Title,
                Body= notification.Body,
                IsRead= false,
                BlogId= notification.BlogId,
                UserId = Guid.Parse(userid),
                CreatedAt= DateTime.Now,
                UpdatedAt= DateTime.Now,

            };
            if (ModelState.IsValid)
            {
                _context.Notifications.Add(noti);
                await _context.SaveChangesAsync();
                
            }
            //ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Body", notification.BlogId);
            return Ok(new {status=200, message="success"});
        }

        // GET: Notifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.Blog)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notification == null)
            {
                return NotFound();
            }

            return View(notification);
        }

        [HttpGet]
        public IActionResult GetNotification()
        {
            var currentuser = _userManager.GetUserAsync(User).Result;
            var userid = currentuser.Id;

            var notification = (from noti in _context.Notifications
                                 join blog in _context.Blogs on noti.BlogId equals blog.Id
                                 join user in _context.Users on noti.UserId.ToString() equals user.Id
                                 where blog.AuthorId == Guid.Parse(userid) && user.Id == userid
                                 select new NotificationVm
                                 {
                                     Title = noti.Title, // Assuming Notification entity has a Title property
                                     Body = noti.Body,
                                     Username = user.DisplayName,
                                     Url= user.ProfileUrl,
                                     BlogId=blog.Id,
                                     NotificationDate = noti.CreatedAt

                                 }).ToList();
            var unreadNotificationCount = _context.Notifications
                    .Where(noti => !noti.IsRead)
                    .Count();

            notification.FirstOrDefault().TotalNotification = unreadNotificationCount;
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(new { notification= notification });
        }


        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
