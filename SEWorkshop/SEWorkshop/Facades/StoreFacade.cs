using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.Facades
{
    public class StoreFacade : IStoreFacade
    {
        private static StoreFacade? Instance { get; set; } = null;
        private ICollection<Store> Stores { get; set; }

        private StoreFacade()
        {
            Stores = new List<Store>();
        }

        public static StoreFacade GetInstance()
        {
            if (Instance is null)
            {
                Instance = new StoreFacade();
            }
            return Instance;
        }

        /// <summary>
        /// Creates a store and saves it in the fecade
        /// </summary>
        /// <returns>The created store</returns>
        public Store CreateStore(LoggedInUser owner, string storeName)
        {
            Store newStore = new Store(owner, storeName);
            Stores.Add(newStore);
            return newStore;
        }

        public ICollection<Store> BrowseStores()
        {
            return (from store in Stores
                    where store.IsOpen
                    select store).ToList<Store>();
        }

        /// <summary>
        /// Returns IEnumerable of stores that pred is true for them
        /// </summary>
        public ICollection<Store> SearchStore(Func<Store, bool> pred)
        {
            return (from store in BrowseStores()
                where pred(store)
                select store).ToList<Store>();
        }

        public bool IsProductExists(Product product)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns IEnumerable of products that pred is true for them
        /// </summary>
        public ICollection<Product> SearchProducts(Func<Product, bool> pred)
        {
            return (from product in BrowseStores().Aggregate(Enumerable.Empty<Product>(), (acc, store) => Enumerable.Concat(acc, store.Products))
                   where pred(product)
                   select product).ToList<Product>();
        }

        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            throw new NotImplementedException();
        }
    }
}