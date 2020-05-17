using SEWorkshop.Models.Discounts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataBuyOverDiscount : DataDiscount
    {
        private BuyOverDiscount InnerBuyOverModel { get; }
        public double MinSum => InnerBuyOverModel.MinSum;
        public DataProduct Product => new DataProduct(InnerBuyOverModel.Product);
        
        public DataBuyOverDiscount(BuyOverDiscount discount) : base(discount)
        {
            InnerBuyOverModel = discount;
        }
    }
}
