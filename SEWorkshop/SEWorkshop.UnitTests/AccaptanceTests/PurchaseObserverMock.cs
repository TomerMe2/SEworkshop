using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;

namespace SEWorkshop.Tests.AccaptanceTests
{
    class PurchaseObserverMock : IServiceObserver<DataPurchase>
    {
        public IList<DataPurchase> Purchases { get; }

        public PurchaseObserverMock()
        {
            Purchases = new List<DataPurchase>();
        }

        public void Notify(DataPurchase arg)
        {
            Purchases.Add(arg);
        }

    }
}
