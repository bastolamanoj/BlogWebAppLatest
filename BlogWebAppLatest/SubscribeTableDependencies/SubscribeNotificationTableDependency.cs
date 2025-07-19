using BlogWebApp.Hubs;
using BlogWebApp.Models;
using BlogWebApp.SubscribeTableDependencies;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;


namespace BlogWebApp.SubscribeTableDependencies
{
    public class SubscribeNotificationTableDependency : ISubscribeTableDependency
    {
        //private const DmlTriggerType UpdateOf = DmlTriggerType.Insert | DmlTriggerType.Update;
        SqlTableDependency<Notification> tableDependency;
        NotificationHub notificationHub;

        public SubscribeNotificationTableDependency(NotificationHub notificationHub)
        {
            this.notificationHub = notificationHub;
        }

        public void SubscribeTableDependency(string connectionString)
        {
            tableDependency = new SqlTableDependency<Notification>(connectionString);
            //tableDependency = new SqlTableDependency<Notification>(connectionString, "Notifications",DmlTriggerType.Insert);
            tableDependency.OnChanged += TableDependency_OnChanged;
            tableDependency.OnError += TableDependency_OnError;
            tableDependency.Start();
        }

        private void TableDependency_OnError(object sender, TableDependency.SqlClient.Base.EventArgs.ErrorEventArgs e)
        {
            Console.WriteLine($"{nameof(Notification)} SqlTableDependency error: {e.Error.Message}");
        }

        private async void TableDependency_OnChanged(object sender, TableDependency.SqlClient.Base.EventArgs.RecordChangedEventArgs<Notification> e)
        {
            if(e.ChangeType != TableDependency.SqlClient.Base.Enums.ChangeType.None)
            {
                var notification = e.Entity;
                if(notification.MessageType == "All")
                {
                    await notificationHub.SendNotificationToAll(notification.Title  );
                }
                else if(notification.MessageType == "Personal")
                {
                    await notificationHub.SendNotificationToClient(notification.Body, notification.ForUserId, notification.Id);
                }
                else if (notification.MessageType == "Group")
                {
                    await notificationHub.SendNotificationToGroup(notification.Body, notification.Username);
                }
            }
        }
    }
}
