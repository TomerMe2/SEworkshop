using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class User
    {
        public ICollection<Cart> Carts { get; private set; }

        public User()
        {
            Carts = new List<Cart>();
        }

        void StoreInformation(Store store)
        {
            Console.WriteLine("Store Name:\n" + store.Store_name);
            Console.WriteLine("\nStores Owners:\n");
            foreach (var Owner in store.Owners)
            {
                Console.WriteLine(Owner.Username);
            }
            Console.WriteLine("\nStores Managers:\n");
            foreach (var Manager in store.Managers)
            {
                Console.WriteLine(Manager.Username);
            }
            foreach (var Product in store.Products)
            {
                Console.WriteLine(Product.ToString());
            }
        }
    }
}
