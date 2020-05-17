using SEWorkshop.Models.Discounts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels
{
    public abstract class DataDiscount : DataModel<Discount>
    {
        public double Percentage => InnerModel.Percentage;

        public DateTime Deadline => InnerModel.Deadline;

        public DataStore Store { get; }

        public (DataDiscount, Operator)? InnerDiscount
        {
            get
            {
                if (InnerModel.InnerDiscount == null)
                {
                    return null;
                }
                return (CreateDataFromDiscount(InnerModel.InnerDiscount.Value.Item1), InnerModel.InnerDiscount.Value.Item2);
            }
        }

        public DataDiscount(Discount discount) : base(discount)
        {
            Store = new DataStore(InnerModel.Store);
        }
        
        public static DataDiscount CreateDataFromDiscount(Discount dis)
        {
            return dis switch
            {
                SpecificProducDiscount d => new DataOpenDiscount(d),
                ProductCategoryDiscount d => new DataProductCategoryDiscount(d),
                BuyOverDiscount d => new DataBuyOverDiscount(d),
                BuySomeGetSomeFreeDiscount d => new DataBuySomeGetSomeFreeDiscount(d),
                _ => throw new Exception("Should not get here"),
            };
        }
    }
}
