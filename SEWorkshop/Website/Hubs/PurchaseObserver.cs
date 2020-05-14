using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Website.Hubs
{
    public class PurchaseObserver : IServiceObserver<DataPurchase>
    {
        private IHubContext<PurchasesNotificationsHub> PrchsHub { get; }
        private ConcurrentDictionary<string, string> ConnectionsDict { get; }
        private IUserManager UserManager { get; }

        public PurchaseObserver(IHubContext<PurchasesNotificationsHub> hubContext, IUserManager userManager)
        {
            PrchsHub = hubContext;
            UserManager = userManager;
            UserManager.RegisterPurchaseObserver(this);
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

        public async void Notify(DataPurchase arg)
        {
            foreach (var keyVal in ConnectionsDict)
            {
                var usrName = keyVal.Value;
                var owns = arg.Basket.Store.Owners.First(innerKeyVal => innerKeyVal.Key.Username.Equals(usrName));
                // This requirement is defined only for owners
                foreach (var innerKeyVal in arg.Basket.Store.Owners)
                {
                    if (innerKeyVal.Key.Username.Equals(usrName))
                    {
                        //push it
                        await PrchsHub.Clients.Client(keyVal.Key).SendAsync("NewPurchase", arg.Basket.Store.Name);
                        return;
                    }
                }
            }
        }
    }
}
