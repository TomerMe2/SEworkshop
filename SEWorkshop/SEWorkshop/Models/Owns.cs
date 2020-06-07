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
using System.Data.Entity.Validation;

namespace SEWorkshop.Models
{
    public class Owns : AuthorityHandler
    {

        private readonly Logger log = LogManager.GetCurrentClassLogger();

        private Owns() : base()
        {
            LoggedInUser = null!;
            Store = null!;
            Appointer = null!;
            Username = "";
            StoreName = "";
            AppointerName = "";
        }

        public Owns(LoggedInUser loggedInUser, Store store, LoggedInUser appointer) : base(loggedInUser, store, appointer)
        {
            AuthoriztionsOfUser.Add(new Authority(this, Authorizations.Authorizing));
            AuthoriztionsOfUser.Add(new Authority(this, Authorizations.Replying));
            AuthoriztionsOfUser.Add(new Authority(this, Authorizations.Products));
            AuthoriztionsOfUser.Add(new Authority(this, Authorizations.Watching));
            AuthoriztionsOfUser.Add(new Authority(this, Authorizations.Manager));
            AuthoriztionsOfUser.Add(new Authority(this, Authorizations.Owner));
        }

        public void AddStoreOwner(LoggedInUser newOwner)
        {
            log.Info("User tries to add a new owner {0} to store", newOwner.Username);
            OwnershipRequest request = new OwnershipRequest(Store, LoggedInUser, newOwner);
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
            LoggedInUser.OwnershipRequestsFrom.Add(request);
            DatabaseProxy.Instance.OwnershipRequests.Add(request);
            foreach(var answer in request.Answers)
            {
                DatabaseProxy.Instance.OwnershipAnswers.Add(answer);
            }
            DatabaseProxy.Instance.SaveChanges();
            // He wants him to be an owner, cus he suggested that
            request.Answer(LoggedInUser, RequestState.Approved);
            if (request.GetRequestState() == RequestState.Approved)
            {
                if(Store.GetOwnership(newOwner) != null)
                {
                    throw new UserIsAlreadyStoreOwnerException();
                }
                Store.OwnershipRequests.Remove(request);
                newOwner.OwnershipRequests.Remove(request);
                request.Owner.OwnershipRequestsFrom.Remove(request);
                var ownership = new Owns(newOwner, Store, LoggedInUser);
                newOwner.Owns.Add(ownership);
                Store.Ownership.Add(ownership);
                DatabaseProxy.Instance.OwnershipRequests.Remove(request);
                DatabaseProxy.Instance.AuthorityHandlers.Add(ownership);
                DatabaseProxy.Instance.SaveChanges();
            }
        }

