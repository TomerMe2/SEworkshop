using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Discounts
{
    public abstract class BuySomeGetSomeFree : ConditionalDiscount
    {
        public Store Store { get; set; }
        public int BuySome { get; set; }
        public int GetSome{ get; set; }
        public BuySomeGetSomeFree(Store store, int buySome, int getSome,double percentage, DateTime deadline, Product product) : base( percentage, deadline, product)
        {
            Store = store;
            BuySome = getSome;
            GetSome= buySome;
        }

        public override double ApplyDiscount()
        {
            throw new NotImplementedException();
        }

        protected override double ApplyDiscount(Basket basket)
        {
            foreach(var product in basket.Products)
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
