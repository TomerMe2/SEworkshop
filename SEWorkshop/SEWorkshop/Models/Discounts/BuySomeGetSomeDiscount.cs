using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    [Table("BuySomeGetSomeDiscounts")]
    public class BuySomeGetSomeDiscount : ConditionalDiscount
    {
        public int BuySome { get; set; }
        public int GetSome { get; set; }
        [ForeignKey("Products")]
        public Product ProdUnderDiscount { get; set; }

        public BuySomeGetSomeDiscount(Store store, int buySome, int getSome, double percentage, DateTime deadline, Product conditionProd, Product underDiscount) : base(percentage, deadline, conditionProd, store)
        {
            BuySome = buySome;
            GetSome = getSome;
            ProdUnderDiscount = underDiscount;
        }

        public override double ComputeDiscount(ICollection<(Product, int)> itemsList)
        {
            foreach (var (product, quantity) in itemsList)
            {
                if (product == Product)
                {
                    if (quantity >= BuySome)
                    {
                        if (ProdUnderDiscount == Product && GetSome > quantity-BuySome)
                        {
                            return 0;
                        }
                        foreach (var (prod, quant) in itemsList)
                        {
                            if (prod == ProdUnderDiscount)
                            {
                                if (GetSome == -1)
                                {
                                    return quant * prod.Price * (Percentage / 100);
                                }
                                return GetSome * prod.Price * (Percentage / 100);
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}
