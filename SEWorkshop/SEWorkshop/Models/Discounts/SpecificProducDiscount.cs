﻿using System;
using System.Collections.Generic;

namespace SEWorkshop.Models.Discounts
{
    public class SpecificProducDiscount : OpenDiscount
    {
        public SpecificProducDiscount(double percentage, DateTime deadline, 
            Product product, Store store) : 
            base(percentage, deadline, product, store)
        {
            
        }

        public override double ApplyDiscount(ICollection<(Product, int)> itemsList)
        {
            double totalDiscount = 0;
            foreach (var (prod, quantity) in itemsList)
            {
                if (Product == prod)
                {
                    totalDiscount += prod.Price * (1 - Percentage / 100);
                }
            }

            return totalDiscount;
        }
    }
}