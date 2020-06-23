using SEWorkshop.Models.Discounts;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels
{
    public class DataComposedDiscount : DataDiscount
    {
        public DataComposedDiscount(Discount discount) : base(discount)
        {
        }
    }
}