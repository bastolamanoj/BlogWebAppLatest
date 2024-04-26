using System.Security.Policy;

namespace BlogWebApp.ViewModel
{
    public class NotificationVm
    {
        public string? Title { get; set; }   // This the Notification Title
        public string? Body { get; set; }
        public Guid UserId { get; set; } 
        public string? Username { get; set; }
        public string? Url { get; set; }
        public DateTime? NotificationDate { get; set; }  
        public Guid BlogId { get; set; } 
        public int? TotalNotification { get; set;  }
        public bool IsRead { get; set; } //
    }
}
