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

namespace SEWorkshop.Models
{
    [Table("Stores")]
    public class Store
    {
        public ICollection<Product> Products { get; private set; }
        public ICollection<Manages> Management { get; private set; }
        public ICollection<Owns> Ownership { get; private set; }
    //    public IDictionary<LoggedInUser, LoggedInUser> Managers { get; private set; }
    //    public IDictionary<LoggedInUser, LoggedInUser> Owners { get; private set; }
        public IDictionary<LoggedInUser, LoggedInUser> OwnershipRequests { get; private set; }
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
        
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public Store(LoggedInUser owner, string name)
        {
            Products = new List<Product>();
            Management = new List<Manages>();
            Ownership = new List<Owns>();
            OwnershipRequests=new Dictionary<LoggedInUser, LoggedInUser>();
            Messages = new List<Message>();
            IsOpen = true;
            Discounts = new List<Discount>();
            Name = name;
            Policy = new AlwaysTruePolicy(this);
            Purchases = new List<Purchase>();
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
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (prod.Quantity - purchaseQuantity < 0)
                {
                    throw new NegativeInventoryException();
                }
            }
            if (supplyAdapter.CanSupply(basket.Products, address)
                && billingAdapter.Bill(basket.Products, creditCardNumber, basket.PriceAfterDiscount()))
            {
                supplyAdapter.Supply(basket.Products, address);
                // Update the quantity in the product itself
                foreach (var (prod, purchaseQuantity) in basket.Products)
                {
                    prod.Quantity = prod.Quantity - purchaseQuantity;
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
    }
}
