using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataBuySomeGetSomeFreeDiscount : DataDiscount
    {
        private BuySomeGetSomeFreeDiscount InnerBuySomeModel { get; }
        public int BuySome => InnerBuySomeModel.BuySome;
        public int GetSome => InnerBuySomeModel.GetSome;
        
        public DataBuySomeGetSomeFreeDiscount(BuySomeGetSomeFreeDiscount discount) : base(discount)
        {
            InnerBuySomeModel = discount;
        }
    }
}
