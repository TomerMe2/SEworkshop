using System;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;

namespace SEWorkshop.Models.Discounts
{
    public abstract class PrimitiveDiscount : Discount
    {
        public virtual double Percentage { get; private set; }

        public PrimitiveDiscount() : base()
        {
            
        }

        public PrimitiveDiscount(double percentage, DateTime deadline, Store store) : base(deadline, store)
        {
            if (!SetDiscountPercentage(percentage))
            {
                throw new IllegalDiscountPercentageException();
            }
        }
        
        public bool SetDiscountPercentage(double percentage)
        {
            if (percentage >= 0 && percentage <= 100)
            {
                Percentage = percentage;
                return true;
            }
            return false;
        }

        public override void SelfDestruct()
        {
            Store.Discounts.Remove(this);
            DatabaseProxy.Instance.Discounts.Remove(this);
            DatabaseProxy.Instance.SaveChanges();
        }
    }
}