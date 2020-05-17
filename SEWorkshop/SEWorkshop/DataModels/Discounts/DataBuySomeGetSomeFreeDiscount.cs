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
        public DataProduct Product => new DataProduct(InnerBuySomeModel.Product);
        
        public DataBuySomeGetSomeFreeDiscount(BuySomeGetSomeFreeDiscount discount) : base(discount)
        {
            InnerBuySomeModel = discount;
        }

        public override string ToString()
        {
            return "Buy " + BuySome + " Get "+ GetSome + " With: " + Percentage + "% Discount On Product: '" + this.Product.Name + "' Untill: " + Deadline.ToString();
        }
    }
}
