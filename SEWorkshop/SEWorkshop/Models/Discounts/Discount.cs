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
    [Table("Discounts")]
    public abstract class Discount
    {
        private static int _nextId = 0;
        public int DiscountId;
        public Operator? Op;
        public Discount? leftChild;
        public Discount? rightChild;
        public ComposedDiscount? Father;
        public DateTime Deadline { get; set; }
        [ForeignKey("Stores")]
        public Store Store { get; }

        public Discount(DateTime deadline, Store store)
        {
            Deadline = deadline;
            Store = store;
            DiscountId = _nextId++;
        }

        public bool IsLeaf()
        {
            return Op != null;
        }

        public bool IsLeftChild()
        {
            return Father?.leftChild == this;
        }

        public abstract double ComputeDiscount(ICollection<ProductsInBasket> itemsList);
    }
}
