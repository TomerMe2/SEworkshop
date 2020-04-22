using SEWorkshop.Models;
using System;
using System.Collections.Generic;
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

        private IEnumerable<Product> AllActiveProducts()
        {
            return BrowseStores().Aggregate(Enumerable.Empty<Product>(), (acc, store) => Enumerable.Concat(acc, store.Products));
        }

        /// <summary>
        /// Returns IEnumerable of products that pred is true for them
        /// </summary>
        public ICollection<Product> SearchProducts(Func<Product, bool> pred)
        {
            return (from product in AllActiveProducts()
                    where pred(product)
                    select product).ToList();
        }

        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            return (from product in products
                    where pred(product)
                    select product).ToList();
        }

        bool isUserAStoreOwner(LoggedInUser user, Store store) => ((from owner in store.Owners
                                                                    where owner == user
                                                                    select owner).ToList().Count()> 0);


        bool isUserAStoreManager(LoggedInUser user, Store store) => ((from manager in store.Managers
                                                                    where manager == user
                                                                    select manager).ToList().Count() > 0);

        public void AddProduct(LoggedInUser user, Store store, string name, string description, string category, double price)
        {
           
            if (isUserAStoreOwner(user, store))
            {
                Product newProduct = new Product(store, name, description, category, price);
                if (!IsProductExists(newProduct))
                {
                    AllActiveProducts().ToList().Add(newProduct);
                }
            }

        }

        public void RemoveProduct(LoggedInUser user, Store store, Product product)
        {
            if (isUserAStoreOwner(user, store))
            {
                if (IsProductExists(product))
                {
                    AllActiveProducts().ToList().Remove(product);
                }
            }
        }

        public void EditProductPrice(Product product, string description) => product.Description= description;

        public void EditProductCategory(Product product, string category) => product.Category= category;

        public void EditProductName(Product product, string name) => product.Name = name;

        public void EditProductPrice(Product product, double price) => product.Price = price;

        public void SetNewStoreOwner(LoggedInUser user, Store store, LoggedInUser newOwner)
        {
           if( isUserAStoreOwner(user, store)&& isUserAStoreManager(user, store)&& !isUserAStoreOwner(newOwner,store))
            {
                store.Owners.Add(newOwner);
                //TODO set that newOwner was appointed by loggedinUser
            }
        }

        public void SetStoreManager(LoggedInUser loggedinUser, Store store, LoggedInUser newManager)
        {
            if (isUserAStoreManager(loggedinUser, store)&& !isUserAStoreManager(newManager, store))
            {
                store.Managers.Add(newManager);
                //TODO: set that newManager was appointed by loggedinUser
            }
        }

        public void SetPermissionsOfManager(LoggedInUser loggedinUser, Store store, LoggedInUser manager,string newPermission)
        {
            
        }

        public void RemoveStoreManager(LoggedInUser loggedinUser, Store store, LoggedInUser managerToRemove)
        {

        }

        public void ViewPurchaseHistory(LoggedInUser loggedinUser, Store store)
        {

        }
    }
}