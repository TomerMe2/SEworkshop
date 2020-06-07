using NLog;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.Facades;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;
using System.Data.Entity;
using System;
using Microsoft.VisualBasic;

namespace SEWorkshop.Models
{
    public class LoggedInUser : User
    {
        public virtual ICollection<Owns> Owns { get; private set; }
        public virtual ICollection<OwnershipAnswer> OwnershipAnswers { get; private set; }
        public virtual ICollection<OwnershipRequest> OwnershipRequests { get; private set; }
        public virtual ICollection<OwnershipRequest> OwnershipRequestsFrom { get; private set; }
        public virtual ICollection<Manages> Manage { get; private set; }
        public virtual ICollection<AuthorityHandler> Appointements { get; set; }
        public virtual IList<Review> Reviews { get; private set; }
        public virtual IList<Message> Messages { get; private set; }
        public virtual string Username { get; private set; }
        public virtual byte[] Password { get; private set; }   //it will be SHA256 encrypted password
        public virtual ICollection<Purchase> Purchases { get; set; }
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public LoggedInUser() : base()
        {
            Username = "";
            Password = new byte[] { 0 };
            Owns = new List<Owns>();
            Manage = new List<Manages>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
            Appointements = new List<AuthorityHandler>();
            Cart = new Cart(this);
            OwnershipAnswers = new List<OwnershipAnswer>();
            OwnershipRequests = new List<OwnershipRequest>();
            OwnershipRequestsFrom = new List<OwnershipRequest>();
        }

        public LoggedInUser(string username, byte[] password) : base()
        {
            Username = username;
            Password = password;
            /*            OwnershipRequests = (IList<OwnershipRequest>)dbContext.OwnershipRequests.Select(req => req.Owner != null && req.Owner.Equals(this));
                        Owns = (IList<Owns>)dbContext.AuthorityHandlers.Select(handler => handler is Owns &&
                                ((Owns)handler).LoggedInUser != null && ((Owns)handler).LoggedInUser.Equals(this));
                        Manage = (IList<Manages>)dbContext.AuthorityHandlers.Select(handler => handler is Manages &&
                                ((Manages)handler).LoggedInUser != null && ((Manages)handler).LoggedInUser.Equals(this));
                        Reviews = (IList<Review>)dbContext.Reviews.Select(review => review.Writer != null && review.Writer.Equals(this));
                        Messages = (IList<Message>)dbContext.Messages.Select(message => message.WrittenBy != null && message.WrittenBy.Equals(this));
                        Purchases = (IList<Purchase>)dbContext.Purchases.Select(purhcase => purhcase.User != null && purhcase.User.Equals(this));
                        Cart = dbContext.Carts.FirstOrDefault(cart => cart.LoggedInUser != null && cart.LoggedInUser.Equals(this));
                        if(Cart == default)
            */
            //Owns = DatabaseProxy.Instance.AuthorityHandlers.OfType<Owns>().Where(owns => owns.Username.Equals(username)).ToList();
            Owns = new List<Owns>();
            Manage = new List<Manages>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
            Purchases = new List<Purchase>();
            Appointements = new List<AuthorityHandler>();
            Cart = new Cart(this);
            OwnershipAnswers = new List<OwnershipAnswer>();
            OwnershipRequests = new List<OwnershipRequest>();
            OwnershipRequestsFrom = new List<OwnershipRequest>();
        }

        public int AmountOfUnReadMessage
        {
            get
            {
                int counter = 0;
                foreach (var msg in Messages)
                {
                    //If some message in a message line is unseen, it counts only as one
                    Message? run = msg;
                    while (run != null)
                    {
                        if (run.ClientSawIt != true)
                        {
                            counter++;
                            break;
                        }
                        run = run.Next;
                    }
                }
                return counter;
            }
        }

        public void WriteReview(Product product, string description)
        {
            if (description.Length == 0)
            {
                throw new ReviewIsEmptyException();
            }
            Review review = new Review(this, description, product);
            product.Reviews.Add(review);
            Reviews.Add(review);
            DatabaseProxy.Instance.Reviews.Add(review);
            DatabaseProxy.Instance.SaveChanges();
        }
       
