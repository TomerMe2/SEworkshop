using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NLog;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models.Discounts;
using SEWorkshop.Models.Policies;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;
using System.Data.Entity;

namespace SEWorkshop.Models
{
    public class Store
    {
        public virtual ICollection<Product> Products { get; private set; }
        public virtual ICollection<Manages> Management { get; private set; }
        public virtual ICollection<Owns> Ownership { get; private set; }
        public virtual ICollection<Basket> Baskets { get; private set; }
        public virtual ICollection<OwnershipRequest> OwnershipRequests { get; private set; }
        public virtual IList<Message> Messages { get; set; }
        public virtual IList<Discount> Discounts { get; set; }
        public virtual bool IsOpen { get; private set; }
        public virtual string Name { get; private set; }

        [NotMapped()]
        public virtual Policy Policy
        {
            get
            {
                // The first (and hopefuly only) policy without a father in the list
                return Policies.First(pol => pol.OuterPolicy is null);
            }
        }

        public virtual ICollection<Purchase> Purchases {get; private set; }
        public virtual ICollection<Policy> Policies { get; private set; }
        private readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();
        
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public static Store StoreBuilder(LoggedInUser owner, string name)
        {
            Store newStore = new Store(name);
            var demo = DatabaseProxy.Instance.LoggedInUsers.FirstOrDefault(usr => usr.Username.Equals("DEMO"));
            if (demo == null)
            {
                demo = new LoggedInUser("DEMO", new byte[] { 0 });
                DatabaseProxy.Instance.LoggedInUsers.Add(demo);
            }
            DatabaseProxy.Instance.Stores.Add(newStore);
            DatabaseProxy.Instance.Policies.Add(newStore.Policies.First());   //it will be the first and only policy
            DatabaseProxy.Instance.SaveChanges();

            Owns ownership = new Owns(owner, newStore, demo);
            newStore.Ownership.Add(ownership);
            owner.Owns.Add(ownership);
            DatabaseProxy.Instance.Owns.Add(ownership);
            foreach (var auth in ownership.AuthoriztionsOfUser)
            {
                DatabaseProxy.Instance.Authorities.Add(auth);
            }
            DatabaseProxy.Instance.SaveChanges();
            return newStore;
        }
        public static Store StoreBuilderWithoutDB(LoggedInUser owner, string name)
        {
            Store newStore = new Store(name);
            var demo = new LoggedInUser("DEMO", new byte[] { 0 });

            Owns ownership = new Owns(owner, newStore, demo);
            newStore.Ownership.Add(ownership);
            owner.Owns.Add(ownership);
            return newStore;
        }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Store()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        //TODO: Make this private if possible
        public Store(string name)
        {
            IsOpen = true;
            Ownership = new List<Owns>();
            Management = new List<Manages>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
            Products = new List<Product>();
            Baskets = new List<Basket>();
            Discounts = new List<Discount>();
            Name = name;
            Policies = new List<Policy>();
            var alwaysTrue = new AlwaysTruePolicy(this);
            Policies.Add(alwaysTrue);
            OwnershipRequests = new List<OwnershipRequest>();
        }

        public void CloseStore()
        {
            IsOpen = false;
        }

        public ICollection<Product> SearchProducts(Func<Product, bool> pred)
        {
            log.Info("SearchProducts was invoked");
            return (from product in Products
                where pred(product)
                select product).ToList();
        }

        public void PurchaseBasket(Basket basket, string creditCardNumber, DateTime expirationDate, string cvv, Address address, User user, string username, string id)
        {
            int transactionBillingId = -1;
            int transactionSupplyId = -1;
            if (!Policy.CanPurchase(user, address))
            {
                throw new PolicyIsFalse();
            }
            foreach (var product in basket.Products)
            {
                if (product.Product.Quantity - product.Quantity < 0)
                {
                    throw new NegativeInventoryException();
                }
            }
            using (var transaction = DatabaseProxy.Instance.Database.BeginTransaction())
            {
                try
                {
                    transactionBillingId = billingAdapter.Bill(basket.Products, creditCardNumber, expirationDate, cvv, basket.PriceAfterDiscount(), username, id).Result;
                    transactionSupplyId = supplyAdapter.Supply(basket.Products, address, username).Result;
                    if (supplyAdapter.CanSupply(basket.Products, address) && transactionBillingId > -1 && transactionSupplyId > -1)
                    {
                        // Update the quantity in the product itself
                        foreach (var product in basket.Products)
                        {
                            Product prod = DatabaseProxy.Instance.Products.ToList().FirstOrDefault(prd => prd.Name.Equals(product.Product.Name) &&
                                                                                        prd.StoreName.Equals(product.Product.StoreName));

                            if (prod == default)
                            {
                                prod = product.Product;
                            }

                            prod.Quantity -= product.Quantity;
                        }
                        DatabaseProxy.Instance.SaveChanges();
                    }
                    else
                    {
                        throw new PurchaseFailedException();
                    }
                    transaction.Commit();
                }
                catch
                {
                    try
                    {
                        if (transactionBillingId > -1)
                            billingAdapter.CancelBill(transactionBillingId);
                    }
                    catch
                    {
                        throw new BillingCancellationHasFailedException();
                    }
                    try
                    {
                        if (transactionSupplyId > -1)
                            supplyAdapter.CancelSupply(transactionSupplyId);
                    }
                    catch
                    {
                        throw new SupplyCancellationHasFailedException();
                    }
                    throw new PurchaseFailedException();
                }
            }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public Manages? GetManagement(LoggedInUser manager)
        {
            foreach(var manage in Management)
            {
                if(manager == manage.LoggedInUser)
                {
                    return manage;
                }
            }
            return null;
        }

        public Owns? GetOwnership(LoggedInUser owner)
        {
            foreach(var own in Ownership)
            {
                if(owner == own.LoggedInUser)
                {
                    return own;
                }
            }
            return null;
        }

        public bool RequestExists(LoggedInUser candidate)
        {
            return OwnershipRequests.Where(req => req.NewOwner.Equals(candidate)).Count() > 0;
        }

        public override bool Equals(object? other)
        {
            if (other is Store)
            {
                Store otherStore = (Store)other;
                return otherStore.Name.Equals(Name);
            }
            return false;
        }
    }
}
