using SEWorkshop.Models;
using SEWorkshop.Exceptions;
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
            Func<Store, bool> StoresWithThisNamePredicate = store => store.Name.Equals(storeName);
            ICollection<Store> StoresWithTheSameName = SearchStore(StoresWithThisNamePredicate);
            if (StoresWithTheSameName.Count > 0)
                throw new StoreWithThisNameAlreadyExistsException();
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
            if (products.Count == 0)
                throw new NoProductsToFilterException();
            return (from product in products
                    where pred(product)
                    select product).ToList();
        }

        public void AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price)
        {
            ICollection<Authorizations> authorizations;
            if (isUserAStoreOwner(loggedInUser, store)
                || (isUserAStoreManager(loggedInUser, store)
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)
                    && authorizations.Contains(Authorizations.Products)))
            {
                Product newProduct = new Product(store, name, description, category, price);
                if (!IsProductExists(newProduct))
                {
                    AllActiveProducts().ToList().Add(newProduct);
                    return;
                }
            }
            throw new Exception();
        }

        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product product)
        {
            ICollection<Authorizations> authorizations;
            if (isUserAStoreOwner(loggedInUser, store)
                || (isUserAStoreManager(loggedInUser, store)
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)
                    && authorizations.Contains(Authorizations.Products)))
            {
                if (IsProductExists(product))
                {
                    AllActiveProducts().ToList().Remove(product);
                    return;
                }
            }
            throw new Exception();
        }

        public void AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner)
        {
            ICollection<Authorizations> authorizations;
            if((isUserAStoreOwner(loggedInUser, store)
                || (isUserAStoreManager(loggedInUser, store)
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)
                    && authorizations.Contains(Authorizations.Owner)))
                && !isUserAStoreOwner(newOwner,store))
            {
                store.Owners.Add(newOwner, loggedInUser);
                newOwner.Owns.Add(store);
                return;
            }
            throw new Exception();
        }

        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager)
        {
            ICollection<Authorizations> authorizations;
            if ((isUserAStoreOwner(loggedInUser, store)
                    || (isUserAStoreManager(loggedInUser, store)
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)
                    && authorizations.Contains(Authorizations.Manager)))
                && !isUserAStoreOwner(newManager,store))
            {
                store.Managers.Add(newManager, loggedInUser);
                newManager.Manages.Add(store, new List<Authorizations>()
                {
                    Authorizations.Watching
                });
                return;
            }
            throw new Exception();
        }

        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization)
        {
            ICollection<Authorizations> authorizations;
            if ((isUserAStoreOwner(loggedInUser, store)
                    || (isUserAStoreManager(loggedInUser, store)
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)
                    && authorizations.Contains(Authorizations.Authorizing)))
                && !isUserAStoreOwner(manager,store))
            {
                if(loggedInUser.Manages.TryGetValue(store, out authorizations)
                        && authorizations.Contains(authorization))
                    {
                        authorizations.Remove(authorization);
                    }
                    else if (authorizations.Contains(authorization))
                    {
                        authorizations.Add(authorization);
                    }
                return;
            }
            throw new Exception();
        }

        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove)
        {
            ICollection<Authorizations> authorizations;
            if ((isUserAStoreOwner(loggedInUser, store)
                    || (isUserAStoreManager(loggedInUser, store)
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)
                    && authorizations.Contains(Authorizations.Manager)))
                && isUserAStoreOwner(managerToRemove,store))
            {
                LoggedInUser appointer;
                if(!store.Managers.TryGetValue(managerToRemove, out appointer) ||
                    appointer != loggedInUser)
                {
                    throw new Exception();
                }
                store.Managers.Remove(managerToRemove);
                managerToRemove.Manages.Remove(store);
            }
            throw new Exception();
        }
        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store)
        {
            ICollection<Authorizations> authorizations;
            if(isUserAStoreOwner(loggedInUser, store) ||
                (isUserAStoreManager(loggedInUser, store) &&
                loggedInUser.Manages.TryGetValue(store, out authorizations)
                && authorizations.Contains(Authorizations.Replying)))
            {
                return store.Messages;
            }
            throw new Exception();
        }

        public void MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description)
        {
            ICollection<Authorizations> authorizations;
            if(isUserAStoreOwner(loggedInUser, store) ||
                (isUserAStoreManager(loggedInUser, store) &&
                loggedInUser.Manages.TryGetValue(store, out authorizations)
                && authorizations.Contains(Authorizations.Replying)))
            {
                Message reply = new Message(loggedInUser, description, message);
                message.Next = reply;
            }
            throw new Exception();
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store)
        {
            ICollection<Authorizations> authorizations;
            if(isUserAStoreOwner(loggedInUser, store) ||
                (isUserAStoreManager(loggedInUser, store) &&
                loggedInUser.Manages.TryGetValue(store, out authorizations)
                && authorizations.Contains(Authorizations.Replying)))
            {
                return store.Purchases;
            }
            throw new Exception();
        }

        bool isUserAStoreOwner(LoggedInUser user, Store store) => ((from owner in store.Owners
                                                                    where owner.Value == user
                                                                    select owner).ToList().Count()> 0);

        bool isUserAStoreManager(LoggedInUser user, Store store) => ((from manager in store.Managers
                                                                    where manager.Value == user
                                                                    select manager).ToList().Count() > 0);

        public void EditProductPrice(Product product, string description) => product.Description= description;

        public void EditProductCategory(Product product, string category) => product.Category= category;

        public void EditProductName(Product product, string name) => product.Name = name;

        public void EditProductPrice(Product product, double price) => product.Price = price;

    }
}