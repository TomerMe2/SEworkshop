using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataBuySomeGetSomeDiscount : DataPrimitiveDiscount
    {
        private BuySomeGetSomeDiscount InnerBuySomeModel { get; }
        public int BuySome => InnerBuySomeModel.BuySome;
        public int GetSome => InnerBuySomeModel.GetSome;
        public DataProduct Product => new DataProduct(InnerBuySomeModel.Product);
        public DataProduct ProdUnderDiscount => new DataProduct(InnerBuySomeModel.ProdUnderDiscount);
        
        public DataBuySomeGetSomeDiscount(BuySomeGetSomeDiscount discount) : base(discount)
        {
            InnerBuySomeModel = discount;
        }

        public override string ToString()
        {
            return "Buy " + BuySome + " Get "+ GetSome + " With: " + Percentage + "% Discount On Product: '" + this.Product.Name + "' Untill: " + Deadline.ToString();
        }
    }
}
