using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Cart
    {
        public ICollection<Basket> Baskets { get; private set; }

        public Cart()
        {
            Baskets = new List<Basket>();
        }
    }
}
