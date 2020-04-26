using SEWorkshop.Models;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEWorkshop.Facades
{
    public class ManageFacade : IManageFacade
    {
        private static ManageFacade? Instance { get; set; } = null;

        private ManageFacade()
        {
        }

        public static ManageFacade GetInstance()
        {
            if (Instance is null)
            {
                Instance = new ManageFacade();
            }
            return Instance;
        }

        public bool UserHasPermission(LoggedInUser loggedInUser, Store store, Authorizations authorization)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            return (IsUserStoreOwner(loggedInUser, store)
                    || (IsUserStoreManager(loggedInUser, store)
                        && loggedInUser.Manages[store].Contains(authorization)));
        }

        public Product AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price, int quantity)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager

            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                Product newProduct = new Product(store, name, description, category, price, quantity);
                if (!StoreContainsProduct(store, newProduct))
                {
                    store.Products.Add(newProduct);
                    return newProduct;
                }
                else
                {
                    throw new ProductAlreadyExistException();
                }
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product productToRemove)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (StoreContainsProduct(store, productToRemove))
                {
                    store.Products.Remove(productToRemove);
                    return;
                }
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductDescription(LoggedInUser loggedInUser, Store store, Product product, string description)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                product.Description = description;
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductCategory(LoggedInUser loggedInUser, Store store, Product product, string category)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                product.Category = category;
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductName(LoggedInUser loggedInUser, Store store, Product product, string name)
        {
            Product demo = new Product(store, name, "", "", 0, 0);
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                if (StoreContainsProduct(store, demo))
                {
                    throw new StoreWithThisNameAlreadyExistsException();
                }
                product.Name = name;
                return;
            }
            throw new UserHasNoPermissionException();
        }


        public void EditProductPrice(LoggedInUser loggedInUser, Store store, Product product, double price)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Products)|| price<0)
            {
                if (!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                product.Price = price;
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductQuantity(LoggedInUser loggedInUser, Store store, Product product, int quantity)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                product.Quantity = quantity;
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner)
        {
            if (!UserHasPermission(loggedInUser, store, Authorizations.Owner))
                throw new UserHasNoPermissionException();
            if (IsUserStoreOwner(newOwner, store))
                throw new UserIsAlreadyStoreOwnerException();
            store.Owners.Add(newOwner, loggedInUser);
            newOwner.Owns.Add(store);
        }

        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager)
        {
            if (!UserHasPermission(loggedInUser, store, Authorizations.Manager))
                throw new UserHasNoPermissionException();
            if (IsUserStoreManager(newManager, store) || IsUserStoreOwner(newManager, store))
                throw new UserIsAlreadyStoreManagerException();
            store.Managers.Add(newManager, loggedInUser);
            newManager.Manages.Add(store, new List<Authorizations>()
            {
                Authorizations.Watching
            });
        }

        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Authorizing)
                && !IsUserStoreOwner(manager, store))
            {
                if (!manager.Manages.ContainsKey(store)
                    || store.Managers[manager] != loggedInUser)
                {
                    throw new UserHasNoPermissionException();
                }
                ICollection<Authorizations> authorizations = manager.Manages[store];
                if (authorizations.Contains(authorization))
                {
                    authorizations.Remove(authorization);
                }
                else
                {
                    authorizations.Add(authorization);
                }
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Manager)
                && IsUserStoreManager(managerToRemove,store))
            {
                if (!store.Managers.ContainsKey(managerToRemove))
                {
                    throw new UserHasNoPermissionException();
                }
                LoggedInUser appointer = store.Managers[managerToRemove];
                if(appointer != loggedInUser)
                {
                    throw new UserHasNoPermissionException();
                }
                store.Managers.Remove(managerToRemove);
                managerToRemove.Manages.Remove(store);
                return;
            }
            else
            {
                throw new UserHasNoPermissionException();
            }
        }

        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store)
        {
            if(UserHasPermission(loggedInUser, store, Authorizations.Watching))
            {
                return store.Messages;
            }
            throw new UserHasNoPermissionException();
        }

        public Message MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description)
        {
            if(UserHasPermission(loggedInUser, store, Authorizations.Replying))
            {
                Message reply = new Message(loggedInUser, description, message);
                message.Next = reply;
                return reply;
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Watching))
            {
                return store.Purchases;
            }
            throw new UserHasNoPermissionException();
        }

        public bool IsUserStoreOwner(LoggedInUser user, Store store) => ((from owner in store.Owners
                                                                          where owner.Key == user
                                                                          select owner).ToList().Count() > 0);

        public bool IsUserStoreManager(LoggedInUser user, Store store) => ((from manager in store.Managers
                                                                            where manager.Key == user
                                                                            select manager).ToList().Count() > 0);

        public bool StoreContainsProduct(Store store, Product product) => ((from pr in store.Products
                                                                            where pr.Name == product.Name
                                                                            select product).ToList().Count() > 0);
    }
}