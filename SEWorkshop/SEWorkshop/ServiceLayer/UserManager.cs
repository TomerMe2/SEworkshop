using System;
using System.Collections.Generic;
using SEWorkshop.Facades;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using NLog;
using System.Linq;
using SEWorkshop.Adapters;
using SEWorkshop.TyposFix;

namespace SEWorkshop.ServiceLayer
{
    public class UserManager : IUserManager
    {
        User currUser = new GuestUser();
        readonly StoreFacade StoreFacadeInstance = StoreFacade.GetInstance();
        ManageFacade ManageFacadeInstance = ManageFacade.GetInstance();
        UserFacade UserFacadeInstance = UserFacade.GetInstance();
        private readonly ISecurityAdapter securityAdapter = new SecurityAdapter();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private ITyposFixerProxy TyposFixerNames { get; set; }
        private ITyposFixerProxy TyposFixerCategories { get; set; }
        private ITyposFixerProxy TyposFixerKeywords { get; set; }

        public UserManager()
        {
            TyposFixerNames = new TyposFixer(new List<string>());
            TyposFixerCategories = new TyposFixer(new List<string>());
            TyposFixerKeywords = new TyposFixer(new List<string>());
        }

        private Product GetProduct(string storeName, string productName)
        {
            var product = StoreFacadeInstance.SearchProducts(prod => prod.Store.Name.Equals(storeName) && prod.Name.Equals(productName))
                .FirstOrDefault();
            if (product is null)
            {
                Log.Info(string.Format("Someone searched for a non existing product with name {0} and store name {1}",
                    productName, storeName));
                throw new ProductNotInTradingSystemException();
            }
            return product;
        }

        private Store GetStore(string storeName)
        {
            var store = StoreFacadeInstance.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone searched for a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            return store;
        }

        public void AddProductToCart(string storeName, string productName, int quantity)
        {
            UserFacadeInstance.AddProductToCart(currUser, GetProduct(storeName, productName), quantity);
        }

        public IEnumerable<Store> BrowseStores()
        {
            return StoreFacadeInstance.BrowseStores();
        }

        public IEnumerable<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred)
        {
            return StoreFacadeInstance.FilterProducts(products, pred);
        }

        public void Login(string username, string password)
        {
            //preserve loggedIn user's cart that he gathered as a GuestUser.
            Cart cart = currUser.Cart;
            currUser = UserFacadeInstance.Login(username, securityAdapter.Encrypt(password));
            currUser.Cart = cart;
        }

        public void Logout()
        {
            Cart cart = currUser.Cart;
            UserFacadeInstance.Logout();
            currUser = new GuestUser();
            currUser.Cart = cart;
        }

        public IEnumerable<Basket> MyCart()
        {
            return UserFacadeInstance.MyCart(currUser);
        }

        public void OpenStore(string storeName)
        {
            if(currUser is GuestUser)
            {
                throw new UserHasNoPermissionException();
            }
            StoreFacadeInstance.CreateStore((LoggedInUser)currUser, storeName);
        }

        public void Purchase(Basket basket)
        {
            UserFacadeInstance.Purchase(currUser, basket);
        }

        public void Register(string username, string password)
        {
            UserFacadeInstance.Register(username, securityAdapter.Encrypt(password));
        }

