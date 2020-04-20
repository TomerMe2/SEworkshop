using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Administrator : LoggedInUser
    {
        public ICollection<Purchase> PurchasesToView { get; private set; }

        public Administrator(string username, string password) : base(username, password)
        {
            PurchasesToView = new List<Purchase>();
        }
    }
}
