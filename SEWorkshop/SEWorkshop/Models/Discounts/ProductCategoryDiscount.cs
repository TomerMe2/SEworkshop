using System;

namespace SEWorkshop.Models.Discounts
{
    public class ProductCategoryDiscount : OpenDiscount
    {
        public string CatUnderDiscount;
        
        public ProductCategoryDiscount(double percentage, DateTime deadline, 
                                        Product product, Store store, string category, User user) : 
                                                                    base(percentage, deadline, product, store, user)
        {
            CatUnderDiscount = category;
        }

        public override double ApplyDiscount()
        {
            double totalDiscount = 0;

            foreach (var (prod, quantity) in GetBasket().Products)
            {
                if (prod.Category.Equals(CatUnderDiscount))
                {
                    totalDiscount += (prod.Price * quantity) * (Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}