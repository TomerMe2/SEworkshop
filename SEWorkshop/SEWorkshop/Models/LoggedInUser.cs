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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public LoggedInUser() : base()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        public LoggedInUser(string username, byte[] password) : base()
        {
            Username = username;
            Password = password;
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

        [NotMapped()]
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
            //DatabaseProxy.Instance.SaveChanges();
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
            //DatabaseProxy.Instance.SaveChanges();
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
            var ownership = Owns.FirstOrDefault(man =>(man.Store.Name.Equals(store.Name)));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));  
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
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
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
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
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
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
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
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
            if (management == null)
            {
                ownership.SetPermissionsOfManager(manager, authorization);
                return;
            }
            management.SetPermissionsOfManager(manager,authorization);
        }

        public void AddStoreOwner(Store store, LoggedInUser newOwner)
        {
            Owns.FirstOrDefault(own => own.Store.Name.Equals(store.Name))?.AddStoreOwner(newOwner);
        }
        
        public void AddStoreManager(Store store, LoggedInUser newManager)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
            if (management == default)
            {
                ownership.AddStoreManager(newManager);
                return;
            }
            management.AddStoreManager(newManager);
        }
        
        public void RemoveStoreManager(Store store, LoggedInUser managerToRemove)
        {

            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
            if (management == default)
            {
                ownership.RemoveStoreManager(managerToRemove);
                return;
            }
            management.RemoveStoreManager(managerToRemove);
        }

        public void RemoveStoreOwner(Store store, LoggedInUser ownerToRemove)
        {

            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
            if (management == default)
            {
                ownership.RemoveStoreOwner(ownerToRemove);
                return;
            }
            management.RemoveStoreOwner(ownerToRemove);
        }

        public Message MessageReply(Message message, Store store, string description)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
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
            //DatabaseProxy.Instance.SaveChanges();
            log.Info("Reply has been published successfully");
            return reply;
        }

        public IEnumerable<Message> GetMessage(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
            if (management == default)
            {
                return ownership.GetMessage(store, this);
            }
            return management.GetMessage(store, this);
        }

        public IEnumerable<Purchase> PurchaseHistory(Store store)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
            if (management == null)
            {
                return ownership.ViewPurchaseHistory(this, store);
            }
            return management.ViewPurchaseHistory(this, store);
        }

        override public Purchase Purchase(Basket basket, string creditCardNumber, Address address)
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
            DatabaseProxy.Instance.Purchases.Add(purchase);
            //DatabaseProxy.Instance.SaveChanges();
            return purchase;
        }


        public void RemovePermissionsOfManager(Store store, LoggedInUser manager, Authorizations authorization)
        {
            var ownership = Owns.FirstOrDefault(man => man.Store.Equals(store));
            var management = Manage.FirstOrDefault(man => (man.Store.Name.Equals(store.Name)));
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
        public void AddBuyOverDiscount(Store store, Product product, DateTime deadline, double percentage, double minSum, Operator op, int IndexInChain, int disId, bool toLeft)
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

        public override void AddProductToCart(Product product, int quantity)
        {
            if (quantity < 1)
            {
                throw new NegativeQuantityException();
            }
            if (product.Quantity - quantity < 0)
            {
                throw new NegativeInventoryException();
            }
            Cart cart = this.Cart;
            foreach (var basket in cart.Baskets)
            {
                if (product.Store == basket.Store)
                {
                    var prod = basket.Products.FirstOrDefault(tup => tup.Product.Equals(product));
                    if (!(prod is null))
                    {
                        //quantity = quantity + prod.Quantity;
                        // we are doing this because of the fact that when a tuple is assigned, it's copied and int is a primitive...
                        //basket.Products.Remove(prod);  //so we can add it later :)
                        prod.Quantity += quantity;
                        //DatabaseProxy.Instance.SaveChanges();
                        return;
                    }
                    ProductsInBasket newPib = new ProductsInBasket(basket, product, quantity);
                    basket.Products.Add(newPib);
                    DatabaseProxy.Instance.ProductsInBaskets.Add(newPib);
                    //DatabaseProxy.Instance.SaveChanges();
                    return;  // basket found and updated. Nothing more to do here...
                }
            }
            // if we got here, the correct basket doesn't exists now, so we should create it!
            Basket newBasket = new Basket(product.Store, cart);
            Cart.Baskets.Add(newBasket);
            ProductsInBasket pib = new ProductsInBasket(newBasket, product, quantity);
            newBasket.Products.Add(pib);
            DatabaseProxy.Instance.Baskets.Add(newBasket);
            DatabaseProxy.Instance.ProductsInBaskets.Add(pib);
            //DatabaseProxy.Instance.SaveChanges();
        }

        public override void RemoveProductFromCart(User user, Product product, int quantity)
        {
            if (quantity < 1)
            {
                throw new NegativeQuantityException();
            }
            foreach (var basket in user.Cart.Baskets)
            {
                if (product.Store == basket.Store)
                {
                    var prod = basket.Products.FirstOrDefault(tup => tup.Product.Equals(product));
                    if (prod is null)
                    {
                        throw new ProductIsNotInCartException();
                    }
                    int quantityDelta = prod.Quantity - quantity;
                    if (quantityDelta < 0)
                    {
                        throw new ArgumentOutOfRangeException("quantity in cart minus quantity is smaller then 0");
                    }
                    if (quantityDelta > 0)
                    {
                        // The item should still be in the basket because it still has a positive quantity
                        prod.Quantity = quantityDelta;
                        return;
                    }
                    basket.Products.Remove(prod);
                    DatabaseProxy.Instance.ProductsInBaskets.Remove(prod);
                    if(basket.Products.Count() == 0)
                    {
                        Cart.Baskets.Remove(basket);
                        DatabaseProxy.Instance.Baskets.Remove(basket);
                    }
                    return;
                }
            }
            throw new ProductIsNotInCartException();
        }
    }
}