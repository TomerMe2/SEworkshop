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
    [Table("Stores")]
    public class Store
    {
        public ICollection<Product> Products { get; private set; }
        public ICollection<Manages> Management { get; private set; }
        public ICollection<Owns> Ownership { get; private set; }
        public ICollection<OwnershipRequest> OwnershipRequests { get; private set; }
        public IList<Message> Messages { get; private set; }
        public IList<Discount> Discounts { get; private set; }
        public bool IsOpen { get; private set; }
        [Key]
        public string Name { get; private set; }
        public Policy Policy { get; set; }
        public ICollection<Purchase> Purchases {get; private set; }
        
        private readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();
        private AppDbContext DbContext { get; }
        
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public Store(LoggedInUser owner, string name, AppDbContext dbContext)
        {
            DbContext = dbContext;
            Products = new List<Product>();
            Ownership = (IList<Owns>)dbContext.AuthorityHandlers.Select(handler => handler is Owns &&
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
                Policy = new AlwaysTruePolicy(this);
            Purchases = (IList<Purchase>)dbContext.Purchases.Select(purhcase => purhcase.Basket.Store != null
                    && purhcase.Basket.Store.Equals(this));
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
    }
}
