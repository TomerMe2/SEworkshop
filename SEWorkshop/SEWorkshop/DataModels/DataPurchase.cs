using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    class DataPurchase
    {
        public DataUser User { get => new DataUser(InnerPurchase.User); }
        public DataBasket Basket { get => new DataBasket(InnerPurchase.Basket); }
        public DateTime TimeStamp { get => InnerPurchase.TimeStamp; }
        private Purchase InnerPurchase { get; }

        public DataPurchase(Purchase prchs)
        {
            InnerPurchase = prchs;
        }
    }
}
