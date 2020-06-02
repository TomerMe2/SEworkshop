using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public abstract class DataPrimitiveDiscount : DataDiscount
    {
        public double Percentage => ((PrimitiveDiscount) InnerModel).Percentage;
        public DataPrimitiveDiscount(PrimitiveDiscount discount) : base(discount)
        {
        }
    }
}