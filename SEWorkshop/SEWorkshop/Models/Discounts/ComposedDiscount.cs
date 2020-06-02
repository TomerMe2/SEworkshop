using System;
using System.Collections.Generic;
using SEWorkshop.Enums;

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
            ComposedParts = (op, dis1, dis2);
            Deadline = dis1.Deadline > dis2.Deadline ? dis2.Deadline : dis1.Deadline;
        }

        public override double ComputeDiscount(ICollection<(Product, int)> itemsList)
        {
            if (ComposedParts != null)
                return ComposedParts.Value.Item1 switch
                {
                    Operator.And => ComposedParts.Value.Item2.ComputeDiscount(itemsList) +
                                    ComposedParts.Value.Item3.ComputeDiscount(itemsList),
                    Operator.Xor => ChooseCheaper(itemsList),
                    Operator.Implies => ApplyImplies(itemsList),
                    _ => throw new Exception("Should not get here"),
                };
            throw new Exception("should not get here");
        }
        
        public double ChooseCheaper(ICollection<(Product, int)> itemsList)
        {
            if (ComposedParts != null)
                return Math.Min(ComposedParts.Value.Item2.ComputeDiscount(itemsList),
                    ComposedParts.Value.Item3.ComputeDiscount(itemsList));
            throw new Exception("should not get here");
        }

        public double ApplyImplies(ICollection<(Product, int)> itemsList)
        {
            if (ComposedParts != null)
            {
                double firstDiscount = ComposedParts.Value.Item2.ComputeDiscount(itemsList);
                if (firstDiscount > 0)
                {
                    return firstDiscount + ComposedParts.Value.Item3.ComputeDiscount(itemsList);
                }
            }
            return 0;
        }
    }
}