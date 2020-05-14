using NLog;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.Facades;
using SEWorkshop.Enums;
using System;

namespace SEWorkshop.Models
{
    public class LoggedInUser : User
    {
        public ICollection<Owns> Owns { get; private set; }
        public ICollection<Manages> Manage { get; private set; }
        public IList<Review> Reviews { get; private set; }
        public IList<Message> Messages { get; private set; }
        public string Username { get; private set; }
        public byte[] Password { get; private set; }   //it will be SHA256 encrypted password
        private ICollection<Purchase> Purchases { get; set; }
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public LoggedInUser(string username, byte[] password)
        {
            Username = username;
            Password = password;
            Owns = new List<Owns>();
            Manage = new List<Manages>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
        }

        public void WriteReview(Product product, string description)
        {
            if (description.Length == 0)
            {
                throw new ReviewIsEmptyException();
            }
            Review review = new Review(this, description);
            product.Reviews.Add(review);
            Reviews.Add(review);
        }
       
        public void WriteMessage(Store store, string description)
        {
            if (description.Length == 0)
            {
                throw new MessageIsEmptyException();
            }
            Message message = new Message(this, description);
            store.Messages.Add(message);
            Messages.Add(message);
        }
        
        public Product AddProduct(Store store, string name, string description, string category, double price, int quantity)
        {
            var ownership = Owns.FirstOrDefault(man =>(man.Store.Name==(store.Name)));
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));  
            if(management == null)
            {
                if (ownership == null)
                {
                    throw new UserHasNoPermissionException();
                }
                return ownership.AddProduct(name, description, category, price, quantity);

            }
            return management.AddProduct(name, description, category, price, quantity);
        }
        
        public void RemoveProduct(Store store, Product productToRemove)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.RemoveProduct(productToRemove);
                return;
            }
            management.RemoveProduct(productToRemove);
        }
        
        public void EditProductDescription( Store store, Product product, string description)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
           var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.EditProductDescription(product, description);
                return;
            }
            management.EditProductDescription(product, description);

        }
        
        public void EditProductCategory(Store store, Product product, string category)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.EditProductCategory(product, category);
                return;
            }
            management.EditProductCategory(product, category);
        }
        
        public void EditProductName(Store store, Product product, string name)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.EditProductName(product, name);
                return;
            }
            management.EditProductName(product, name);

        }
        
        public void EditProductPrice(Store store, Product product, double price)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
              var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.EditProductPrice(product, price);
                return;
            }
            else
            {
                management.EditProductPrice(product, price);
            }
        }
        
        public void EditProductQuantity(Store store, Product product, int quantity)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.EditProductQuantity(product, quantity);
                return;
            }
            management.EditProductQuantity(product, quantity);

        }
        
        public void SetPermissionsOfManager(Store store, LoggedInUser manager, Authorizations authorization)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == null)
            {
                ownership.SetPermissionsOfManager(manager, authorization);
                return;
            }

            management.SetPermissionsOfManager(manager,authorization);
        }

        public void AddStoreOwner(Store store, LoggedInUser newOwner)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);


            if (Manage.Select(mng => mng.LoggedInUser == newOwner).Any())
            {
                throw new UserIsAlreadyStoreManagerException();
            }

            ownership.AddStoreOwner(newOwner);
            
        }
        
        public void AddStoreManager(Store store, LoggedInUser newManager)
        {

            var ownership = Owns.FirstOrDefault(man => man.Store == store);
           var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.AddStoreManager(newManager);
                return;
            }
            management.AddStoreManager(newManager);
        }
        
        public void RemoveStoreManager(Store store, LoggedInUser managerToRemove)
        {

            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.RemoveStoreManager(managerToRemove);
                return;
            }
            management.RemoveStoreManager(managerToRemove);
        }

        public Message MessageReply(Message message, Store store, string description)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                return ownership.MessageReply(this, store, message, description);
            }
            
            return management.MessageReply(this,store,message,description);
        }

        public IEnumerable<Message> GetMessage(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                return ownership.GetMessage(store, this);
            }

            return management.GetMessage(store, this);
        }

        public IEnumerable<Purchase> PurchaseHistory(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == null)
            {
                return ownership.ViewPurchaseHistory(this, store);
            }

            return management.ViewPurchaseHistory(this, store);
        }

        override public void Purchase(Basket basket, string creditCardNumber, Address address, UserFacade facade)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            purchase = new Purchase(this, basket);
         
            ICollection<(Product, int)> productsToPurchase= new List<(Product, int)>();
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (purchaseQuantity <= 0)
                    throw new NegativeQuantityException();
                else
                    productsToPurchase.Add((prod, purchaseQuantity));
            }
            basket.Store.PurchaseBasket(productsToPurchase, creditCardNumber, address);
            Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            Purchases.Add(purchase);
            facade.AddPurchaseToList(purchase);
        }


        public void RemovePermissionsOfManager(Store store, LoggedInUser manager, Authorizations authorization)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == null)
            {
                ownership.RemovePermissionsOfManager(manager, authorization);
                return;
            }

            management.RemovePermissionsOfManager(manager,authorization);
        }

        private Owns OwnsForStore(Store store)
        {
            var res = Owns.FirstOrDefault(owns => owns.Store == store);
            if (res == null)
            {
                throw new UserIsNotOwnerOfThisStore();
            }
            return res;

        }

        //All add policies are adding to the end
        public void AddAlwaysTruePolicy(Store store, Operator op)
        {
            OwnsForStore(store).AddAlwaysTruePolicy(op);
        }

        public void AddSingleProductQuantityPolicy(Store store, Operator op, Product product, int minQuantity, int maxQuantity)
        {
            OwnsForStore(store).AddSingleProductQuantityPolicy(op, product, minQuantity, maxQuantity);
        }

        public void AddSystemDayPolicy(Store store, Operator op, DayOfWeek cantBuyIn)
        {
            OwnsForStore(store).AddSystemDayPolicy(op, cantBuyIn);
        }

        public void AddUserCityPolicy(Store store, Operator op, string requiredCity)
        {
            OwnsForStore(store).AddUserCityPolicy(op, requiredCity);
        }

        public void AddUserCountryPolicy(Store store, Operator op, string requiredCountry)
        {
            OwnsForStore(store).AddUserCountryPolicy(op, requiredCountry);
        }

        public void AddWholeStoreQuantityPolicy(Store store, Operator op, int minQuantity, int maxQuantity)
        {
            OwnsForStore(store).AddWholeStoreQuantityPolicy(op, minQuantity, maxQuantity);
        }

        public void RemovePolicy(Store store, int indexInChain)
        {
            OwnsForStore(store).RemovePolicy(indexInChain);
        }
    }
}