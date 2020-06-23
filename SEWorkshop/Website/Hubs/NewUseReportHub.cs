using Microsoft.AspNetCore.SignalR;
using SEWorkshop.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Hubs
{
    public class NewUseReportHub : Hub
    {
        public IUserManager UserManager { get; }
        private NewUseReportObserver Observer { get; }

        public NewUseReportHub(IUserManager userManager, NewUseReportObserver observer)
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
