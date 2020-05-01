using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataPurchase
    {
        public DataUser User => new DataUser(InnerPurchase.User);
        public DataBasket Basket => new DataBasket(InnerPurchase.Basket);
        public DateTime TimeStamp => InnerPurchase.TimeStamp;
        private Purchase InnerPurchase { get; }

        public DataPurchase(Purchase prchs)
        {
            InnerPurchase = prchs;
        }
    }
}
