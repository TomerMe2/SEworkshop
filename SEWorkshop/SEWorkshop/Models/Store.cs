using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
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

            Owners.Add(owner, new LoggedInUser("DEMO", new Byte[0]));
        }

        public void CloseStore()
        {
            IsOpen = false;
        }

        public Product GetProduct(string name)
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
        
        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            log.Info("FilterProducts was invoked");
            if (products.Count == 0)
            {
                log.Info("Attemp to filter an empty collection");
                throw new NoProductsToFilterException();

            }
            return (from product in products
                where pred(product)
                select product).ToList();
        }
    }
}
