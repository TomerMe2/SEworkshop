using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataComposedDiscount : DataDiscount
    {
        public DataComposedDiscount(Discount discount) : base(discount)
        {
        }
    }
}