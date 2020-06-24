using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SEWorkshop.Enums;
using SEWorkshop.ServiceLayer;

namespace Website.Hubs
{
    public class NewUseReportObserver : IServiceObserver<KindOfUser>
    {

        private IHubContext<NewUseReportHub> Hub { get; }
        private ConcurrentDictionary<string, string> ConnectionsDict { get; }
        private IUserManager UserManager { get; }

        public NewUseReportObserver(IUserManager userManager, IHubContext<NewUseReportHub> hub)
        {
            UserManager = userManager;
            Hub = hub;
            ConnectionsDict = new ConcurrentDictionary<string, string>();
            UserManager.RegisterNewUseReportObserver(this);
        }


        public void OnConnectionToHub(string conId, string userName)
        {
            ConnectionsDict[conId] = userName;
        }

        public void OnDisconnectedFromHub(string conId)
        {
            ConnectionsDict.Remove(conId, out string? _);
        }

        public async void Notify(KindOfUser arg)
        {
            foreach (var keyVal in ConnectionsDict)
            {
                if (keyVal.Value.Equals("admin"))
                {
                    await Hub.Clients.Client(keyVal.Key).SendAsync("NewUseReport", arg.ToString());
                }
            }
        }
    }
}
