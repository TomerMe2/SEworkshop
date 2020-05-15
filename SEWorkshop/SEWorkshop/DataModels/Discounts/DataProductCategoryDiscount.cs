using SEWorkshop.Models.Discounts;

namespace SEWorkshop.DataModels
{
    public class DataProductCategoryDiscount : DataOpenDiscount
    {
        
        private ProductCategoryDiscount InnerProductCategoryDiscount { get; }
        public string CatUnderDiscount => InnerProductCategoryDiscount.CatUnderDiscount;
        
        public DataProductCategoryDiscount(ProductCategoryDiscount discount) : base(discount)
        {
            InnerProductCategoryDiscount = discount;
        }
    }
}