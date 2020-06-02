using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataProductCategoryDiscount : DataPrimitiveDiscount
    {
        
        private ProductCategoryDiscount InnerProductCategoryDiscount { get; }
        public string CatUnderDiscount => InnerProductCategoryDiscount.CatUnderDiscount;
        
        public DataProductCategoryDiscount(ProductCategoryDiscount discount) : base(discount)
        {
            InnerProductCategoryDiscount = discount;
        }

        public override string ToString()
        {
            return Percentage + "% On Category: '" + CatUnderDiscount + "' Untill: " + Deadline.ToString();
        }
    }
}