using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.Models
{
    public enum Operator
    {
        And,
        Or,
        Xor
    }

    public abstract class Policy
    {
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

        protected abstract bool IsThisPolicySatisfied(User user);

        public bool CanPurchase(User user)
        {
            if (InnerPolicy is null)
            {
                return IsThisPolicySatisfied(user);
            }
            Policy other = InnerPolicy.Value.Item1;
            return InnerPolicy.Value.Item2 switch
            {
                Operator.And => IsThisPolicySatisfied(user) && other.CanPurchase(user),
                Operator.Or => IsThisPolicySatisfied(user) || other.CanPurchase(user),
                Operator.Xor => IsThisPolicySatisfied(user) ^ other.CanPurchase(user),
                _ => false,  //should never get here
            };
        }

    }
}
