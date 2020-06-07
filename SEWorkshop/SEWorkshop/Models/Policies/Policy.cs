using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    public abstract class Policy
    {
        public virtual int Id { get; set; }
        public virtual string StoreName { get; set; }
        public virtual Policy? OuterPolicy { get; set; }
        public virtual Policy? InnerPolicy { get; set; }
        public virtual Operator? InnerOperator { get; set; }
        public virtual Store Store { get; set; }

        protected Policy()
        {
            Store = null!;
            StoreName = "";
        }
        
        public Policy(Store store)
        {
            Store = store;
            StoreName = store.Name;
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
            Policy other = InnerPolicy;
            return InnerOperator switch
            {
                Operator.And => IsThisPolicySatisfied(user, address) && other.CanPurchase(user, address),
                Operator.Or => IsThisPolicySatisfied(user, address) || other.CanPurchase(user, address),
                Operator.Xor => IsThisPolicySatisfied(user, address) ^ other.CanPurchase(user, address),
                _ => false,  //should never get here
            };
        }

    }
}
