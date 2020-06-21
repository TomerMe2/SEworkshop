using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Administrator : LoggedInUser
    {
        public ICollection<Purchase> PurchasesToView { get; private set; }
        public int GeustCounter { get; private set; }
        public int UsersCounter { get; private set; }
        public int StoreOwnerCounter { get; private set; }
        public int StoreManagerCounter { get; private set; }
        public int OnlyStoreOwnerCounter { get; private set; }
        public int OnlyStoreManagerCounter { get; private set; }

        public Administrator(string username, byte[] password) : base(username, password)
        {
            PurchasesToView = new List<Purchase>();

        }

    }
}
