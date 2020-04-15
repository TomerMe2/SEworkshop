using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Administrator : RegisteredUser
    {
        public ICollection<Purchase> PurchasesToView { get; private set; }

        public Administrator() : base()
        {
            PurchasesToView = new List<Purchase>();
        }
    }
}
