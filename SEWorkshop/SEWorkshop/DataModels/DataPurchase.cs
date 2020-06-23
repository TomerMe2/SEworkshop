using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataPurchase : DataModel<Purchase>
    {
        public DataUser? User => InnerModel.User != null ? new DataUser(InnerModel.User) : null;
        public DataBasket Basket => new DataBasket(InnerModel.Basket);
        public DateTime TimeStamp => InnerModel.TimeStamp;
        public Address Address => InnerModel.Address;

        public DataPurchase(Purchase prchs) : base(prchs) { }
    }
}
