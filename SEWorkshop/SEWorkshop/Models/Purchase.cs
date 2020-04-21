using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    public class Purchase
    {
        public Basket Basket { get; private set; }
        public DateTime TimeStamp { get; private set; }
        public Purchase(Basket basket)
        {
            Basket = basket;
            TimeStamp = DateTime.Now;
        }
    }
}
