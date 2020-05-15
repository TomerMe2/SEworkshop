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
        
        
        public DataDiscount(Discount discount) : base(discount) { }
        
        public static DataDiscount CreateDataFromDiscount(Discount dis)
        {
            throw new NotImplementedException(); //TODO: implement this.
        }
    }
}
