using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataOpenDiscount : DataDiscount
    {
        
        private OpenDiscount InnerOpenDiscount { get; }
        public DataProduct Product => new DataProduct(InnerOpenDiscount.Product);
        
        public DataOpenDiscount(OpenDiscount discount) : base(discount)
        {
            InnerOpenDiscount = discount;
        }
    }
}