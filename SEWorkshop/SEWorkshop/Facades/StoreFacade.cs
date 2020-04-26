using SEWorkshop.Models;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using SEWorkshop.TyposFix;

namespace SEWorkshop.Facades
{
    public class StoreFacade : IStoreFacade
    {
        private static StoreFacade? Instance { get; set; } = null;
        private ICollection<Store> Stores { get; set; }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private StoreFacade()
        {
            Stores = new List<Store>();
        }

        public static StoreFacade GetInstance()
        {
            if (Instance is null)
            {
                Instance = new StoreFacade();
                log.Info("StoreFacade instance was created");
            }
            return Instance;
        }

        /// <summary>
        /// Creates a store and saves it in the fecade
        /// </summary>
        /// <returns>The created store</returns>
        public Store CreateStore(LoggedInUser owner, string storeName)
        {
            log.Info(string.Format("CreateStore was invoked with userName {0}, storeName {1}", owner.Username, storeName));
            Func<Store, bool> StoresWithThisNamePredicate = store => store.Name.Equals(storeName);
            ICollection<Store> StoresWithTheSameName = SearchStore(StoresWithThisNamePredicate);
            if (StoresWithTheSameName.Count > 0)
            {
                log.Info(string.Format("User {0} tried to create a store with name {1} and it's already exist",
                    owner.Username, storeName));
                throw new StoreWithThisNameAlreadyExistsException();
            }
            Store newStore = new Store(owner, storeName);
            Stores.Add(newStore);
            owner.Owns.Add(newStore);
            log.Info(string.Format("User {0} created a store with name {1}",
                    owner.Username, storeName));
            return newStore;
        }

        public ICollection<Store> BrowseStores()
        {
            log.Info("BrowseStore was invoked");
            return (from store in Stores
                    where store.IsOpen
                    select store).ToList();
        }

        /// <summary>
        /// Returns IEnumerable of stores that pred is true for them
        /// </summary>
        public ICollection<Store> SearchStore(Func<Store, bool> pred)
        {
            log.Info("SearchStore was invoked");
            return (from store in BrowseStores()
                    where pred(store)
                    select store).ToList();
        }

        public bool IsProductExists(Product product)
        {
            log.Info("IsProductExists was invoked with product name of {0} in stoer name of {1}", product.Name, product.Store.Name);
            return (from prod in AllActiveProducts()
                    where prod == product
                    select prod).Any();
        }

        public IEnumerable<Product> AllActiveProducts()
        {
            log.Info("AllActiveProduct was invoked");
            return BrowseStores().Aggregate(Enumerable.Empty<Product>(), (acc, store) => Enumerable.Concat(acc, store.Products));
        }

        /// <summary>
        /// Returns IEnumerable of products that pred is true for them
        /// </summary>
        public ICollection<Product> SearchProducts(Func<Product, bool> pred)
        {
            log.Info("SearchProducts was invoked");
            return (from product in AllActiveProducts()
                    where pred(product)
                    select product).ToList();
        }

        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            log.Info("FilterProducts was invoked");
            if (products.Count == 0)
            {
                log.Info("Attemp to filter an empty collection");
                throw new NoProductsToFilterException();

            }
            return (from product in products
                    where pred(product)
                    select product).ToList();
        }
    }
}