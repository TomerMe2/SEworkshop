using NLog;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    public class Manages : AuthorityHandler
    {
        public LoggedInUser LoggedInUser { get; set; }
        public Store Store { get; set; }
        private readonly Logger log = LogManager.GetCurrentClassLogger();


        public Manages(LoggedInUser loggedInUser, Store store)
        {
            AuthoriztionsOfUser=new List<Authorizations>();
            AuthoriztionsOfUser.Add(Authorizations.Watching);
            LoggedInUser = loggedInUser;
            Store = store;
    
        }

     

        public override void RemoveStoreManager(LoggedInUser managerToRemove)
        {
            log.Info("User tries to remove the manager {0} from store", managerToRemove.Username);
            bool isStoreManager = IsUserStoreManager(managerToRemove, Store);
            if (!isStoreManager)
            {
                log.Info("The requested manager is not a store manager");
                throw new UserIsNotMangerOfTheStoreException();
            }
            if (HasAuthorization(Authorizations.Manager))
            {
                if (!Store.Managers.ContainsKey(managerToRemove))
                {
                    log.Info("The requested manager is not a store manager");
                    throw new UserIsNotMangerOfTheStoreException();
                }
                LoggedInUser appointer = Store.Managers[managerToRemove];
                if (appointer != LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                Store.Managers.Remove(managerToRemove);
                var management = managerToRemove.Manage.FirstOrDefault(man => man.Store.Equals(Store));
                managerToRemove.Manage.Remove(management);
                log.Info("The manager has been removed successfully");
                return;
            }
            else
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
        
        }

        public override Product AddProduct(string name, string description, string category, double price, int quantity)
        {
            if (HasAuthorization(Authorizations.Products)) {
                // to add a product it is required that the user who want to add the proudct is a store owner or a manager
                log.Info("User tries to add a new product to store");

                Product newProduct = new Product(Store, name, description, category, price, quantity);
                if (!StoreContainsProduct(newProduct, Store))
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

        public override void RemoveProduct(Product productToRemove)
        {
            log.Info("User tries to add a new product to store");
            if (HasAuthorization(Authorizations.Products))
            {
                if (StoreContainsProduct(productToRemove, Store))
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

        public override void EditProductDescription(Product product, string description)
        {
            log.Info("User tries to modify product's description");
            if (HasAuthorization(Authorizations.Products))
            {
                if (!StoreContainsProduct(product, Store))
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

        public override void EditProductCategory(Product product, string category)
        {
            if (HasAuthorization(Authorizations.Products))
            {
                log.Info("User tries to modify product's category");

                if (!StoreContainsProduct(product, Store))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                product.Category = category;
                log.Info("Product's category has been modified successfully");
                return;
            }
        }

        public override void EditProductName(Product product, string name)
        {
            if (HasAuthorization(Authorizations.Products))
            {
                log.Info("User tries to modify product's name");
                Product demo = new Product(Store, name, "", "", 0, 0);

                if (!StoreContainsProduct(product, Store))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }
                if (StoreContainsProduct(demo, Store))
                {
                    log.Info("Product name is already taken in store");
                    throw new StoreWithThisNameAlreadyExistsException();
                }
                product.Name = name;
                log.Info("Product's category has been modified successfully");
            }
        }
        public override void EditProductPrice(Product product, double price)
        {
            log.Info("User tries to modify product's price");
            if (price < 0 || HasAuthorization(Authorizations.Products))
            {
                if (!StoreContainsProduct(product, Store))
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

        public override void EditProductQuantity(Product product, int quantity)
        {
            if (HasAuthorization(Authorizations.Products)) 
            {
                if (!StoreContainsProduct(product, Store))
                {
                    log.Info("Product does not exist in store");
                    throw new ProductNotInTradingSystemException();
                }       //TODO : update in store class
                log.Info("Product's quantity has been modified successfully");
                product.Quantity = quantity;
            }
            else
            {
                throw new UserHasNoPermissionException();
            }
        }
    
     public bool HasAuthorization(Authorizations autho)
    {
            return AuthoriztionsOfUser.Contains(autho);
    }

        public override void AddStoreManager(LoggedInUser newManager)
        {
           log.Info("User tries to add a new manager {0} to store", newManager.Username);
                if (!HasAuthorization(Authorizations.Manager))
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                if (IsUserStoreManager(newManager, Store) || IsUserStoreOwner(newManager, Store))
                {
                    log.Info("The requested user is already a store manager or owner");
                    throw new UserIsAlreadyStoreManagerException();
                }
                Store.Managers.Add(newManager, LoggedInUser);
                Manages mangement = new Manages(newManager, Store);
                newManager.Manage.Add(mangement);
                log.Info("A new manager has been added successfully");
                return;

        }

     

        public void SetPermissionsOfManager(LoggedInUser manager, Authorizations authorization)
        {
            if (!HasAuthorization(Authorizations.Authorizing))
            {
                throw new UserHasNoPermissionException();
            }
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (!IsUserStoreOwner(manager, Store))
            {
                if (Store.Managers[manager].Username == this.LoggedInUser.Username)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                var man = manager.Manage.FirstOrDefault(man => man.Store.Equals(Store));

                ICollection<Authorizations> authorizations = man.AuthoriztionsOfUser;
                if (authorizations.Contains(authorization))
                {
                    log.Info("Permission has been taken away successfully");
                    //authorizations.Remove(authorization);
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
    }
}