        public void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            UserFacadeInstance.RemoveProductFromCart(currUser, GetProduct(storeName, productName), quantity);
        }

        private IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            return StoreFacadeInstance.SearchProducts(pred);
        }

        public IEnumerable<Product> SearchProductsByName(ref string input)
        {
            string localInput = input;
            IEnumerable<Product> products = SearchProducts(product => product.Name.Equals(localInput));
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(input);
            products = SearchProducts(product => product.Name.ToLower().Replace(' ', '_').Equals(corrected));
            input = corrected.Replace('_', ' ');   // the typo fixer returns '_' instead of ' ', so it will fix it
            return products;
        }

        public IEnumerable<Product> SearchProductsByCategory(ref string input)
        {
            string localInput = input;
            IEnumerable<Product> products = SearchProducts(product => product.Category.Equals(localInput));
            if (products.Any())
                return products;
            string corrected = TyposFixerNames.Correct(input);
            products = SearchProducts(product => product.Category.ToLower().Replace(' ', '_').Equals(corrected));
            input = corrected.Replace('_', ' ');   // the typo fixer returns '_' instead of ' ', so it will fix it
            return products;
        }

        public IEnumerable<Product> SearchProductsByKeywords(ref string input)
        { 
            string localInput = input;
            bool hasWordInsideOther(string[] words1, List<string> words2)
            {
                foreach (string word1 in words1)
                {
                    foreach (string word2 in words2)
                    {
                        if (word1.Equals(word2.ToLower()))
                        {
                            return true;
                        }
                    }
                }
                return false;
            };
            bool hasWordInsideInput(string[] words) => hasWordInsideOther(words, localInput.Split(' ').ToList());   // curry version
            bool predicate(Product product) => hasWordInsideInput(product.Name.Split(' ')) ||
                                            hasWordInsideInput(product.Category.Split(' ')) ||
                                            hasWordInsideInput(product.Description.Split(' '));
            IEnumerable <Product> products = SearchProducts(predicate);
            if (products.Any())
                return products;
            // Each word should be corrected seperatly because the words do not have to depend on each other
            List<string> corrected = input.Split(' ').Select(word => TyposFixerKeywords.Correct(word)).ToList();
            bool hasWordInsideCorrected(string[] words) => hasWordInsideOther(words, corrected);  // curry version
            bool correctedPredicate(Product product) => hasWordInsideCorrected(product.Name.Split(' ')) ||
                                            hasWordInsideCorrected(product.Category.Split(' ')) ||
                                            hasWordInsideCorrected(product.Description.Split(' '));
            products = SearchProducts(correctedPredicate);
            input = String.Join(' ', corrected);
            return products;
        }

        public IEnumerable<Purchase> PurcahseHistory()
        {
            return UserFacadeInstance.PurcahseHistory(currUser);
        }

        public void WriteReview(string storeName, string productName, string description)
        {
            if(description.Equals(string.Empty))
            {
                Log.Info(string.Format("Someone tried to write empty review on product named {0}", productName));
                throw new ArgumentException("description is empty");
            }
            UserFacadeInstance.WriteReview(currUser, GetProduct(storeName, productName), description);
        }

        public void WriteMessage(string storeName, string description)
        {
            if (description.Equals(string.Empty))
            {
                Log.Info(string.Format("Someone tried to write empty message to store named {0}", storeName));
                throw new ArgumentException("description is empty");
            }
            UserFacadeInstance.WriteMessage(currUser, GetStore(storeName), description);
        }

        public IEnumerable<Purchase> UserPurchaseHistory(string userNm)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return UserFacadeInstance.UserPurchaseHistory((LoggedInUser)currUser, userNm);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> StorePurchaseHistory(string storeName)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return UserFacadeInstance.StorePurchaseHistory((LoggedInUser)currUser, GetStore(storeName));
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> ManagingPurchaseHistory(string storeName)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return ManageFacadeInstance.ViewPurchaseHistory((LoggedInUser)currUser, GetStore(storeName));
            }
            throw new UserHasNoPermissionException();
        }

        public void AddProduct(string storeName, string productName, string description, string category, double price, int quantity)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.AddProduct((LoggedInUser)currUser, GetStore(storeName), productName, description, category, price, quantity);
                //replacing spaces with _, so different words will be related to one product name in the typos fixer algorithm
                TyposFixerNames.AddToDictionary(productName);
                TyposFixerCategories.AddToDictionary(category);
                // for keywods, we are treating each word in an un-connected way, because each word is a keyword
                foreach(string word in productName.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
                foreach (string word in category.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
                foreach (string word in description.Split(' '))
                {
                    TyposFixerKeywords.AddToDictionary(word);
                }
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(string storeName, string productName)
        {
            if(UserFacadeInstance.HasPermission)
            {
                Store store = GetStore(storeName);
                Product product = GetProduct(storeName, productName);
                ManageFacadeInstance.RemoveProduct((LoggedInUser)currUser, store, product);
                // we don't need to remove the product's description cus there are lots of produts with possibly similar descriptions
                // same applies for category
                TyposFixerNames.RemoveFromDictionary(product.Name);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductDescription(string storeName, string productName, string Description)
        {
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            if (UserFacadeInstance.HasPermission)
             {
                ManageFacade.GetInstance().EditProductDescription((LoggedInUser)currUser, store, product, Description);
                return;
             }
             throw new UserHasNoPermissionException();
        }

        public void EditProductPrice(string storeName, string productName, double price)
        {
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            if (UserFacadeInstance.HasPermission)
            {            
                ManageFacade.GetInstance().EditProductPrice((LoggedInUser)currUser, store, product, price);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductCategory(string storeName, string productName, string category)
        {
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            if (UserFacadeInstance.HasPermission)
            {
                ManageFacade.GetInstance().EditProductCategory((LoggedInUser)currUser, store, product, category);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductName(string storeName, string productName, string name)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacade.GetInstance().EditProductName((LoggedInUser)currUser, GetStore(storeName),
                    GetProduct(storeName, productName), name);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductQuantity(string storeName, string productName, int quantity)
        {
            if (UserFacadeInstance.HasPermission)
            {
                ManageFacade.GetInstance().EditProductQuantity((LoggedInUser)currUser, GetStore(storeName),
                    GetProduct(storeName, productName), quantity);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(string storeName, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newOwner = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreOwner((LoggedInUser)currUser, GetStore(storeName), newOwner);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreManager(string storeName, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newManager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreManager((LoggedInUser)currUser, GetStore(storeName), newManager);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void SetPermissionsOfManager(string storeName, string username, string auth)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newManager = UserFacadeInstance.GetUser(username);
                Authorizations authorization;
                switch (auth)
                {
                    case "Products":
                        authorization = Authorizations.Products;
                        break;

                    case "Owner":
                        authorization = Authorizations.Owner;
                        break;

                    case "Manager":
                        authorization = Authorizations.Manager;
                        break;

                    case "Authorizing":
                        authorization = Authorizations.Authorizing;
                        break;

                    case "Replying":
                        authorization = Authorizations.Replying;
                        break;

                    case "Watching":
                        authorization = Authorizations.Watching;
                        break;
                    
                    default:
                        throw new AuthorizationDoesNotExistException();
                }
                ManageFacadeInstance.SetPermissionsOfManager((LoggedInUser)currUser, GetStore(storeName), newManager, authorization);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveStoreManager(string storeName, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser manager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.RemoveStoreManager((LoggedInUser)currUser, GetStore(storeName), manager);
                return;
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Message> ViewMessage(string storeName)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return ManageFacadeInstance.ViewMessage((LoggedInUser)currUser, GetStore(storeName));
            }
            throw new UserHasNoPermissionException();
        }

        public void MessageReply(Message message, string storeName, string description)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.MessageReply((LoggedInUser)currUser, message, GetStore(storeName), description);
                return;
            }
            throw new UserHasNoPermissionException();
        }
    }
}
