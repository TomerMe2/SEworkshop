using NLog;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;

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
             var ownership =(from owner in Owns
             where owner.Store.Name == store.Name
             select owner).First();

            //var ownership = Owns.FirstOrDefault(man =>(man.Store.Name==(store.Name)));
               if(ownership == default)
            {
                throw new BasketIsEmptyException();
            } 
            return ownership.AddProduct(name, description, category, price, quantity);
        }
        
        public void RemoveProduct(Store store, Product productToRemove)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            
            ownership.RemoveProduct(productToRemove);
        }
        
        public void EditProductDescription( Store store, Product product, string description)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            ownership.EditProductDescription(product, description);

        }
        
        public void EditProductCategory(Store store, Product product, string category)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            ownership.EditProductCategory(product, category);

        }
        
        public void EditProductName(Store store, Product product, string name)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            ownership.EditProductName(product, name);

        }
        
        public void EditProductPrice(Store store, Product product, double price)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            ownership.EditProductPrice(product, price);

        }
        
        public void EditProductQuantity(Store store, Product product, int quantity)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            ownership.EditProductQuantity(product, quantity);

        }
        
        public void SetPermissionsOfManager(Store store, LoggedInUser manager, Authorizations authorization)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            ownership.SetPermissionsOfManager(manager, authorization);
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
            if (ownership == default)
            {
                throw new UserIsNotMangerOfTheStoreException();
            }
                ownership.AddStoreManager(newManager);
        }
        
        public void RemoveStoreManager(Store store, LoggedInUser managerToRemove)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            ownership.RemoveStoreManager(managerToRemove);
        }

        public Message MessageReply(Message message, Store store, string description)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
          //  var management = Manage.FirstOrDefault(man => (man.Store.Equals(store)) && man.AuthoriztionsOfUser.Contains(Authorizations.Replying));
           
            return ownership.MessageReply(message, description);
        }

        public IEnumerable<Message> getMessage(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            return ownership.GetMessage();
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            return ownership.ViewPurchaseHistory();
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

      


    }



}