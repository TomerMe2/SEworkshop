using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Website.Hubs
{
    public class NotificationsObserver : IServiceObserver<DataMessage>
    {
        private IHubContext<NotificationsHub> MsgHub { get; }
        private ConcurrentDictionary<string, string> ConnectionsDict { get; }
        private IUserManager UserManager { get; }

        public NotificationsObserver(IHubContext<NotificationsHub> hubContext, IUserManager userManager)
        {
            MsgHub = hubContext;
            UserManager = userManager;
            UserManager.RegisterMessageObserver(this);
            ConnectionsDict = new ConcurrentDictionary<string, string>();
        }

        public void OnConnectionToHub(string conId, string userName)
        {
            ConnectionsDict[conId] = userName;
        }

        public void OnDisconnectedFromHub(string conId)
        {
            ConnectionsDict.Remove(conId, out string? _);
        }

        public async void Notify(DataMessage arg)
        {
            DataMessage first = arg;
            while (first.Prev != null)
            {
                first = first.Prev;
            }
            foreach (var keyVal in ConnectionsDict)
            {
                if (keyVal.Value.Equals(first.WrittenBy.Username) && !arg.WrittenBy.Username.Equals(keyVal.Value))
                {
                    await MsgHub.Clients.Client(keyVal.Key).SendAsync("NewMessage", arg.ToStore.Name);
                }
            }
        }
    }
}