        public void WriteMessage(Store store, string description, bool isClient)
        {
            if (description.Length == 0)
            {
                throw new MessageIsEmptyException();
            }
            Message message = new Message(this, store, description, isClient);
            store.Messages.Add(message);
            Messages.Add(message);
            DatabaseProxy.Instance.Messages.Add(message);
            DatabaseProxy.Instance.SaveChanges();
        }

        public void AnswerOwnershipRequest(Store store,LoggedInUser newOwner, RequestState answer)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            if (Manage.Select(mng => mng.LoggedInUser == newOwner).Any())
            {
                throw new UserIsAlreadyStoreManagerException();
            }
            ownership.AnswerOwnershipRequest(newOwner, answer);
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
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.AddStoreOwner(newOwner);
                return;
            }
            management.AddStoreOwner(newOwner);
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

        public void RemoveStoreOwner(Store store, LoggedInUser ownerToRemove)
        {

            var ownership = Owns.FirstOrDefault(man => man.Store == store);
            var management = Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));
            if (management == default)
            {
                ownership.RemoveStoreOwner(ownerToRemove);
                return;
            }
            management.RemoveStoreOwner(ownerToRemove);
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

        public Message MessageReplyAsNotManager(Message message, string description)
        {
            Message reply = new Message(this, message.ToStore, description, true, message);
            message.Next = reply;
            DatabaseProxy.Instance.Messages.Add(reply);
            DatabaseProxy.Instance.SaveChanges();
            log.Info("Reply has been published successfully");
            return reply;
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

        override public Purchase Purchase(Basket basket, string creditCardNumber, Address address, UserFacade facade)
        {
            if (basket.Products.Count == 0)
                throw new BasketIsEmptyException();
            Purchase purchase;
            purchase = new Purchase(this, basket, address);
            
            foreach (var product in basket.Products)
            {
                if (product.Quantity <= 0)
                    throw new NegativeQuantityException();
            }
            basket.Store.PurchaseBasket(basket, creditCardNumber, address, this);
            Cart.Baskets.Remove(basket);
            basket.Store.Purchases.Add(purchase);
            Purchases.Add(purchase);
            facade.AddPurchaseToList(purchase);
            DatabaseProxy.Instance.Purchases.Add(purchase);
            DatabaseProxy.Instance.SaveChanges();
            return purchase;
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

        public void AddSystemDayPolicy(Store store, Operator op, Weekday cantBuyIn)
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

        public void AddProductCategoryDiscount(Store store, string categoryName, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft)
        {
            OwnsForStore(store).AddProductCategoryDiscount(op, categoryName, deadline, percentage, indexInChain, disId, toLeft);
        }

        public void AddSpecificProductDiscount(Store store, Product product, DateTime deadline, double percentage, Operator op, int IndexInChain, int disId, bool toLeft)
        {
            OwnsForStore(store).AddSpecificProductDiscount(op, product, deadline, percentage, IndexInChain, disId, toLeft);
        }
        public void AddBuyOverDiscountDiscount(Store store, Product product, DateTime deadline, double percentage, double minSum, Operator op, int IndexInChain, int disId, bool toLeft)
        {
            OwnsForStore(store).AddBuyOverDiscount(op, product, deadline, percentage, minSum, IndexInChain, disId, toLeft);
        }
        public void AddBuySomeGetSomeFreeDiscount(Store store, Product prod1, Product prod2, DateTime deadline, double percentage, int buySome, int getSome, Operator op, int IndexInChain, int disId, bool toLeft)
        {
            OwnsForStore(store).AddBuySomeGetSomeDiscount(op, prod1, prod2, deadline, percentage, buySome, getSome, IndexInChain, disId, toLeft);
        }

        public void RemoveDiscount(Store store, int indexInChain)
        {
            OwnsForStore(store).RemoveDiscount(indexInChain);
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }
    }
}