using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEWorkshop.ServiceLayer;
using Microsoft.AspNetCore.Http;
using SEWorkshop.DataModels;
using System.Collections.Concurrent;
using System.Threading;

namespace Website.Hubs
{
    public class MessagesNotificationsHub : Hub
    {
        public IUserManager UserManager { get; }
        private MessagesObserver Observer { get; }

        public MessagesNotificationsHub(IUserManager userManager, MessagesObserver observer)
        {
            UserManager = userManager;
            Observer = observer;
        }

        public Task KeepAlive(string message)
        {
            return Task.CompletedTask;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            string userName = httpContext.Request.Query["userName"];
            Observer.OnConnectionToHub(Context.ConnectionId, userName);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Observer.OnDisconnectedFromHub(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

       
    }
}
