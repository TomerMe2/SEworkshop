using NLog;
using SEWorkshop.Exceptions;
using SEWorkshop.Models.Policies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.Models.Discounts;
using Operator = SEWorkshop.Enums.Operator;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class Owns : AuthorityHandler
    {
        public LoggedInUser LoggedInUser { get; set; }
        public Store Store { get; set; }
        private readonly Logger log = LogManager.GetCurrentClassLogger();


        public Owns(LoggedInUser loggedInUser, Store store) : base()
        {
            AuthoriztionsOfUser.Add(Authorizations.Authorizing);
            AuthoriztionsOfUser.Add(Authorizations.Watching);
            AuthoriztionsOfUser.Add(Authorizations.Manager);
            AuthoriztionsOfUser.Add(Authorizations.Owner);
            LoggedInUser = loggedInUser;
            Store = store;
        }

        public void AddStoreOwner(LoggedInUser newOwner)
        {
            log.Info("User tries to add a new owner {0} to store", newOwner.Username);
            OwnershipRequest request = new OwnershipRequest(Store, LoggedInUser ,newOwner);
            if (Store.Owners.ContainsKey(newOwner))
            {
                throw new UserIsAlreadyStoreOwnerException();
            }
            
            if(!Store.OwnershipRequests.TryAdd(newOwner, LoggedInUser))
            {
                throw new OwnershipRequestAlreadyExistsException();
            }
            newOwner.OwnershipRequests.Add(request);
            if (request.IsApproved())
            {
                if (!Store.Owners.TryAdd(newOwner, LoggedInUser))
                {
                    throw new UserIsAlreadyStoreOwnerException();
                }
                Store.OwnershipRequests.Remove(newOwner);
                Owns ownership = new Owns(newOwner, Store);
                newOwner.Owns.Add(ownership);
                log.Info("A new Owner has been added to store");
            }
            else
            {
                log.Info("A new Owner request has been sent");
            }
        }
        public void AnswerOwnershipRequest(LoggedInUser newOwner, Boolean answer)
        {
            foreach (var req in newOwner.OwnershipRequests)
            {
                if (req.Store == Store)
                {
                    req.Answer(LoggedInUser, answer);
                }
                if (req.IsApproved())
                {
                    if (!Store.Owners.TryAdd(newOwner, LoggedInUser))
                    {
                        throw new UserIsAlreadyStoreOwnerException();
                    }
                    Owns ownership = new Owns(newOwner, Store);
                    Store.OwnershipRequests.Remove(newOwner);
                    newOwner.Owns.Add(ownership);
                    log.Info("A new Owner has been added to store");
                }
            }

        }

        override public void AddStoreManager(LoggedInUser newManager)
        {
            log.Info("User tries to add a new manager {0} to store", newManager.Username);

            if (IsUserStoreManager(newManager, Store) || IsUserStoreOwner(newManager, Store))
            {
                log.Info("The requested user is already a store manager or owner");
                throw new UserIsAlreadyStoreManagerException();
            }

            Store.Managers.Add(newManager, LoggedInUser);
            Manages mangement = new Manages(newManager, Store);
            newManager.Manage.Add(mangement);
            log.Info("A new manager has been added successfully");
        }

        override public void RemoveStoreManager(LoggedInUser managerToRemove)
        {
            log.Info("User tries to remove the manager {0} from store", managerToRemove.Username);
            bool isStoreManager = IsUserStoreManager(managerToRemove, Store);
            if (!isStoreManager)
            {
                log.Info("The requested manager is not a store manager");
                throw new UserIsNotMangerOfTheStoreException();
            }

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
        }

        public void SetPermissionsOfManager(LoggedInUser manager, Authorizations authorization)
        {
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (!IsUserStoreOwner(manager, Store))
            {
                if (Store.Managers[manager] != this.LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }

                var man = manager.Manage.FirstOrDefault(man => man.Store == (Store));

                ICollection<Authorizations> authorizations = man.AuthoriztionsOfUser;
                if (!authorizations.Contains(authorization))
                {
                    log.Info("Permission has been granted successfully");
                    authorizations.Add(authorization);
                }

                return;
            }

            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        override public Product AddProduct(string name, string description, string category, double price, int quantity)
        {
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

        override public void RemoveProduct(Product productToRemove)
        {
            if (StoreContainsProduct(productToRemove, Store))
            {
                productToRemove.Quantity = 0;   //can't sell it anymore
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

        override public void EditProductDescription(Product product, string description)
        {
            if (!StoreContainsProduct(product, Store))
            {
                log.Info("Product does not exist in store");
                throw new ProductNotInTradingSystemException();
            }

            product.Description = description;
            log.Info("Product's description has been modified successfully");
        }

        override public void EditProductCategory(Product product, string category)
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

        override public void EditProductName(Product product, string name)
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
            return;
        }

        override public void EditProductPrice(Product product, double price)
        {
            log.Info("User tries to modify product's price");
            if (price > 0.00)
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

        override public void EditProductQuantity(Product product, int quantity)
        {
            if (!StoreContainsProduct(product, Store))
            {
                log.Info("Product does not exist in store");
                throw new ProductNotInTradingSystemException();
            }

            log.Info("Product's quantity has been modified successfully");
            product.Quantity = quantity;

        }

        public void RemovePermissionsOfManager(LoggedInUser manager, Authorizations authorization)
        {
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (!IsUserStoreOwner(manager, Store))
            {
                if (Store.Managers[manager] != this.LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }

                var man = manager.Manage.FirstOrDefault(man => man.Store == (Store));

                ICollection<Authorizations> authorizations = man.AuthoriztionsOfUser;
                if (authorizations.Contains(authorization))
                {
                    log.Info("Permission has been taken away successfully");
                    authorizations.Remove(authorization);
                }
            }
        }

        private void AddPolicyToEnd(Policy pol, Operator op)
        {
            if (pol.InnerPolicy != null)
            {
                throw new PolicyCauseCycilicError();
            }
            Policy currPol = Store.Policy;
            if (currPol is AlwaysTruePolicy && currPol.InnerPolicy is null)
            {
                //The owner wants different policy, and allways true should be removed
                Store.Policy = pol;
                return;
            }
            while(currPol.InnerPolicy != null)
            {
                currPol = currPol.InnerPolicy.Value.Item1;
            }
            currPol.InnerPolicy = (pol, op);
        }
        
        private void AddDiscountToEnd(Discount dis, Operator op, int indexInChain)
        {
            if (indexInChain >= Store.Discounts.Count || indexInChain < 0)
            {
                Store.Discounts.Add(dis);
            }
            else
            {
                if (dis.InnerDiscount != null)
                {
                    throw new PolicyCauseCycilicError();
                }
                Discount currDis = Store.Discounts.ElementAt(indexInChain);
                while(currDis.InnerDiscount != null)
                {
                    currDis = currDis.InnerDiscount.Value.Item1;
                }
                currDis.InnerDiscount = (dis, op);
            }
        }

        //All add policies are adding to the end
        public void AddAlwaysTruePolicy(Operator op)
        {
            AddPolicyToEnd(new AlwaysTruePolicy(Store), op);
        }

        public void AddSingleProductQuantityPolicy(Operator op, Product product, int minQuantity, int maxQuantity)
        {
            AddPolicyToEnd(new SingleProductQuantityPolicy(Store, product, minQuantity, maxQuantity), op);
        }

        public void AddSystemDayPolicy(Operator op, DayOfWeek cantBuyIn)
        {
            AddPolicyToEnd(new SystemDayPolicy(Store, cantBuyIn), op);
        }

        public void AddUserCityPolicy(Operator op, string requiredCity)
        {
            AddPolicyToEnd(new UserCityPolicy(Store, requiredCity), op);
        }

        public void AddUserCountryPolicy(Operator op, string requiredCountry)
        {
            AddPolicyToEnd(new UserCountryPolicy(Store, requiredCountry), op);
        }

        public void AddWholeStoreQuantityPolicy(Operator op, int minQuantity, int maxQuantity)
        {
            AddPolicyToEnd(new WholeStoreQuantityPolicy(Store, minQuantity, maxQuantity), op);
        }

        public void RemovePolicy(int indexInChain)
        {
            Policy currPol = Store.Policy;
            Policy? prev = null;
            int i = 0;
            while (currPol.InnerPolicy != null && i < indexInChain)
            {
                prev = currPol;
                currPol = currPol.InnerPolicy.Value.Item1;
                i++;
            }
            if (i != indexInChain)
            {
                throw new NoPolicyInTheGivenIndex();
            }
            if (i == 0 || prev == null)
            {
                if (currPol.InnerPolicy == null)
                {
                    Store.Policy = new AlwaysTruePolicy(Store);
                }
                else
                {
                    Store.Policy = currPol.InnerPolicy.Value.Item1;
                }
            }
            else
            {
                prev.InnerPolicy = currPol.InnerPolicy;
            }
        }

        public void AddProductCategoryDiscount(Operator op, string categoryName, DateTime deadline, double percentage, int indexInChain)
        {
            AddDiscountToEnd(new ProductCategoryDiscount(percentage, deadline, Store, categoryName), op, indexInChain);
        }

        public void AddSpecificProductDiscount(Operator op, Product product, DateTime deadline, double percentage, int indexInChain)
        {
            AddDiscountToEnd(new SpecificProducDiscount(percentage, deadline, product, Store), op, indexInChain);
        }

        public void AddBuyOverDiscount(Operator op, Product product, DateTime deadline, double percentage, double minSum, int indexInChain)
        {
            AddDiscountToEnd(new BuyOverDiscount(Store, minSum, percentage, deadline, product), op, indexInChain);
        }
        public void AddBuySomeGetSomeDiscount(Operator op, Product product, DateTime deadline, double percentage, int buySome, int getSome, int indexInChain)
        {
            AddDiscountToEnd(new BuySomeGetSomeFreeDiscount(Store, buySome, getSome, percentage, deadline, product), op, indexInChain);
        }


        public void RemoveDiscount(int indexInChain)
        {
            Store.Discounts.Remove(Store.Discounts.ElementAt(indexInChain));
        }
    }

}


