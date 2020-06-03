using System;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    [Table("PrimitiveDiscounts")]
    public abstract class PrimitiveDiscount : Discount
    {
        public double Percentage { get; private set; }

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
    }
}