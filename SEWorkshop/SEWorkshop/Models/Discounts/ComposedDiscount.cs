using System;
using System.Collections.Generic;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public class ComposedDiscount : Discount
    {
        public ComposedDiscount(Operator op, Discount dis1, Discount dis2) : base(dis1.Deadline, dis1.Store)
        {
            if ((dis1.Store != dis2.Store) || (dis1.Deadline < DateTime.Today) || (dis2.Deadline < DateTime.Today))
            {
                throw new Exception();
            }

            dis1.Father = this;
            dis2.Father = this;
            Op = op;
            leftChild = dis1;
            rightChild = dis2;
            Deadline = dis1.Deadline > dis2.Deadline ? dis2.Deadline : dis1.Deadline;
        }

        public override double ComputeDiscount(ICollection<ProductsInBasket> itemsList)
        {
            if (Op != null && leftChild != null && rightChild != null)
                return Op switch
                {
                    Operator.And => leftChild.ComputeDiscount(itemsList) +
                                    rightChild.ComputeDiscount(itemsList),
                    Operator.Xor => ChooseCheaper(itemsList),
                    Operator.Implies => ApplyImplies(itemsList),
                    _ => throw new Exception("Should not get here"),
                };
            throw new Exception("should not get here");
        }
        
        public double ChooseCheaper(ICollection<ProductsInBasket> itemsList)
        {
            if (Op != null && leftChild != null && rightChild != null)
                return Math.Min(leftChild.ComputeDiscount(itemsList),
                    rightChild.ComputeDiscount(itemsList));
            throw new Exception("should not get here");
        }

        public double ApplyImplies(ICollection<ProductsInBasket> itemsList)
        {
            if (Op != null && leftChild != null && rightChild != null)
            {
                double firstDiscount = leftChild.ComputeDiscount(itemsList);
                if (firstDiscount > 0)
                {
                    return firstDiscount + rightChild.ComputeDiscount(itemsList);
                }
            }
            return 0;
        }
    }
}