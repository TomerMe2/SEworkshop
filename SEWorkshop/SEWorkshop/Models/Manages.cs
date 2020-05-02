using NLog;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    public class Manages
    {
        public LoggedInUser LoggedInUser { get; set; }
        public Store Store { get; set; }
        public  ICollection<Authorizations> AuthoriztionsOfUser { get; private set; }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();


        public Manages(LoggedInUser loggedInUser, Store store)
        {
            LoggedInUser = loggedInUser;
            Store = store;
            AuthoriztionsOfUser = new List<Authorizations>();
        }

        public bool IsUserStoreOwner() => ((from owner in Store.Owners
                                            where owner.Key == LoggedInUser
                                            select owner).ToList().Count() > 0);

        public bool IsUserStoreManager() => ((from manager in Store.Managers
                                              where manager.Key == LoggedInUser
                                              select manager).ToList().Count() > 0);

        public bool StoreContainsProduct(Product product) => ((from pr in Store.Products
                                                               where pr.Name == product.Name
                                                               select product).ToList().Count() > 0);

        public bool UserHasPermission(Authorizations authorization)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            return (IsUserStoreOwner()
                    || (IsUserStoreManager()
                        && AuthoriztionsOfUser.Contains(authorization)));
        }
        public void AddStoreOwner(LoggedInUser newOwner)
        {
            log.Info("User tries to add a new owner {0} to store", newOwner.Username);
            if (!UserHasPermission( Authorizations.Owner))
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
            if (IsUserStoreOwner())
            {
                log.Info("The requested user is already a store owner");
                throw new UserIsAlreadyStoreOwnerException();
            }
            Store.Owners.Add(newOwner, LoggedInUser);
            newOwner.Owns.Add(Store);
            log.Info("A new owner has been added successfully");
        }
        public void SetPermissionsOfManager(LoggedInUser manager, Authorizations authorization)
        {
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (UserHasPermission(Authorizations.Authorizing)
                && !IsUserStoreOwner())
            {
                if (Store.Managers[manager] != this.LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                var man = manager.Manage.FirstOrDefault(man => man.Store.Equals(Store));

                ICollection<Authorizations> authorizations =man.AuthoriztionsOfUser;
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

        public Product AddProduct(string name, string description, string category, double price, int quantity)
        {
            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            log.Info("User tries to add a new product to store");
            if (UserHasPermission(Authorizations.Products))
            {
                Product newProduct = new Product(Store, name, description, category, price, quantity);
                if (!StoreContainsProduct(newProduct))
                {
                    Store.Products.Add(newProduct);
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
        public void RemoveProduct(Product productToRemove)
        {
            log.Info("User tries to add a new product to store");
            if (UserHasPermission(Authorizations.Products))
            {
                if (StoreContainsProduct(productToRemove))
                {
                    Store.Products.Remove(productToRemove);
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

        public void EditProductDescription(Product product, string description)
        {
            log.Info("User tries to modify product's description");
            if (UserHasPermission( Authorizations.Products))
            {
                if (!StoreContainsProduct(product))
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

        public void EditProductCategory(Product product, string category)
        {
            log.Info("User tries to modify product's category");
            if (UserHasPermission( Authorizations.Products))
            {
                if (!StoreContainsProduct(product))
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

        public void EditProductName( Product product, string name)
        {
            log.Info("User tries to modify product's name");
            Product demo = new Product(Store, name, "", "", 0, 0);
            if (UserHasPermission(Authorizations.Products))
            {
                if (!StoreContainsProduct(product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                if (StoreContainsProduct(demo))
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

        public void EditProductPrice(Product product, double price)
        {
            log.Info("User tries to modify product's price");
            if (UserHasPermission(Authorizations.Products) || price < 0)
            {
                if (!StoreContainsProduct(product))
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

        public void EditProductQuantity(Product product, int quantity)
        {
            log.Info("User tries to modify product's quantity");
            if (UserHasPermission(Authorizations.Products))
            {
                if (!StoreContainsProduct(product))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }

                //TODO : update in store class
                log.Info("Product's quantity has been modified successfully");
                product.Quantity = quantity;
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }



    }
    public enum Authorizations
    {
        Products,
        Owner,
        Manager,
        Authorizing,
        Replying,
        Watching
    }
}
