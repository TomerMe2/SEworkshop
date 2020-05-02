using NLog;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace SEWorkshop.Models
{
    public class LoggedInUser : User
    {
        public ICollection<Store> Owns { get; private set; }
        public ICollection<Manages> Manage { get; private set; }
        public IList<Review> Reviews { get; private set; }
        public IList<Message> Messages { get; private set; }
        public string Username { get; private set; }
        public byte[] Password { get; private set; }   //it will be SHA256 encrypted password
        private ICollection<Purchase> Purchases { get; set; }
        private ICollection<LoggedInUser> Administrators { get; set; }
        private ICollection<LoggedInUser> Users { get; set; }
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public LoggedInUser(string username, byte[] password)
        {
            Username = username;
            Password = password;
            Owns = new List<Store>();
            Manage = new List<Manages>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
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
        
        public void AddProduct(Store store, string name, string description, string category, double price, int quantity)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.AddProduct(name, description, category, price, quantity);
        }
        
        public void RemoveProduct(Store store, Product productToRemove)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.RemoveProduct(productToRemove);
        }
        
        public void EditProductDescription( Store store, Product product, string description)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.EditProductDescription(product, description);

        }
        
        public void EditProductCategory(Store store, Product product, string category)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.EditProductCategory(product, category);

        }
        
        public void EditProductName(Store store, Product product, string name)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.EditProductName(product, name);

        }
        
        public void EditProductPrice(Store store, Product product, double price)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.EditProductPrice(product, price);

        }
        
        public void EditProductQuantity(Store store, Product product, int quantity)
        {
            var mangement = Manage.FirstOrDefault(man => man.Store.Equals(store));
            mangement.EditProductQuantity(product, quantity);

        }
        
        public void SetPermissionsOfManager(Store store, LoggedInUser manager, Authorizations authorization)
        {
            log.Info("User tries to set permission of {1} of the manager {0} ", manager.Username, authorization);
               
            if (!isManger(store))
                {
                    log.Info("User has no permission for that action");
                    throw new UserHasNoPermissionException();
                }
            var man = manager.Manage.FirstOrDefault(man => man.Store.Equals(store));
            man.SetPermissionsOfManager(manager, authorization);
                return;
        }
        
        public void AddStoreOwner(Store store, LoggedInUser newOwner)
        {
            var management = Manage.FirstOrDefault(man => man.Store.Equals(store));
            management.AddStoreOwner(newOwner);
            
        }
        
        public void AddStoreManager(Store store, LoggedInUser newManager)
        {
            var management = Manage.FirstOrDefault(man => man.Store.Equals(store));
            management.AddStoreManager(newManager);
        }
        
        public void RemoveStoreManager(Store store, LoggedInUser managerToRemove)
        {
            var management = Manage.FirstOrDefault(man => man.Store.Equals(store));
            management.RemoveStoreManager(managerToRemove);
        }

        public void MessageReply(Message message, Store store, string description)
        {
            var management = Manage.FirstOrDefault(man => man.Store.Equals(store));
            management.MessageReply(message, description);
        }

        public IEnumerable<Message> getMassage(Store store)
        {
            var management = Manage.FirstOrDefault(man => man.Store.Equals(store));
            return management.GetMessage();
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(Store store)
        {
            var management = Manage.FirstOrDefault(man => man.Store.Equals(store));
            return management.ViewPurchaseHistory();
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
            if( Manage.FirstOrDefault(man => man.Store.Equals(store)) != default)
            {
                return true;
            }
            
            return false;
        }

      


    }



}