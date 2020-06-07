using NLog;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;
using System.Data.Entity.Validation;

namespace SEWorkshop.Models
{

    public abstract class AuthorityHandler
    {
        public virtual string Username { get; set; }
        public virtual LoggedInUser LoggedInUser { get; set; }
        public virtual string StoreName { get; set; }
        public virtual Store Store { get; set; }
        public virtual string AppointerName { get; set; }
        public virtual LoggedInUser Appointer { get; set;}
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public virtual ICollection<Authority> AuthoriztionsOfUser { get; set; }

        protected AuthorityHandler()
        {
            AuthoriztionsOfUser = new List<Authority>();
            Appointer = null!;
            AppointerName = "";
            LoggedInUser = null!;
            Store = null!;
            Username = "";
            StoreName = "";
        }

        public AuthorityHandler(LoggedInUser loggedInUser, Store store, LoggedInUser appointer)
        {
            AuthoriztionsOfUser = new List<Authority>();
            Appointer = appointer;
            AppointerName = appointer.Username;
            LoggedInUser = loggedInUser;
            Store = store;
            Username = loggedInUser.Username;
            StoreName = store.Name;
        }

        public Authority AddAuthorization(Authorizations authorizations)
        {
            Authority authority = new Authority(this, authorizations);
            AuthoriztionsOfUser.Add(authority);
            DatabaseProxy.Instance.Authorities.Add(authority);
            return authority;
        }

        public void RemoveAuthorization(Authorizations authorizations)
        {
            Authority? authority = (DatabaseProxy.Instance.Authorities
                .Where(auth => auth.Authorization == authorizations && auth.AuthHandler.Equals(this))).FirstOrDefault();
            if (authority != null)
            {
                AuthoriztionsOfUser.Remove(authority);
                DatabaseProxy.Instance.Authorities.Remove(authority);
                DatabaseProxy.Instance.SaveChanges();
            }
        }

        public abstract void AddStoreManager(LoggedInUser newManager);
        public abstract void RemoveStoreManager(LoggedInUser newManager);
        public abstract void RemoveStoreOwner(LoggedInUser newOwner);
        abstract public Product AddProduct(string name, string description, string category, double price, int quantity);
        abstract public void RemoveProduct(Product productToRemove);
        abstract public void EditProductDescription(Product product, string description);
        abstract public void EditProductCategory(Product product, string category);
        abstract public void EditProductName(Product product, string name);
        abstract public void EditProductPrice(Product product, double price);
        abstract public void EditProductQuantity(Product product, int quantity);

        public bool IsUserStoreOwner(LoggedInUser loggedInUser, Store store) => ((from owner in store.Ownership
                                                                     where owner.LoggedInUser == loggedInUser
                                                                     select owner).ToList().Count() > 0);

        public bool IsUserStoreManager(LoggedInUser loggedInUser, Store store) => ((from manager in store.Management
                                                                       where manager.LoggedInUser == loggedInUser
                                                                       select manager).ToList().Count() > 0);

        public bool StoreContainsProduct(Product product, Store store) => ((from pr in store.Products
                                                               where pr.Name == product.Name
                                                               select product).ToList().Count() > 0);

        public bool UserHasPermission(Store store ,LoggedInUser loggedInUser, Authorizations authorization)
        {
            var management =loggedInUser.Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));

            // to add a product it is required that the user who want to add the proudct is a store owner or a manager
            return (IsUserStoreOwner(loggedInUser, store)
                    || (IsUserStoreManager(loggedInUser, store)
                        && management.HasAuthorization(authorization)));
        }

        public IEnumerable<Purchase> ViewPurchaseHistory(LoggedInUser loggedInUser, Store store)
        {
            log.Info("User tries to view purchase history of store {0}", store.Name);
            if (UserHasPermission(store, loggedInUser ,Authorizations.Watching))
            {
                log.Info("Data has been fetched successfully");
                return store.Purchases;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Message> GetMessage(Store store, LoggedInUser loggedInUser)
        {
            log.Info("User tries to view messages of store {0}", store.Name);
            if (UserHasPermission(store, loggedInUser, Authorizations.Watching))
            {
                log.Info("Data has been fetched successfully");
                return store.Messages;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }

        public Message MessageReply(LoggedInUser loggedInUser, Store store, Message message, string description)
        {
            log.Info("User tries to reply to a message");
            if (UserHasPermission(store, loggedInUser, Authorizations.Replying))
            {
                Message reply = new Message(loggedInUser, store, description, false, message);
                message.Next = reply;
                log.Info("Reply has been published successfully");
                return reply;
            }
            log.Info("User has no permission for that action");
            throw new UserHasNoPermissionException();
        }
    }
}
