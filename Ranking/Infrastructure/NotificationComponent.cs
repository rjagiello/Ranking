using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Configuration;
using System.Data.SqlClient;

namespace Ranking.Infrastructure
{
    public class NotificationComponent
    {
        public void RegisterNotification(string name)
        {
            string conStr = ConfigurationManager.ConnectionStrings["RankContext"].ConnectionString;
            string sqlCommand = @"SELECT [MatchId] FROM [dbo].[Match] WHERE ([Team1] = @Name or [Team2] = @Name) and ([IsFinished] = 0 and [NotAddedBy] = @Name)";
            //you can notice here I have added table name like this [dbo].[Contacts] with [dbo], its mendatory when you use Sql Dependency  
            using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con);
                cmd.Parameters.AddWithValue("@Name", name);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange;
                //we must have to execute the command here  
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now  
                }
            }
        }

        void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            //or you can also check => if (e.Info == SqlNotificationInfo.Insert) , if you want notification only for inserted record  
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;

                //from here we will send notification message to client  
                var notificationHub = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                notificationHub.Clients.All.notify("added");
                //re-register notification  
                RegisterNotification(DateTime.Now);
            }
        }

        public List<tblEmployee> GetData(DateTime afterDate)
        {
            using (SignalRDBEntities dc = new SignalRDBEntities())
            {
                return dc.tblEmployees.Where(a => a.AddedOn > afterDate).OrderByDescending(a => a.AddedOn).ToList();
            }
        }
    }
}
}