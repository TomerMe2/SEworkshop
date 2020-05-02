using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataPurchase : DataModel<Purchase>
    {
        public DataUser User => new DataUser(InnerModel.User);
        public DataBasket Basket => new DataBasket(InnerModel.Basket);
        public DateTime TimeStamp => InnerModel.TimeStamp;

        public DataPurchase(Purchase prchs) : base(prchs) { }
    }
}
