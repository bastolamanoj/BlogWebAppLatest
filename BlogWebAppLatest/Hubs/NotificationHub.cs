using BlogWebApp.Models;
using BlogWebApp.ViewModel;
using BlogWebAppLatest.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ApplicationDbContext dbContext;

        public NotificationHub(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.SendAsync("ReceivedNotification", message);
        }

        public async Task SendNotificationToClient(string message, string userid, int id)
        {
            var hubConnections = dbContext.HubConnections.Where(con => con.UserId == userid).ToList();
            var unreadNotificationCount = dbContext.Notifications.Where(a => !a.IsRead && a.ForUserId == userid).Count();
            var notifications = (from noti in dbContext.Notifications
                                 join blog in dbContext.Blogs on noti.BlogId equals blog.Id
                                 join user in dbContext.Users on blog.AuthorId.ToString() equals user.Id
                                 where blog.AuthorId == Guid.Parse(userid) && !noti.IsRead
                                 orderby noti.CreatedAt descending
                                 select new NotificationVm
                                 {
                                     Id = noti.Id,
                                     Title = noti.Title,
                                     Body = noti.Body,
                                     Username = user.DisplayName,
                                     Url = user.ProfileUrl,
                                     IsRead = noti.IsRead,
                                     BlogId = blog.Id,
                                     NotificationDate = noti.CreatedAt
                                 }).ToList();

            if (notifications.Count() > 0)
            {
                notifications[0].TotalNotification = unreadNotificationCount;

            }

            foreach (var hubConnection in hubConnections)
            {
                //await Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, userid);
                await Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", notifications);
            }
        }
        public async Task SendNotificationToGroup(string message, string group)
        {
            var hubConnections = dbContext.HubConnections.Join(dbContext.Users, c => c.Username, o => o.DisplayName, (c, o) => new { c.Username, c.ConnectionId}).ToList();
            foreach (var hubConnection in hubConnections)
            {
                string username = hubConnection.Username;
                await Clients.Client(hubConnection.ConnectionId).SendAsync("ReceivedPersonalNotification", message, username);
                //Call Send Email function here
            }
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("OnConnected");
            return base.OnConnectedAsync();
        }

        public async Task SaveUserConnection(string username, string userid)
        {
            var connectionId = Context.ConnectionId;
            HubConnection hubConnection = new HubConnection
            {
                ConnectionId = connectionId,
                Username = username,
                UserId= userid
            };

            dbContext.HubConnections.Add(hubConnection);
            await dbContext.SaveChangesAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var hubConnection = dbContext.HubConnections.FirstOrDefault(con => con.ConnectionId == Context.ConnectionId);
            if (hubConnection != null)
            {
                dbContext.HubConnections.Remove(hubConnection);
                await dbContext.SaveChangesAsync(); // Await SaveChangesAsync
            }

            await base.OnDisconnectedAsync(exception); // Ensure base method is awaited
        }

    }
}
