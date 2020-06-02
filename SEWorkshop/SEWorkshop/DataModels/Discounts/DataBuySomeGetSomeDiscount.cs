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
            if(GetSome == -1)
                return "Buy " + BuySome + " " + Product.Name + " Get Limitless Amounf Of " + ProdUnderDiscount.Name + " With: " + Percentage + "% Discount Untill: " + Deadline.ToString();
            return "Buy " + BuySome + " " + Product.Name + " Get "+ GetSome + " " + ProdUnderDiscount.Name + " With: " + Percentage + "% Discount Untill: " + Deadline.ToString();
        }
    }
}
