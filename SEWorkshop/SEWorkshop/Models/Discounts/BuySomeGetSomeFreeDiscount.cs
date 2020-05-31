using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Discounts
{
    public class BuySomeGetSomeFreeDiscount : ConditionalDiscount
    {
        public int BuySome { get; set; }
        public int GetSome { get; set; }

        public BuySomeGetSomeFreeDiscount(Store store, int buySome, int getSome, double percentage, DateTime deadline, Product product) : base(percentage, deadline, product, store)
        {
            BuySome = buySome;
            GetSome = getSome;
        }

        public override double ComputeDiscount(ICollection<(Product, int)> itemsList)
        {
            foreach (var product in itemsList)
            {
                if (product.Item1 == Product)
                {
                    if (product.Item2 >= BuySome + GetSome)
                    {
                        return product.Item1.Price * GetSome;
                    }
                }
            }
            return 0;
        }
    }
}
