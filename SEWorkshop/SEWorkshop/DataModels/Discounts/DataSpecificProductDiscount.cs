using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataSpecificProductDiscount : DataOpenDiscount
    {
        
        private SpecificProducDiscount InnerSpecificProducDiscount { get; }
        
        public DataSpecificProductDiscount(SpecificProducDiscount discount) : base(discount)
        {
            InnerSpecificProducDiscount = discount;
        }

        public override string ToString()
        {
            return Percentage + "% On Product: '"  + this.Product.Name + "' Untill: " + Deadline.ToString();
        }
    }
}