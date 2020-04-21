using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop
{
    class StoreFacade
    {
        private static StoreFacade? Instance { get; set; } = null;
        private ICollection<Store> Stores { get; set; }

        private StoreFacade()
        {
            Stores = new List<Store>();
        }

        public static StoreFacade GetInstance()
        {
            if(Instance is null)
            {
                Instance = new StoreFacade();
            }
            return Instance;
        }

        /// <summary>
        /// Creates a store and saves it in the fecade
        /// </summary>
        /// <returns>The created store</returns>
        public Store CreateStore(RegisteredUser owner, string storeName)
        {
            Store newStore = new Store(owner, storeName);
            Stores.Add(newStore);
            return newStore;
        }

        private IEnumerable<Store> OpenStores()
        {
            return from store in Stores
                   where store.IsOpen
                   select store;
        }

        /// <summary>
        /// Returns IEnumerable of products that pred is true for them
        /// </summary>
        public IEnumerable<Product> SearchProduct(Func<Product, bool> pred)
        {
            return from product in OpenStores().Aggregate(Enumerable.Empty<Product>(), (acc, store) => Enumerable.Concat(acc, store.Products))
                   where pred(product)
                   select product;
        }

        /// <summary>
        /// Returns IEnumerable of stores that pred is true for them
        /// </summary>
        public IEnumerable<Store> SearchStore(Func<Store, bool> pred)
        {
            return from store in OpenStores()
                   where pred(store)
                   select store;
        }
        
    }
}
