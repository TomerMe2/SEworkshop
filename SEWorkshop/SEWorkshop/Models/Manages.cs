using NLog;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    class Manages
    {
        public LoggedInUser LoggedInUser { get; set; }
        public Store Store { get; set; }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();


        public Manages(LoggedInUser loggedInUser, Store store)
        {
            LoggedInUser = loggedInUser;
            Store = store;
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
                        && LoggedInUser.Manages[Store].Contains(authorization)));
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

        public void EditProductName(LoggedInUser loggedInUser, Store store, Product product, string name)
        {
            log.Info("User tries to modify product's name");
            Product demo = new Product(store, name, "", "", 0, 0);
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
