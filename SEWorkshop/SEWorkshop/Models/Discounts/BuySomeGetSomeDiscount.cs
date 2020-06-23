using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public class BuySomeGetSomeDiscount : ConditionalDiscount
    {
        public virtual int BuySome { get; set; }
        public virtual int GetSome { get; set; }
        public virtual int ProductIdUnderDiscount { get; set; }
        public virtual Product ProdUnderDiscount { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public BuySomeGetSomeDiscount() : base()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        public BuySomeGetSomeDiscount(Store store, int buySome, int getSome, double percentage, DateTime deadline, Product conditionProd, Product underDiscount) : base(percentage, deadline, conditionProd, store)
        {
            BuySome = buySome;
            GetSome = getSome;
            ProdUnderDiscount = underDiscount;
            ProductIdUnderDiscount = conditionProd.Id;
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
