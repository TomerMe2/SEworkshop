﻿using NUnit.Framework.Interfaces;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Discounts
{
    public abstract class ConditionalDiscount : PrimitiveDiscount
    {
        public virtual string ProdName { get; set; }
        public virtual string ProdStoreName { get; set; }
        public virtual Product Product { get; set; }

        protected ConditionalDiscount() : base()
        {
            ProdName = "";
            ProdStoreName = "";
            Product = null!;
        }

        protected ConditionalDiscount(double percentage, DateTime deadline, Product product, Store store) : base(percentage, deadline, store)
        {
            Product = product;
            ProdName = product.Name;
            ProdStoreName = product.Store.Name;
        }
    }
}
