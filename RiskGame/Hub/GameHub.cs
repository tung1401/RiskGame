using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace RiskGame.Helper
{
    public class GameHub : Hub
    {
        private static string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<RiskHub>();
            context.Clients.All.updateMessages();
        }

    }
}