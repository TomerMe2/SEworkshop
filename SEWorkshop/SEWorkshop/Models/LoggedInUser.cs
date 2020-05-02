using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    public class LoggedInUser : User
    {
        public ICollection<Store> Owns { get; private set; }
        public IDictionary<Store, ICollection<Authorizations>> Manages { get; private set; }
        public IList<Review> Reviews { get; private set; }
        public IList<Message> Messages { get; private set; }
        public string Username { get; private set; }
        public byte[] Password { get; private set; }   //it will be SHA256 encrypted password
        private ICollection<Purchase> Purchases { get; set; }
        private ICollection<LoggedInUser> Administrators { get; set; }
        private ICollection<LoggedInUser> Users { get; set; }

        public LoggedInUser(string username, byte[] password)
        {
            Username = username;
            Password = password;
            Owns = new List<Store>();
            Manages = new Dictionary<Store, ICollection<Authorizations>>();
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

        public void AddProduct()
        {
            throw new NotImplementedException();
        }

        public void RemoveProduct()
        {
            throw new NotImplementedException();
        }

        public void Edit()
        {
            throw new NotImplementedException();
        }

        public void AddStoreOwner()
        {
            throw new NotImplementedException();
        }

        public void AddStoreManager()
        {
            throw new NotImplementedException();
        }

        public void SetPermissionOfManager()
        {
            throw new NotImplementedException();
        }

        public void RemoveStoreManager()
        {
            throw new NotImplementedException();
        }

        public void MessageReply(Product product, string description)
        {
            throw new NotImplementedException();
        }

        public void getMassage()
        {
            throw new NotImplementedException();
        }



        public IEnumerable<Purchase> UserPurchaseHistory(string userNmToView)
        {
            if (!Administrators.Contains(this))
            {
                throw new UserHasNoPermissionException();
            }
            var user = Users.Concat(Administrators).FirstOrDefault(user => user.Username.Equals(userNmToView));
            if (user is null)
            {
                throw new UserDoesNotExistException();
            }
            return PurcahseHistory(user);
        }

        public IEnumerable<Purchase> PurcahseHistory(User user)
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
    }





}