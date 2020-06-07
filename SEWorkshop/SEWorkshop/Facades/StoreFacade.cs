using SEWorkshop.Models;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.DAL;
using NLog;
using SEWorkshop.TyposFix;

namespace SEWorkshop.Facades
{
    public class StoreFacade : IStoreFacade
    {
        //private ICollection<Store> Stores { get; set; }

        public StoreFacade()
        {
            //Stores = new List<Store>();
        }

        /// <summary>
        /// Creates a store and saves it in the fecade
        /// </summary>
        /// <returns>The created store</returns>
        public Store CreateStore(LoggedInUser owner, string storeName)
        {
            Func<Store, bool> StoresWithThisNamePredicate = store => store.Name.Equals(storeName);
            ICollection<Store> StoresWithTheSameName = SearchStore(StoresWithThisNamePredicate);
            if (StoresWithTheSameName.Count > 0)
            {
                throw new StoreWithThisNameAlreadyExistsException();
            }
            Store newStore = new Store(owner, storeName);
            var demo = DatabaseProxy.Instance.LoggedInUsers.FirstOrDefault(usr => usr.Username.Equals("DEMO"));
            if (demo == null)
            {
                demo = new LoggedInUser("DEMO", new byte[] { 0 });
                DatabaseProxy.Instance.LoggedInUsers.Add(demo);
            }
            DatabaseProxy.Instance.Stores.Add(newStore);
            DatabaseProxy.Instance.Policies.Add(newStore.AlwaysTruePolicies.First());
            DatabaseProxy.Instance.SaveChanges();

            Owns ownership = new Owns(owner, newStore, demo);
            newStore.Ownership.Add(ownership);
            owner.Owns.Add(ownership);
            DatabaseProxy.Instance.AuthorityHandlers.Add(ownership);
            foreach(var auth in ownership.AuthoriztionsOfUser)
            {
                DatabaseProxy.Instance.Authorities.Add(auth);
            }
            DatabaseProxy.Instance.SaveChanges();

            //TODO: CHECK THIS WITH RAVID. LIKE CODE IN Manages.cs line 293
            //Owns newOwnership = new Owns(owner, newStore, demo);
            //owner.Owns.Add(newOwnership);
            //newStore.Ownership.Add(newOwnership);
            //DatabaseProxy.Instance.AuthorityHandlers.Add(newOwnership);
            //DatabaseProxy.Instance.SaveChanges();
            return newStore;
        }

        public ICollection<Store> BrowseStores()
        {
            return (from store in DatabaseProxy.Instance.Stores
                    where store.IsOpen
                    select store).ToList();
        }

        /// <summary>
        /// Returns IEnumerable of stores that pred is true for them
        /// </summary>
        public ICollection<Store> SearchStore(Func<Store, bool> pred)
        {
            return (from store in BrowseStores()
                    where pred(store)
                    select store).ToList();
        }

        public bool IsProductExists(Product product)
        {
            return (from prod in AllActiveProducts()
                    where prod == product
                    select prod).Any();
        }

        public IEnumerable<Product> AllActiveProducts()
        {
            return BrowseStores().Aggregate(Enumerable.Empty<Product>(), (acc, store) => Enumerable.Concat(acc, store.Products.ToList()));
        }

        /// <summary>
        /// Returns IEnumerable of products that pred is true for them
        /// </summary>
        public ICollection<Product> SearchProducts(Func<Product, bool> pred)
        {
            ICollection<Product> searchResult = new List<Product>();
            foreach (var store in DatabaseProxy.Instance.Stores)
            {
                foreach (var prod in store.SearchProducts(pred))
                {
                    searchResult.Add(prod);
                }
            }
            return (from product in AllActiveProducts()
                    where pred(product)
                    select product).ToList();
        }

        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            if (products.Count == 0)
            {
                throw new NoProductsToFilterException();
            }
            return (from product in products
                    where pred(product)
                    select product).ToList();
        }
    }
}