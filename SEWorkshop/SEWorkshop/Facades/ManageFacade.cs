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
            ICollection<Authorizations>? authorizations;
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            if ((isUserAStoreOwner(loggedInUser, store)
                || (isUserAStoreManager(loggedInUser, store))
                    && loggedInUser.Manages.TryGetValue(store, out authorizations)// ravid explain this to me 
                    && authorizations != null           //user must be logged in to add a product
                    && authorizations.Contains(Authorizations.Products))) { return true; }
            else
            {
                return false;
            }

        }

        public void AddProduct(LoggedInUser loggedInUser, Store store, string name, string description, string category, double price, int quantity)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
           
            if (UserHasPermission(loggedInUser, store, Authorizations.Products)) 
            {
                Product newProduct = new Product(store, name, description, category, price, quantity);
                if (!StoreContainsProduct(store, newProduct))
                {
                    store.Products.Add(newProduct);
                    return;
                }
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(LoggedInUser loggedInUser, Store store, Product productToRemove)
        {
            if (UserHasPermission(loggedInUser,store, Authorizations.Products))
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
                if(!StoreContainsProduct(store, product))
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
                if(!StoreContainsProduct(store, product))
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
                if(!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                if(StoreContainsProduct(store, demo))
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
            if (UserHasPermission(loggedInUser, store, Authorizations.Products))
            {
                if(!StoreContainsProduct(store, product))
                {
                    throw new ProductNotInTradingSystemException();
                }
                product.Price = price;
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
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(LoggedInUser loggedInUser, Store store, LoggedInUser newOwner)
        {
            if(UserHasPermission(loggedInUser,store, Authorizations.Owner)
                && !isUserAStoreOwner(newOwner,store))
            {
                store.Owners.Add(newOwner, loggedInUser);
                newOwner.Owns.Add(store);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreManager(LoggedInUser loggedInUser, Store store, LoggedInUser newManager)
        {
            if (UserHasPermission(loggedInUser, store, Authorizations.Manager)
                && !isUserAStoreOwner(newManager,store))
            {
                store.Managers.Add(newManager, loggedInUser);
                newManager.Manages.Add(store, new List<Authorizations>()
                {
                    Authorizations.Watching
                });
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void SetPermissionsOfManager(LoggedInUser loggedInUser, Store store, LoggedInUser manager, Authorizations authorization)
        {
            ICollection<Authorizations>? authorizations;
            if (UserHasPermission(loggedInUser, store, Authorizations.Authorizing)
                && !isUserAStoreOwner(manager,store))
            {
                if(loggedInUser.Manages.TryGetValue(store, out authorizations)
                        && authorizations.Contains(authorization))
                {
                    authorizations.Remove(authorization);
                }
                else if (authorizations != null && authorizations.Contains(authorization))
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
                && isUserAStoreOwner(managerToRemove,store))
            {
                LoggedInUser? appointer;
                if(!store.Managers.TryGetValue(managerToRemove, out appointer) ||
                    appointer != loggedInUser)
                {
                    throw new UserHasNoPermissionException();
                }
                store.Managers.Remove(managerToRemove);
                managerToRemove.Manages.Remove(store);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Message> ViewMessage(LoggedInUser loggedInUser, Store store)
        {
            if(UserHasPermission(loggedInUser, store, Authorizations.Replying))
            {
                return store.Messages;
            }
            throw new UserHasNoPermissionException();
        }

        public void MessageReply(LoggedInUser loggedInUser, Message message, Store store, string description)
        {
            if(UserHasPermission(loggedInUser, store, Authorizations.Watching))
            {
                Message reply = new Message(loggedInUser, description, message);
                message.Next = reply;
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store)
        {
            if(UserHasPermission(loggedInUser, store, Authorizations.Watching))
            {
                return store.Purchases;
            }
            throw new UserHasNoPermissionException();
        }

        bool isUserAStoreOwner(LoggedInUser user, Store store) => ((from owner in store.Owners
                                                                    where owner.Value == user
                                                                    select owner).ToList().Count()> 0);

        bool isUserAStoreManager(LoggedInUser user, Store store) => ((from manager in store.Managers
                                                                    where manager.Value == user
                                                                    select manager).ToList().Count() > 0);

        public bool StoreContainsProduct(Store store, Product product) => ((from pr in store.Products 
                                                                            where pr.Name== product.Name
                                                                            select product).ToList().Count()> 0);
     }
}