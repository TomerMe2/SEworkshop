using Microsoft.AspNetCore.SignalR;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Hubs
{
    public class OwnershipRequestObserver : IServiceObserver<DataOwnershipRequest>
    {
        private IHubContext<OwnershipRequestHub> Hub { get; }
        private ConcurrentDictionary<string, string> ConnectionsDict { get; }
        private IUserManager UserManager { get; }

        public OwnershipRequestObserver(IUserManager userManager, IHubContext<OwnershipRequestHub> hub)
        {
            UserManager = userManager;
            Hub = hub;
            ConnectionsDict = new ConcurrentDictionary<string, string>();
            UserManager.RegisterOwnershipObserver(this);
        }

        public void OnConnectionToHub(string conId, string userName)
        {
            ConnectionsDict[conId] = userName;
        }

        public void OnDisconnectedFromHub(string conId)
        {
            ConnectionsDict.Remove(conId, out string? _);
        }

        public async void Notify(DataOwnershipRequest arg)
        {
            
            foreach (var keyVal in ConnectionsDict)
            {
                string userName = keyVal.Value;
                bool isOwningStore = arg.Store.Owners.Keys.Any(usr => usr.Username.Equals(userName));
                bool isPending;
                try
                {
                    isPending = arg.Answers.First(tup =>
                        tup.Item1.Username.Equals(userName)).Item2 == SEWorkshop.Enums.RequestState.Pending;
                }
                catch
                {
                    isPending = false;
                }
                if (isOwningStore && isPending)
                {
                    await Hub.Clients.Client(keyVal.Key).SendAsync("NewOwnershipRequest", 
                        string.Format("{0} is asking to become owner of store {1}", arg.NewOwner.Username, arg.Store.Name));
                }
            }
        }

    }
}
