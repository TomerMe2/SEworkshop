using System;
using System.Collections.Generic;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SEWorkshop.Models.Discounts
{
    public class ComposedDiscount : Discount
    {
        public virtual Operator? Op { get; set; }
        public virtual int? LeftChildId { get; set; }
        public virtual int? RightChildId { get; set; }

        public virtual Discount? LeftChild { get; set; }
        public virtual Discount? RightChild {get; set;}

        protected ComposedDiscount() : base()
        {

        }

        public ComposedDiscount(Operator op, Discount dis1, Discount dis2) : base(dis1.Deadline, dis1.Store)
        {
            if ((dis1.Store != dis2.Store) || (dis1.Deadline < DateTime.Today) || (dis2.Deadline < DateTime.Today))
            {
                throw new Exception();
            }

            dis1.Father = this;
            dis1.FatherId = DiscountId;
            dis2.Father = this;
            dis2.FatherId = DiscountId;
            Op = op;

            LeftChild = dis1;
            RightChild = dis2;
            //Childs = new List<Discount>() { dis1, dis2 };
            Deadline = dis1.Deadline > dis2.Deadline ? dis2.Deadline : dis1.Deadline;
        }

        public override double ComputeDiscount(ICollection<ProductsInBasket> itemsList)
        {
            if (Op != null && LeftChild != null && RightChild != null)
                return Op switch
                {
                    Operator.And => LeftChild.ComputeDiscount(itemsList) +
                                    RightChild.ComputeDiscount(itemsList),
                    Operator.Xor => ChooseCheaper(itemsList),
                    Operator.Implies => ApplyImplies(itemsList),
                    _ => throw new Exception("Should not get here"),
                };
            throw new Exception("should not get here");
        }
        
        public double ChooseCheaper(ICollection<ProductsInBasket> itemsList)
        {
            if (Op != null && LeftChild != null && RightChild != null)
                return Math.Min(LeftChild.ComputeDiscount(itemsList),
                    RightChild.ComputeDiscount(itemsList));
            throw new Exception("should not get here");
        }

        public double ApplyImplies(ICollection<ProductsInBasket> itemsList)
        {
            if (Op != null && LeftChild != null && RightChild != null)
            {
                double firstDiscount = LeftChild.ComputeDiscount(itemsList);
                if (firstDiscount > 0)
                {
                    return firstDiscount + RightChild.ComputeDiscount(itemsList);
                }
            }
            return 0;
        }
    }
}