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
            try
            {
            var user = _userManager.GetUserAsync(User).Result;
            var userid = user.Id;
                var forUserId = _context.Blogs
                .Where(b => b.Id == notification.BlogId)
                .Select(b => b.AuthorId)
                .FirstOrDefault().ToString();
                Notification noti = new Notification()
            {
                Title=notification.Title,
                Body= notification.Body,
                MessageType= "Personal",
                ForUserId= forUserId,
                IsRead = false,
                BlogId= notification.BlogId,
                UserId = Guid.Parse(userid),
                CreatedAt= DateTime.Now,
                UpdatedAt= DateTime.Now,

            };
                if (ModelState.IsValid)
                {
                    using (var transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            // Disable triggers
                            //await _context.Database.ExecuteSqlRawAsync("DISABLE TRIGGER ALL ON dbo.Notification");

                            // Add the notification
                            _context.Notifications.Add(noti);
                            await _context.SaveChangesAsync();

                            // Commit the transaction
                            await transaction.CommitAsync();

                            // Re-enable triggers
                            //await _context.Database.ExecuteSqlRawAsync("ENABLE TRIGGER ALL ON dbo.Notification");
                        }
                        catch (Exception)
                        {
                            // Rollback the transaction if an exception occurs
                            await transaction.RollbackAsync();
                            throw;
                        }
                    }
                }
                return Ok(new { status = 200, message = "success" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
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
            var currentUser = _userManager.GetUserAsync(User).Result;
            var userId = currentUser.Id;

            var notifications = (from noti in _context.Notifications
                                 join blog in _context.Blogs on noti.BlogId equals blog.Id
                                 join user in _context.Users on blog.AuthorId.ToString() equals user.Id
                                 where blog.AuthorId == Guid.Parse(userId)
                                 orderby noti.CreatedAt descending
                                 select new NotificationVm
                                 {
                                     Id = noti.Id,
                                     Title = noti.Title,
                                     Body = noti.Body,
                                     Username = user.DisplayName,
                                     Url = user.ProfileUrl,
                                     IsRead=noti.IsRead,
                                     BlogId = blog.Id,
                                     NotificationDate = noti.CreatedAt
                                 }).ToList();

            var unreadNotificationCount = notifications
                .Count(noti => !noti.IsRead);
            if (notifications.Count() > 0)
            {
                var unreadlatestnoti = notifications.Where(a => !a.IsRead).FirstOrDefault();
                if(unreadlatestnoti != null)
                {
                  TempData["SuccessMessage"] = unreadlatestnoti.Body;
                }
                notifications[0].TotalNotification = unreadNotificationCount;

            }

            if (notifications.Count() == 0)
            {
                return NotFound();
            }

            return Ok(new { notifications = notifications });
        }

        //[HttpGet]
        //public async Task<IActionResult> GetNotification()
        //{
        //    var currentUser = await _userManager.GetUserAsync(User);
        //    if (currentUser == null)
        //    {
        //        // Handle the case where the user is not found
        //        return NotFound("User not found");
        //    }

        //    var userId = currentUser.Id;

        //    var notifications = (from noti in _context.Notifications
        //                         join blog in _context.Blogs on noti.BlogId equals blog.Id
        //                         join user in _context.Users on blog.AuthorId.ToString() equals user.Id
        //                         where blog.AuthorId == Guid.Parse(userId)
        //                         orderby noti.CreatedAt descending
        //                         select new NotificationVm
        //                         {
        //                             Id = noti.Id,
        //                             Title = noti.Title,
        //                             Body = noti.Body,
        //                             Username = user.DisplayName,
        //                             Url = user.ProfileUrl,
        //                             IsRead = noti.IsRead,
        //                             BlogId = blog.Id,
        //                             NotificationDate = noti.CreatedAt
        //                         }).ToList();


        //    var unreadNotificationCount = notifications.Count(noti => !noti.IsRead);
        //    if (notifications.Any())
        //    {
        //        TempData["SuccessMessage"] = notifications[0].Body;
        //        notifications[0].TotalNotification = unreadNotificationCount;
        //    }

        //    if (!notifications.Any())
        //    {
        //        return NotFound();
        //    }

        //    return Ok(new { notifications = notifications });
        //}

        [HttpPost]
        public async Task<IActionResult> UdpateNotificationStaus([FromBody] List<int> notificationIds)
        {
            if (notificationIds == null || notificationIds.Count == 0)
            {
                return BadRequest("No notification IDs provided.");
            }

            //var notification= _context.Notifications.
            foreach (var notificationId in notificationIds)
            {
                var notificationToUpdate = await _context.Notifications.FindAsync(notificationId);

                if (notificationToUpdate != null)
                {
                    notificationToUpdate.IsRead = true;
                    notificationToUpdate.UpdatedAt = DateTime.Now;

                    _context.Notifications.Update(notificationToUpdate);
                }
            }
            await _context.SaveChangesAsync();

            return Ok(new { status = 200, message = "Notifications updated successfully" });

        }

        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
