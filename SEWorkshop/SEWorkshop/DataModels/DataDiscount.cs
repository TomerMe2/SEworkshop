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
        public (DataDiscount, Operator)? InnerDiscount
        {
            get
            {
                if (InnerModel.InnerDiscount is null)
                {
                    return null;
                }

                return (CreateDataFromDiscount(InnerModel.InnerDiscount.Value.Item1), InnerModel.InnerDiscount.Value.Item2);
            }
        }

        public double Percentage => InnerModel.Percentage;

        public DateTime Deadline => InnerModel.Deadline;
        
        public DataDiscount(Discount discount) : base(discount) { }
        
        private DataDiscount CreateDataFromDiscount(Discount valueItem1)
        {
            throw new NotImplementedException();
        }
    }
}
