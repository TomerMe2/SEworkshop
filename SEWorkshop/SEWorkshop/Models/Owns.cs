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
using SEWorkshop.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Owners")]
    public class Owns : AuthorityHandler
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();
        [ForeignKey("Users")]
        public LoggedInUser Appointer { get; private set;}
        private AppDbContext DbContext { get; }


        public Owns(LoggedInUser loggedInUser, Store store, LoggedInUser appointer, AppDbContext dbContext) : base(dbContext, loggedInUser, store)
        {
            DbContext = dbContext;
            AddAuthorization(Authorizations.Authorizing);
            AddAuthorization(Authorizations.Replying);
            AddAuthorization(Authorizations.Products);
            AddAuthorization(Authorizations.Watching);
            AddAuthorization(Authorizations.Manager);
            AddAuthorization(Authorizations.Owner);
            Appointer = appointer;
        }

        public void AddStoreOwner(LoggedInUser newOwner)
        {
            log.Info("User tries to add a new owner {0} to store", newOwner.Username);
            OwnershipRequest request = new OwnershipRequest(Store, LoggedInUser, newOwner, DbContext);
            if (Store.GetOwnership(newOwner) != null)
            {
                throw new UserIsAlreadyStoreOwnerException();
            }
            if(Store.OwnershipRequests.Contains(request))
            {
                throw new OwnershipRequestAlreadyExistsException();
            }
            Store.OwnershipRequests.Add(request);
            newOwner.OwnershipRequests.Add(request);
            if (request.GetRequestState() == RequestState.Approved)
            {
                if(Store.GetOwnership(newOwner) != null)
                {
                    throw new UserIsAlreadyStoreOwnerException();
                }
                Store.OwnershipRequests.Remove(request);
                Owns ownership = new Owns(newOwner, Store, LoggedInUser, DbContext);
                DbContext.AuthorityHandlers.Add(ownership);
                newOwner.Owns.Add(ownership);
                Store.Ownership.Add(ownership);
            }
        }
        public void AnswerOwnershipRequest(LoggedInUser newOwner, RequestState answer)
        {
            foreach (var req in newOwner.OwnershipRequests)
            {
                if (req.Store == Store)
                {
                    req.Answer(LoggedInUser, answer);
                }
                if (req.GetRequestState()==RequestState.Approved)
                {
                    if(Store.GetOwnership(newOwner) != null)
                    {
                        throw new UserIsAlreadyStoreOwnerException();
                    }
                    Owns ownership = new Owns(newOwner, Store, LoggedInUser, DbContext);
                    Store.Ownership.Add(ownership);
                    Store.OwnershipRequests.Remove(req);
                    newOwner.Owns.Add(ownership);
                    log.Info("A new owner has been added successfully");
                }

                if (req.GetRequestState() == RequestState.Denied)
                {
                    Store.OwnershipRequests.Remove(req);
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
            Manages mangement = new Manages(newManager, Store, LoggedInUser, DbContext);
            Store.Management.Add(mangement);
            newManager.Manage.Add(mangement);
            DbContext.AuthorityHandlers.Add(mangement);
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
        }

        override public void RemoveStoreOwner(LoggedInUser ownerToRemove)
        {
            log.Info("User tries to remove the owner {0} from store", ownerToRemove.Username);
            bool isStoreOwner = IsUserStoreOwner(ownerToRemove, Store);
            if (!isStoreOwner)
            {
                log.Info("The requested owner is not an owner");
                throw new UserIsNotOwnerOfThisStore();
            }
            
            Owns? ownership = Store.GetOwnership(ownerToRemove); 
            if (ownership == null)
            {
                log.Info("The requested owner is not an owner");
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
        }

        public void SetPermissionsOfManager(LoggedInUser manager, Authorizations authorization)
        {
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
            if (!IsUserStoreOwner(manager, Store))
            {
                Manages? management = Store.GetManagement(manager);
                if (management == null || management.Appointer != this.LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }

                var man = manager.Manage.FirstOrDefault(man => man.Store == (Store));
                if (!man.HasAuthorization(authorization))
                {
                    log.Info("Permission has been granted successfully");
                    man.AddAuthorization(authorization);
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

        override public void RemoveProduct(Product productToRemove)
        {
            if (StoreContainsProduct(productToRemove, Store))
            {
                productToRemove.Quantity = 0;   //can't sell it anymore
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
                Manages? management = Store.GetManagement(manager);
                if (management == null || management.Appointer != this.LoggedInUser)
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }

                var man = manager.Manage.FirstOrDefault(man => man.Store == (Store));

                if (!man.HasAuthorization(authorization))
                {
                    log.Info("Permission has been taken away successfully");
                    man.HasAuthorization(authorization);
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
                currPol = currPol.InnerPolicy;
            }
            currPol.InnerPolicy = pol;
            currPol.InnerOperator = op;
            DbContext.Policies.Add(pol);
        }
        
        private void ComposeDiscount(Discount dis, Operator op, int indexInChain, int disId, bool toLeft)
        {
            if (indexInChain >= Store.Discounts.Count || indexInChain < 0)
            {
                Store.Discounts.Add(dis);
            }
            else
            {
                Discount? existing = SearchNode(Store.Discounts.ElementAt(indexInChain), disId);
                if (existing != null)
                {
                    ComposedDiscount? father = existing.Father;
                    if (father is null)
                    {
                        Discount toRemove = Store.Discounts.ElementAt(indexInChain);
                        Store.Discounts.RemoveAt(indexInChain);
                        DbContext.Discounts.Remove(toRemove);
                        if (toLeft)
                        {
                            ComposedDiscount newDis = new ComposedDiscount(op, dis, existing);
                            Store.Discounts.Insert(indexInChain, newDis);
                            DbContext.Discounts.Add(newDis);
                        }
                        else
                        {
                            ComposedDiscount newDis = new ComposedDiscount(op, existing, dis);
                            Store.Discounts.Insert(indexInChain, newDis);
                            DbContext.Discounts.Add(newDis);
                        }
                    }
                    else
                    {
                        if (toLeft)
                        {
                            if (existing.IsLeftChild())
                            {
                                if (father.Op != null && father.leftChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, dis, father.leftChild);
                                    father.leftChild = newDis;
                                    DbContext.Discounts.Add(newDis);
                                }
                            }
                            else
                            {
                                if (father.Op != null && father.rightChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, dis, father.rightChild);
                                    father.rightChild = newDis;
                                    DbContext.Discounts.Add(newDis);
                                }
                            }
                        }
                        else
                        {
                            if (existing.IsLeftChild())
                            {
                                if (father.Op != null && father.leftChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, father.leftChild, dis);
                                    newDis.Father = father;
                                    father.leftChild = newDis;
                                    DbContext.Discounts.Add(newDis);
                                }
                            }
                            else
                            {
                                if (father.Op != null && father.rightChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, father.rightChild, dis);
                                    newDis.Father = father;
                                    father.rightChild = newDis;
                                    DbContext.Discounts.Add(newDis);
                                }
                            }
                        }
                    }
                }
            }
        }

        private Discount? SearchNode(Discount root, int disId)
        {
            if (root.DiscountId == disId)
            {
                return root;
            }
            if (root.Op is null || root.leftChild is null || root.rightChild is null)
            {
                return null;
            }

            return SearchNode(root.leftChild, disId) ?? SearchNode(root.rightChild, disId);
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
                currPol = currPol.InnerPolicy;
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
                    Store.Policy = currPol.InnerPolicy;
                }
            }
            else
            {
                prev.InnerPolicy = currPol.InnerPolicy;
            }
        }

        public void AddProductCategoryDiscount(Operator op, string categoryName, DateTime deadline, double percentage, int indexInChain, int disId, bool toLeft)
        {
            ComposeDiscount(new ProductCategoryDiscount(percentage, deadline, Store, categoryName), op, indexInChain, disId, toLeft);
        }

        public void AddSpecificProductDiscount(Operator op, Product product, DateTime deadline, double percentage, int indexInChain, int disId, bool toLeft)
        {
            ComposeDiscount(new SpecificProducDiscount(percentage, deadline, product, Store), op, indexInChain, disId, toLeft);
        }

        public void AddBuyOverDiscount(Operator op, Product product, DateTime deadline, double percentage, double minSum, int indexInChain, int disId, bool toLeft)
        {
            ComposeDiscount(new BuyOverDiscount(Store, minSum, percentage, deadline, product), op, indexInChain, disId, toLeft);
        }
        public void AddBuySomeGetSomeDiscount(Operator op, Product prod1, Product prod2, DateTime deadline, double percentage, int buySome, int getSome, int indexInChain, int disId, bool toLeft)
        {
            ComposeDiscount(new BuySomeGetSomeDiscount(Store, buySome, getSome, percentage, deadline, prod1, prod2), op, indexInChain, disId, toLeft);
        }


        public void RemoveDiscount(int indexInChain)
        {
            DbContext.Discounts.Remove(Store.Discounts.ElementAt(indexInChain));
            Store.Discounts.Remove(Store.Discounts.ElementAt(indexInChain));
        }
    }

}


