using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;

namespace SEWorkshop.Models.Discounts
{
    public abstract class Discount
    {
        private static int _nextId = 0;
        public int DiscountId;
        public (Operator, Discount, Discount)? ComposedParts;
        public ComposedDiscount? Father;
        
        public DateTime Deadline { get; set; }

        public Store Store { get; }

        public Discount(DateTime deadline, Store store)
        {
            Deadline = deadline;
            Store = store;
            DiscountId = _nextId++;
        }

        public bool IsLeaf()
        {
            return ComposedParts is null;
        }

        public bool IsLeftChild()
        {
            return Father?.ComposedParts?.Item2 == this;
        }

        public abstract double ComputeDiscount(ICollection<(Product, int)> itemsList);
    }
}
