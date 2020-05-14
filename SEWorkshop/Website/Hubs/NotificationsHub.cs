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
    public class NotificationsHub : Hub
    {
        public IUserManager UserManager { get; }
        private NotificationsObserver Observer { get; }

        public NotificationsHub(IUserManager userManager, NotificationsObserver observer)
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
