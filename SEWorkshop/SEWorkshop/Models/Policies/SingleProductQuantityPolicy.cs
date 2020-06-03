using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    [Table("SingleProductQuantityPolicies")]
    public class SingleProductQuantityPolicy : Policy
    {
        [ForeignKey("Products")]
        public Product Prod { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }

        //-1 for quantity is ignoring this quantity
        public SingleProductQuantityPolicy(Store store, Product product, int minQuantity, int maxQuantity) : base(store)
        {
            Prod = product;
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
        }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            Basket? bskt = GetBasket(user);
            if (bskt == null)
            {
                return true;
            }
            foreach(var tup in bskt.Products)
            {
                if (tup.Item1 == Prod)
                {
                    int val = tup.Item2;
                    if (MinQuantity != -1 && MaxQuantity != -1)
                    {
                        return val >= MinQuantity && val <= MaxQuantity;
                    }
                    else if (MinQuantity != -1)
                    {
                        return val >= MinQuantity;
                    }
                    return val <= MaxQuantity;
                }
            }
            return true;   //it's true in an empty sense
        }
    }
}
