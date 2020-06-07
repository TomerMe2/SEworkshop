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

namespace SEWorkshop.Models
{
    public class Store
    {
        public virtual int Id { get; set; }
        public virtual ICollection<Product> Products { get; private set; }
        public virtual ICollection<Manages> Management { get; private set; }
        public virtual ICollection<Owns> Ownership { get; private set; }
        public virtual ICollection<Basket> Baskets { get; private set; }
        public virtual ICollection<OwnershipRequest> OwnershipRequests { get; private set; }
        public virtual IList<Message> Messages { get; private set; }
        public virtual IList<Discount> Discounts { get; private set; }
        public virtual bool IsOpen { get; private set; }
        public virtual string Name { get; private set; }

        [NotMapped()]
        public virtual Policy Policy { get; set; }

        public virtual ICollection<AlwaysTruePolicy> AlwaysTruePolicies { get; set; }
        public virtual ICollection<Purchase> Purchases {get; private set; }
        public virtual ICollection<Policy> Policies { get; private set; }
        private readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();
        
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        private Store()
        {
            Ownership = new List<Owns>();
            Management = new List<Manages>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
            Products = new List<Product>();
            Baskets = new List<Basket>();
            Discounts = new List<Discount>();
            Name = "";
            Policy = null!;
            OwnershipRequests = new List<OwnershipRequest>();
            Policies = new List<Policy>();
            AlwaysTruePolicies = new List<AlwaysTruePolicy>();
        }

        public Store(LoggedInUser owner, string name)
        {
            /*Ownership = (IList<Owns>)dbContext.AuthorityHandlers.Select(handler => handler is Owns &&
                    ((Owns)handler).Store != null && ((Owns)handler).Store.Equals(this));
            Management = (IList<Manages>)dbContext.AuthorityHandlers.Select(handler => handler is Manages &&
                    ((Manages)handler).Store != null && ((Manages)handler).Store.Equals(this));
            OwnershipRequests=new List<OwnershipRequest>();
            Messages = (IList<Message>)dbContext.Messages.Select(message => message.ToStore != null && message.ToStore.Equals(this));
            IsOpen = true;
            Discounts = (IList<Discount>)dbContext.Messages.Select(discount => discount.ToStore != null && discount.ToStore.Equals(this));
            Name = name;
            Policy = ((IList<Policy>)DbContext.Policies.Select(policy => policy.Store.Equals(this)))
                    .OrderByDescending(policy => policy.Id).FirstOrDefault();
            if(Policy == default)
            Purchases = (IList<Purchase>)dbContext.Purchases.Select(purhcase => purhcase.Basket.Store != null
                    && purhcase.Basket.Store.Equals(this));
            Discount = new List<Discount>();
            Policy = new AlwaysTruePolicy(this);*/

            //TODO: FILL OWNERSHIP OUTSIDE OF CONSTRUCTOR
            IsOpen = true;
            Ownership = new List<Owns>();
            Management = new List<Manages>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
            Products = new List<Product>();
            Baskets = new List<Basket>();
            Discounts = new List<Discount>();
            Name = name;
            AlwaysTruePolicies = new List<AlwaysTruePolicy>();
            var alwaysTrue = new AlwaysTruePolicy(this);
            Policy = alwaysTrue;
            AlwaysTruePolicies.Add(alwaysTrue);
            //DatabaseProxy.Instance.Policies.Add(Policy);
            OwnershipRequests = new List<OwnershipRequest>();
            Policies = new List<Policy>();

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

        public void PurchaseBasket(Basket basket, string creditCardNumber, Address address, User user)
        {
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
            if (supplyAdapter.CanSupply(basket.Products, address)
                && billingAdapter.Bill(basket.Products, creditCardNumber, basket.PriceAfterDiscount()))
            {
                supplyAdapter.Supply(basket.Products, address);
                // Update the quantity in the product itself
                foreach (var product in basket.Products)
                {
                    product.Product.Quantity -= product.Quantity;
                }
            }
            else
            {
                throw new PurchaseFailedException();
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

        public bool Equals(object other)
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
