using NLog;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.Facades;

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
        private ICollection<LoggedInUser> Administrators { get; set; }
        private ICollection<LoggedInUser> Users { get; set; }
        private UserFacade Facade { get; set; }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

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
        
        public LoggedInUser(string username, byte[] password, UserFacade facade)
        {
            Username = username;
            Password = password;
            Owns = new List<Owns>();
            Manage = new List<Manages>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
            Facade = facade;
        }

        public void WriteReview(Product product, string description)
        {
            if (!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
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
            if (!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
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
           // ownership.SetPermissionsOfManager(manager, authorization);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == null)
            {
                ownership.SetPermissionsOfManager(manager, authorization); 
                //store.Managers.Add(manager,manager);
                return;
            }

            management.SetPermissionsOfManager(manager,authorization);
        }

        public void AddStoreOwner(Store store, LoggedInUser newOwner)
        {
            if (Manage.Select(mng => mng.LoggedInUser == newOwner).Any())
            {
                throw new UserIsAlreadyStoreManagerException();
            }
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
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

        public IEnumerable<Message> getMessage(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                return ownership.GetMessage(store, this);
            }

            return management.GetMessage(store, this);
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                return ownership.ViewPurchaseHistory(this, store);
            }

             return management.ViewPurchaseHistory(this, store);
        }


        public IEnumerable<Purchase> PurchaseHistory(User user)
        {
            if (!HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ICollection<Purchase> userPurchases = new List<Purchase>();
            foreach (var purchase in Purchases)
            {
                if (purchase.User == user)
                {
                    userPurchases.Add(purchase);
                }
            }
            return userPurchases;
        }

        public bool isManger(Store store)
        {
            if(Owns.FirstOrDefault(man => man.Store == store) != default)
            {
                return true;
            }
            
            return false;
        }

        override public void Purchase(Basket basket)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            if (HasPermission)
                purchase = new Purchase(this, basket);
            else
                purchase = new Purchase(new GuestUser(), basket);
         
            ICollection<(Product, int)> productsToPurchase= new List<(Product, int)>();
            foreach (var (prod, purchaseQuantity) in basket.Products)
            {
                if (purchaseQuantity <= 0)
                    throw new NegativeQuantityException();
                else
                    productsToPurchase.Add((prod, purchaseQuantity));
            }
            basket.Store.PurchaseBasket(productsToPurchase);
            Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            Purchases.Add(purchase);
            Facade.AddPurchaseToList(purchase);
            // TODO when to add purchase to loggedin user purchase history
        }


    }



}