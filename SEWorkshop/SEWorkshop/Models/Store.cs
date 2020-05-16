using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models.Discounts;
using SEWorkshop.Models.Policies;

namespace SEWorkshop.Models
{
    public class Store
    {
        public ICollection<Product> Products { get; private set; }
        public IDictionary<LoggedInUser, LoggedInUser> Managers { get; private set; }
        public IDictionary<LoggedInUser, LoggedInUser> Owners { get; private set; }
        public IList<Message> Messages { get; private set; }
        public ICollection<Discount> Discounts { get; private set; }
        public bool IsOpen { get; private set; }
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
            Managers = new Dictionary<LoggedInUser, LoggedInUser>();
            Owners = new Dictionary<LoggedInUser, LoggedInUser>();
            Messages = new List<Message>();
            IsOpen = true;
            Discounts = new List<Discount>();
            Name = name;
            Policy = new AlwaysTruePolicy(this);
            Purchases = new List<Purchase>();
            Owns owns = new Owns(owner, this);
            owner.Owns.Add(owns);
            Owners.Add(owner, new LoggedInUser("DEMO", new Byte[0]));
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

        public void PurchaseBasket(ICollection<(Product, int)> itemsList, string creditCardNumber, Address address)
        {
            double totalPrice = 0;
            foreach (var (prod, purchaseQuantity) in itemsList)
            {
                if (prod.Quantity - purchaseQuantity < 0)
                {
                    throw new NegativeInventoryException();
                }
            }

            foreach (var discount in Discounts)
            {
                totalPrice -= discount.ComposeDiscounts(itemsList);
            }
            if (supplyAdapter.CanSupply(itemsList, address)
                && billingAdapter.Bill(itemsList, creditCardNumber, totalPrice))
            {
                supplyAdapter.Supply(itemsList, address);
                // Update the quantity in the product itself
                foreach (var (prod, purchaseQuantity) in itemsList)
                {
                    prod.Quantity = prod.Quantity - purchaseQuantity;
                }
            }
            else
            {
                throw new PurchaseFailedException();
            }
        }
    }
}
