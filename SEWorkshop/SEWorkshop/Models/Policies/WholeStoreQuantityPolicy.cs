using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    public class WholeStoreQuantityPolicy : Policy
    {
        public virtual int MinQuantity { get; set; }
        public virtual int MaxQuantity { get; set; }

        //-1 for quantity is ignoring this quantity
        public WholeStoreQuantityPolicy(Store store, int minQuantity, int maxQuantity) : base(store)
        {
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
            int counter = 0;
            foreach (var prod in bskt.Products)
            {
                counter += prod.Quantity;
            }
            if (MinQuantity != -1 && MaxQuantity != -1)
            {
                return counter >= MinQuantity && counter <= MaxQuantity;
            }
            else if (MinQuantity != -1)
            {
                return counter >= MinQuantity;
            }
            return counter <= MaxQuantity;
        }

    }
}
