using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class Discount
    {
        public DiscountType DisType { get; private set;}
        public int Code { get; private set;}
        public ICollection<Product> Products { get; private set; }

        public Discount(int code)
        {
            DisType = DiscountType.visible;
            Products = new List<Product>();
            Code = code;
        }
    }
}
