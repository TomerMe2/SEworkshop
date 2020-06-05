using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public class BuySomeGetSomeDiscount : ConditionalDiscount
    {
        public int BuySome { get; set; }
        public int GetSome { get; set; }
        public Product ProdUnderDiscount { get; set; }

        public BuySomeGetSomeDiscount(Store store, int buySome, int getSome, double percentage, DateTime deadline, Product conditionProd, Product underDiscount) : base(percentage, deadline, conditionProd, store)
        {
            BuySome = buySome;
            GetSome = getSome;
            ProdUnderDiscount = underDiscount;
        }

        public override double ComputeDiscount(ICollection<ProductsInBasket> itemsList)
        {
            foreach (var prod in itemsList)
            {
                if (prod.Product.Equals(Product))
                {
                    if (prod.Quantity >= BuySome)
                    {
                        if (ProdUnderDiscount == Product && GetSome > prod.Quantity-BuySome)
                        {
                            return 0;
                        }
                        foreach (var product in itemsList)
                        {
                            if (product.Product.Equals(ProdUnderDiscount))
                            {
                                if (GetSome == -1)
                                {
                                    return prod.Quantity * product.Product.Price * (Percentage / 100);
                                }
                                return GetSome * product.Product.Price * (Percentage / 100);
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}
