using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Administrator : LoggedInUser
    {
        public ICollection<Purchase> PurchasesToView { get; private set; }

        public Administrator(string username, byte[] password) : base(username, password)
        {
            PurchasesToView = new List<Purchase>();
        }
    }
}
