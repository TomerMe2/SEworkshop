using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations;

namespace SEWorkshop.Models.Policies
{
    public abstract class Policy
    {
        [Key]
        public int Id {get;}
        public (Policy, Operator)? InnerPolicy { get; set; }
        public Store Store { get; }
        
        public Policy(Store store)
        {
            Store = store;
        }

        protected Basket? GetBasket(User user)
        {
            return user.Cart.Baskets.FirstOrDefault(bskt => bskt.Store == Store);
        }

        protected abstract bool IsThisPolicySatisfied(User user, Address address);

        public bool CanPurchase(User user, Address address)
        {
            if (InnerPolicy is null)
            {
                return IsThisPolicySatisfied(user, address);
            }
            Policy other = InnerPolicy.Value.Item1;
            return InnerPolicy.Value.Item2 switch
            {
                Operator.And => IsThisPolicySatisfied(user, address) && other.CanPurchase(user, address),
                Operator.Or => IsThisPolicySatisfied(user, address) || other.CanPurchase(user, address),
                Operator.Xor => IsThisPolicySatisfied(user, address) ^ other.CanPurchase(user, address),
                _ => false,  //should never get here
            };
        }

    }
}
