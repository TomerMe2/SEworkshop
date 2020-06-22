using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public abstract class Discount
    {
        public virtual int DiscountId { get; set; }
        public virtual string StoreName { get; set; }
        public virtual ComposedDiscount? Father { get; set; }
        public virtual int? FatherId { get; set; }
        public virtual DateTime Deadline { get; set; }
        public virtual Store Store { get; set; }

        public Discount()
        {
            /*Deadline = default;
            Store = null!;
            StoreName = "";*/
        }

        public Discount(DateTime deadline, Store store)
        {
            Deadline = deadline;
            Store = store;
            StoreName = store.Name;
        }

        public bool IsLeaf()
        {
            return !(this is ComposedDiscount);
        }

        public bool IsLeftChild()
        {
            return Father?.LeftChild == this;
        }

        public abstract double ComputeDiscount(ICollection<ProductsInBasket> itemsList);

        public abstract void SelfDestruct();
    }
}
