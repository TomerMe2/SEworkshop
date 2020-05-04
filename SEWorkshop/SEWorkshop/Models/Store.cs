using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;

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
        public Policy Policy { get; private set; }
        public ICollection<Purchase> Purchases {get; private set; }
        
        private static readonly IBillingAdapter billingAdapter = new BillingAdapterStub();
        private static readonly ISupplyAdapter supplyAdapter = new SupplyAdapterStub();
        private static readonly ISecurityAdapter securityAdapter = new SecurityAdapter();
        
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public Store(LoggedInUser owner, string name)
        {
            Products = new List<Product>();
            Managers = new Dictionary<LoggedInUser, LoggedInUser>();
            Owners = new Dictionary<LoggedInUser, LoggedInUser>();
            Messages = new List<Message>();
            IsOpen = true;
            Discounts = new List<Discount>();
            Name = name;
            Policy = new Policy();
            Purchases = new List<Purchase>();
            Owns owns = new Owns(owner, this);
            owner.Owns.Add(owns);
            Owners.Add(owner, new LoggedInUser("DEMO", new Byte[0]));
        }

        public void CloseStore()
        {
            IsOpen = false;
        }

        public Product GetProduct(string name) //TODO: I think this is a redundant function and we should remove it (Amit)
        {
            foreach(var product in Products)
            {
                if(product.Name.Equals(name))
                {
                    return product;
                }
            }
            throw new ProductNotInTradingSystemException();
        }
        
        public ICollection<Product> SearchProducts(Func<Product, bool> pred)
        {
            log.Info("SearchProducts was invoked");
            return (from product in Products
                where pred(product)
                select product).ToList();
        }

        public void PurchaseBasket(ICollection<(Product, int)> itemsList)
        {
            const string CREDIT_CARD_NUMBER_STUB = "1234";
            const string CITY_NAME_STUB = "Beer Sheva";
            const string STREET_NAME_STUB = "Shderot Ben Gurion";
            const string HOUSE_NUMBER_STUB = "111";
            /*foreach (var (prod, purchaseQuantity) in itemsList)
            {
                if (purchaseQuantity <= 0)
                {
                    throw new NegativeQuantityException();
                }
            }*/
            foreach (var (prod, purchaseQuantity) in itemsList)
            {
                if (prod.Quantity - purchaseQuantity < 0)
                {
                    throw new NegativeInventoryException();
                }
            }
            if (supplyAdapter.CanSupply(itemsList, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB)
                && billingAdapter.Bill(itemsList, CREDIT_CARD_NUMBER_STUB))
            {
                supplyAdapter.Supply(itemsList, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB);
                // Update the quantity in the product itself
                foreach (var (prod, purchaseQuantity) in itemsList)
                {
                    prod.Quantity = prod.Quantity - purchaseQuantity;
                }
                //TODO ask if user is loggedin add it to user purchases
                // add purchase to store purchase history
                //Purchases.Add(purchase);
            }
            else
            {
                throw new PurchaseFailedException();
            }
        }
    }
}
