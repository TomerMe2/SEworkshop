using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataPrimitiveDiscount : DataDiscount
    {
        public double Percentage => ((PrimitiveDiscount) InnerModel).Percentage;
        public DataPrimitiveDiscount(Discount discount) : base(discount)
        {
        }
    }
}