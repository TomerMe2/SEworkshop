using SEWorkshop.Models;
using SEWorkshop.Exceptions;
using System;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace SEWorkshop.Facades
{
    public class ManageFacade : IManageFacade
    {
        private static ManageFacade? Instance { get; set; } = null;
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

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
            var management = loggedInUser.Manage.FirstOrDefault(man => man.Store.Equals(store));

            return (IsUserStoreOwner(loggedInUser, store)
                    || (IsUserStoreManager(loggedInUser, store)
                        && management.AuthoriztionsOfUser.Contains(Authorizations.Authorizing)));
        }

        public Product AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price, int quantity)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            log.Info("User tries to add a new product to store");
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                Product newProduct = new Product(store, name, description, category, price, quantity);
                if (!StoreContainsProduct(store, newProduct))
                {
                    store.Products.Add(newProduct);
                    log.Info("Product has been added to store successfully");
                    return newProduct;
                }
                else
                {
                    log.Info("Product is already exists in store");
                    throw new ProductAlreadyExistException();
                }
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product productToRemove)
        {
            log.Info("User tries to add a new product to store");
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (StoreContainsProduct(store, productToRemove))
                {
                    store.Products.Remove(productToRemove);
                    log.Info("Product has been removed from store successfully");
                    return;
                }
                else
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTheStoreException();
                }
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void EditProductDescription(LoggedInUser loggedInUser, Store store, Product product, string description)
        {
            log.Info("User tries to modify product's description");
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                product.Description = description;
                log.Info("Product's description has been modified successfully");
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void EditProductCategory(LoggedInUser loggedInUser, Store store, Product product, string category)
        {
            log.Info("User tries to modify product's category");
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                product.Category = category;
                log.Info("Product's category has been modified successfully");
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void EditProductName(LoggedInUser loggedInUser, Store store, Product product, string name)
        {
            log.Info("User tries to modify product's name");
            Product demo = new Product(store, name, "", "", 0, 0);
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                if (StoreContainsProduct(store, demo))
                {
                    log.Info("Product name is already taken in store");
                    throw new StoreWithThisNameAlreadyExistsException();
                }
                product.Name = name;
                log.Info("Product's category has been modified successfully");
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void EditProductPrice(LoggedInUser loggedInUser, Store store, Product product, double price)
        {
            log.Info("User tries to modify product's price");
            if (UserHasPermission(loggedInUser, store, Authorizations.Products)|| price<0)
            {
                if (!StoreContainsProduct(store, product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                product.Price = price;
                log.Info("Product's price has been modified successfully");
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void EditProductQuantity(LoggedInUser loggedInUser, Store store, Product product, int quantity)
        {
            log.Info("User tries to modify product's quantity");
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if (!StoreContainsProduct(store, product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                log.Info("Product's quantity has been modified successfully");
                product.Quantity = quantity;
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner)
        {
            log.Info("User tries to add a new owner {0} to store", newOwner.Username);
            if (!UserHasPermission(loggedInUser, store, Authorizations.Owner))
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
            if (IsUserStoreOwner(newOwner, store))
            {
                log.Info("The requested user is already a store owner");
                throw new UserIsAlreadyStoreOwnerException();
            }
            store.Owners.Add(newOwner, loggedInUser);
            Owns ownership = new Owns(newOwner, store);
            newOwner.Owns.Add(ownership);
            log.Info("A new owner has been added successfully");
        }

        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager)
        {
            log.Info("User tries to add a new manager {0} to store", newManager.Username);
            if (!UserHasPermission(loggedInUser, store, Authorizations.Manager))
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
            if (IsUserStoreManager(newManager, store) || IsUserStoreOwner(newManager, store))
            {
                log.Info("The requested user is already a store manager or owner");
                throw new UserIsAlreadyStoreManagerException();
            }
            store.Managers.Add(newManager, loggedInUser);
            Manages managementToAdd = new Manages(newManager, store);
            var ownership = loggedInUser.Owns.FirstOrDefault(man => man.Store.Equals(store));

            ownership.SetPermissionsOfManager(loggedInUser, Authorizations.Watching);
            newManager.Manage.Add(managementToAdd);
            log.Info("A new manager has been added successfully");
        }

        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization)
        {
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (UserHasPermission(loggedInUser, store, Authorizations.Authorizing)
                && !IsUserStoreOwner(manager, store))
            {
                if ((manager.Manage.FirstOrDefault(man => man.Store.Equals(store)) == default)
                    || store.Managers[manager] != loggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                var man = manager.Manage.FirstOrDefault(man => man.Store.Equals(store));

                ICollection<Authorizations> authorizations = man.AuthoriztionsOfUser;

                if (authorizations.Contains(authorization))
                {
                    log.Info("Permission has been taken away successfully");
                    authorizations.Remove(authorization);
                }
                else
                {
                    log.Info("Permission has been granted successfully");
                    authorizations.Add(authorization);
                }
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void RemoveStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser managerToRemove)
        {
            log.Info("User tries to remove the manager {0} from store", managerToRemove.Username);
            bool isStoreManager = IsUserStoreManager(managerToRemove, store);
            if (!isStoreManager)
            {
                log.Info("The requested manager is not a store manager");
                throw new UserIsNotMangerOfTheStoreException();
            }
            if (UserHasPermission(loggedInUser, store, Authorizations.Manager)
                && IsUserStoreManager(managerToRemove, store))
            {
                if (!store.Managers.ContainsKey(managerToRemove))
                {
                    log.Info("The requested manager is not a store manager");
                    throw new UserIsNotMangerOfTheStoreException();
                }
                LoggedInUser appointer = store.Managers[managerToRemove];
                if(appointer != loggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                store.Managers.Remove(managerToRemove);
                var management = managerToRemove.Manage.FirstOrDefault(man => man.Store.Equals(store));
                managerToRemove.Manage.Remove(management); log.Info("The manager has been removed successfully");
                return;
            }
            else
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
        }

        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store)
        {
            log.Info("User tries to view messages of store {0}", store.Name);
            if(UserHasPermission(loggedInUser, store, Authorizations.Watching))
            {
                log.Info("Data has been fetched successfully");
                return store.Messages;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public Message MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description)
        {
            log.Info("User tries to reply to a message");
            if(UserHasPermission(loggedInUser, store, Authorizations.Replying))
            {
                Message reply = new Message(loggedInUser, description, message);
                message.Next = reply;
                log.Info("Reply has been published successfully");
                return reply;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store)
        {
            log.Info("User tries to view purchase history of store {0}", store.Name);
            if (UserHasPermission(loggedInUser, store, Authorizations.Watching))
            {
                log.Info("Data has been fetched successfully");
                return store.Purchases;
            }
            log.Info("User has no permission for that action");
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