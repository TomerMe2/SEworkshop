using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    public class SingleProductQuantityPolicy : Policy
    {
        public virtual int ProductId { get; set; }
        public virtual Product Prod { get; set; }
        public virtual int MinQuantity { get; set; }
        public virtual int MaxQuantity { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public SingleProductQuantityPolicy() : base()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }


        //-1 for quantity is ignoring this quantity
        public SingleProductQuantityPolicy(Store store, Product product, int minQuantity, int maxQuantity) : base(store)
        {
            Prod = product;
            MinQuantity = minQuantity;
            MaxQuantity = maxQuantity;
            ProductId = product.Id;
        }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            Basket? bskt = GetBasket(user);
            if (bskt == null)
            {
                return true;
            }
            foreach(var prod in bskt.Products)
            {
                if (prod.Product.Equals(Prod))
                {
                    int val = prod.Quantity;
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