        public void AnswerOwnershipRequest(LoggedInUser newOwner, RequestState answer)
        {
            var req = newOwner.OwnershipRequests.FirstOrDefault(request => request.Store == Store);
            if (req != null)
            {
                req.Answer(LoggedInUser, answer);
                if (req.GetRequestState()==RequestState.Approved)
                {
                    if(Store.GetOwnership(newOwner) != null)
                    {
                        throw new UserIsAlreadyStoreOwnerException();
                    }
                    Owns ownership = new Owns(newOwner, Store, LoggedInUser);
                    Store.Ownership.Add(ownership);
                    Store.OwnershipRequests.Remove(req);
                    newOwner.OwnershipRequests.Remove(req);
                    req.Owner.OwnershipRequestsFrom.Remove(req);
                    newOwner.Owns.Add(ownership);
                    DatabaseProxy.Instance.AuthorityHandlers.Add(ownership);
                    DatabaseProxy.Instance.OwnershipRequests.Remove(req);
                    DatabaseProxy.Instance.SaveChanges();
                    log.Info("A new owner has been added successfully");
                }
                else if (req.GetRequestState() == RequestState.Denied)
                {
                    newOwner.OwnershipRequests.Remove(req);
                    Store.OwnershipRequests.Remove(req);
                    req.Owner.OwnershipRequestsFrom.Remove(req);
                    DatabaseProxy.Instance.OwnershipRequests.Remove(req);
                    DatabaseProxy.Instance.SaveChanges();
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
            Manages mangement = new Manages(newManager, Store, LoggedInUser);
            Store.Management.Add(mangement);
            newManager.Manage.Add(mangement);
            DatabaseProxy.Instance.AuthorityHandlers.Add(mangement);
            DatabaseProxy.Instance.SaveChanges();
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
            DatabaseProxy.Instance.AuthorityHandlers.Remove(management);
            DatabaseProxy.Instance.SaveChanges();
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
            DatabaseProxy.Instance.AuthorityHandlers.Remove(ownership);
            DatabaseProxy.Instance.SaveChanges();
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
                DatabaseProxy.Instance.Products.Add(newProduct);
                DatabaseProxy.Instance.SaveChanges();
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
                DatabaseProxy.Instance.Products.Remove(productToRemove);
                DatabaseProxy.Instance.SaveChanges();
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
            DatabaseProxy.Instance.SaveChanges();
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
            DatabaseProxy.Instance.SaveChanges();
            log.Info("Product's category has been modified successfully");
            return;
        }

        override public void EditProductName(Product product, string name)
        {
            log.Info("User tries to modify product's name");

            if (!StoreContainsProduct(product, Store))
            {
                log.Info("Product does not exist in store");
                throw new ProductNotInTradingSystemException();
            }

            if (Store.Products.Any(prod => prod.Name.Equals(name)))
            {
                log.Info("Product name is already taken in store");
                throw new StoreWithThisNameAlreadyExistsException();
            }

            product.Name = name;
            DatabaseProxy.Instance.SaveChanges();
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
                DatabaseProxy.Instance.SaveChanges();
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

            product.Quantity = quantity;
            DatabaseProxy.Instance.SaveChanges();
            log.Info("Product's quantity has been modified successfully");
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
            DatabaseProxy.Instance.Policies.Add(pol);
            DatabaseProxy.Instance.SaveChanges();
        }
        
        //TODO: THIS FUNCTION WITH RESPECTS TO DB.
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
                        DatabaseProxy.Instance.Discounts.Remove(toRemove);
                        DatabaseProxy.Instance.SaveChanges();
                        if (toLeft)
                        {
                            ComposedDiscount newDis = new ComposedDiscount(op, dis, existing);
                            Store.Discounts.Insert(indexInChain, newDis);
                            DatabaseProxy.Instance.Discounts.Add(newDis);
                            DatabaseProxy.Instance.SaveChanges();
                        }
                        else
                        {
                            ComposedDiscount newDis = new ComposedDiscount(op, existing, dis);
                            Store.Discounts.Insert(indexInChain, newDis);
                            DatabaseProxy.Instance.Discounts.Add(newDis);
                            DatabaseProxy.Instance.SaveChanges();
                        }
                    }
                    else
                    {
                        if (toLeft)
                        {
                            if (existing.IsLeftChild())
                            {
                                if (father.Op != null && father.LeftChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, dis, father.LeftChild);
                                    father.LeftChild = newDis;
                                    DatabaseProxy.Instance.Discounts.Add(newDis);
                                    DatabaseProxy.Instance.SaveChanges();
                                }
                            }
                            else
                            {
                                if (father.Op != null && father.RightChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, dis, father.RightChild);
                                    father.RightChild = newDis;
                                    DatabaseProxy.Instance.Discounts.Add(newDis);
                                    DatabaseProxy.Instance.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            if (existing.IsLeftChild())
                            {
                                if (father.Op != null && father.LeftChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, father.LeftChild, dis);
                                    newDis.Father = father;
                                    father.LeftChild = newDis;
                                    DatabaseProxy.Instance.Discounts.Add(newDis);
                                    DatabaseProxy.Instance.SaveChanges();
                                }
                            }
                            else
                            {
                                if (father.Op != null && father.RightChild != null)
                                {
                                    ComposedDiscount newDis = new ComposedDiscount(op, father.RightChild, dis);
                                    newDis.Father = father;
                                    father.RightChild = newDis;
                                    DatabaseProxy.Instance.Discounts.Add(newDis);
                                    DatabaseProxy.Instance.SaveChanges();
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
            if (!(root is ComposedDiscount) || ((ComposedDiscount)root).Op is null ||
                    ((ComposedDiscount)root).LeftChild is null || ((ComposedDiscount)root).RightChild is null)
            {
                return null;
            }

            return SearchNode(((ComposedDiscount)root).LeftChild, disId) ?? SearchNode(((ComposedDiscount)root).RightChild, disId);
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

        public void AddSystemDayPolicy(Operator op, Weekday cantBuyIn)
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
            DatabaseProxy.Instance.Policies.Remove(currPol);
            DatabaseProxy.Instance.SaveChanges();
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
            var toRemove = Store.Discounts.ElementAt(indexInChain);
            Store.Discounts.Remove(toRemove);
            DatabaseProxy.Instance.Discounts.Remove(toRemove);
            DatabaseProxy.Instance.SaveChanges();
        }
    }

}


