using NLog;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Managers")]
    public class Manages : AuthorityHandler
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        [ForeignKey("Users")]
        public LoggedInUser Appointer { get; private set;}
        private AppDbContext DbContext { get; }

        public Manages(LoggedInUser loggedInUser, Store store, LoggedInUser appointer, AppDbContext dbContext) : base(dbContext, loggedInUser, store)
        {
            DbContext = dbContext;
            AddAuthorization(Authorizations.Watching);
            Appointer = appointer;
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
                Manages? management = Store.GetManagement(managerToRemove);
                if (management == null)
                {
                    log.Info("The requested manager is not a store manager");
                    throw new UserIsNotMangerOfTheStoreException();
                }
                LoggedInUser appointer = management.Appointer;
                if (appointer != LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                Store.Management.Remove(management);
                managerToRemove.Manage.Remove(management);
                DbContext.AuthorityHandlers.Remove(management);
                log.Info("The manager has been removed successfully");
                return;
            }
            else
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
        }

        public override void RemoveStoreOwner(LoggedInUser ownerToRemove)
        {
            log.Info("User tries to remove the owner {0} from store", ownerToRemove.Username);
            bool isStoreOwner = IsUserStoreOwner(ownerToRemove, Store);
            if (!isStoreOwner)
            {
                log.Info("The requested manager is not an owner");
                throw new UserIsNotOwnerOfThisStore();
            }
            if (HasAuthorization(Authorizations.Owner))
            {
                Owns? ownership = Store.GetOwnership(ownerToRemove);
                if (ownership == null)
                {
                    log.Info("The requested manager is not an owner");
                    throw new UserIsNotOwnerOfThisStore();
                }
                LoggedInUser appointer = ownership.Appointer;
                if (appointer != LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
                Store.Ownership.Remove(ownership);
                ownerToRemove.Owns.Remove(ownership);
                DbContext.AuthorityHandlers.Remove(ownership);
                log.Info("The owner has been removed successfully");
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
                    DbContext.Products.Add(newProduct);
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
                    DbContext.Products.Remove(productToRemove);
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
                }
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
            foreach(var authority in AuthoriztionsOfUser)
            {
                if(authority.Authorization == autho)
                    return true;
            }
            return false;
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
                Manages mangement = new Manages(newManager, Store, LoggedInUser, DbContext);
                Store.Management.Add(mangement);
                newManager.Manage.Add(mangement);
                DbContext.AuthorityHandlers.Add(mangement);
                log.Info("A new manager has been added successfully");
                return;

        }

        public void AddStoreOwner(LoggedInUser newOwner)
        {
            log.Info("User tries to add a new owner {0} to store", newOwner.Username);
            if (!HasAuthorization(Authorizations.Owner))
            {
                log.Info("User has no permission for that action");
                throw new UserHasNoPermissionException();
            }
            if (IsUserStoreManager(newOwner, Store) || IsUserStoreOwner(newOwner, Store))
            {
                log.Info("The requested user is already a store manager or owner");
                throw new UserIsAlreadyStoreManagerException();
            }
            Owns owning = new Owns(newOwner, Store, LoggedInUser, DbContext);
            Store.Ownership.Add(owning);
            newOwner.Owns.Add(owning);
            DbContext.AuthorityHandlers.Add(owning);
            log.Info("A new owner has been added successfully");
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
                Manages? management = Store.GetManagement(manager);
                if (management == null || management.Appointer.Username == this.LoggedInUser.Username)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }

                if (!HasAuthorization(authorization))
                {
                    log.Info("Permission has been granted successfully");
                    management.AddAuthorization(authorization);   
                }
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public void RemovePermissionsOfManager(LoggedInUser manager, Authorizations authorization)
        {
            if (!HasAuthorization(Authorizations.Authorizing))
            {
                throw new UserHasNoPermissionException();
            }
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (!IsUserStoreOwner(manager, Store))
            {
                Manages? management = Store.GetManagement(manager);
                if (management == null || management.Appointer.Username == this.LoggedInUser.Username)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }

                if (HasAuthorization(authorization))
                {
                    log.Info("Permission has been granted successfully");
                    management.RemoveAuthorization(authorization);   
                }
                return;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }
    }
}
