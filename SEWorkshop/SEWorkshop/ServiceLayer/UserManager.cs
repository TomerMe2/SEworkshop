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

        public void AddProductToCart(Product product, int quantity)
        {
            UserFacadeInstance.AddProductToCart(currUser, product, quantity);
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

        public void OpenStore(LoggedInUser owner, string storeName)
        {
            StoreFacadeInstance.CreateStore(owner, storeName);
        }

        public void Purchase(Basket basket)
        {
            UserFacadeInstance.Purchase(currUser, basket);
        }

        public void Register(string username, string password)
        {
            UserFacadeInstance.Register(username, securityAdapter.Encrypt(password));
        }

        public void RemoveProductFromCart(Product product, int quantity)
        {
            UserFacadeInstance.RemoveProductFromCart(currUser, product, quantity);
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

        public void WriteReview(Product product, string description)
        {
            UserFacadeInstance.WriteReview(currUser, product, description);
        }

        public void WriteMessage(Store store, string description)
        {
            UserFacadeInstance.WriteMessage(currUser, store, description);
        }

        public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser user)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return UserFacadeInstance.UserPurchaseHistory((LoggedInUser)currUser, user);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> StorePurchaseHistory(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return UserFacadeInstance.StorePurchaseHistory((LoggedInUser)currUser, store);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Purchase> ManagingPurchaseHistory(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                return ManageFacadeInstance.ViewPurchaseHistory((LoggedInUser)currUser, store);
            }
            throw new UserHasNoPermissionException();
        }

        public void AddProduct(Store store, string name, string description, string category, double price, int quantity)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.AddProduct((LoggedInUser)currUser, store, name, description, category, price, quantity);
                //replacing spaces with _, so different words will be related to one product name in the typos fixer algorithm
                TyposFixerNames.AddToDictionary(name);
                TyposFixerCategories.AddToDictionary(category);
                // for keywods, we are treating each word in an un-connected way, because each word is a keyword
                foreach(string word in name.Split(' '))
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
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveProduct(Store store, string name)
        {
            if(UserFacadeInstance.HasPermission)
            {
                Product product = store.GetProduct(name);
                ManageFacadeInstance.RemoveProduct((LoggedInUser)currUser, store, product);
                // we don't need to remove the product's description cus there are lots of produts with possibly similar descriptions
                // same applies for category
                TyposFixerNames.RemoveFromDictionary(product.Name);
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductDescription(Product product, string Description){
             if(UserFacadeInstance.HasPermission)
             {
                ManageFacade.GetInstance().EditProductDescription(product, Description);
             }
             throw new UserHasNoPermissionException();
        }

        public void EditProductPrice(Product product, double price){
            if(UserFacadeInstance.HasPermission)
            {            
                ManageFacade.GetInstance().EditProductPrice(product, price);        
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductCategory(Product product, string category){
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacade.GetInstance().EditProductCategory(product, category);
            }
            throw new UserHasNoPermissionException();
        }

        public void EditProductName(Product product, string name){
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacade.GetInstance().EditProductName(product, name);
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreOwner(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newOwner = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreOwner((LoggedInUser)currUser, store, newOwner);
            }
            throw new UserHasNoPermissionException();
        }

        public void AddStoreManager(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser newManager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.AddStoreManager((LoggedInUser)currUser, store, newManager);
            }
            throw new UserHasNoPermissionException();
        }

        public void SetPermissionsOfManager(Store store, string username, string auth)
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
                ManageFacadeInstance.SetPermissionsOfManager((LoggedInUser)currUser, store, newManager, authorization);
            }
            throw new UserHasNoPermissionException();
        }

        public void RemoveStoreManager(Store store, string username)
        {
            if(UserFacadeInstance.HasPermission)
            {
                LoggedInUser manager = UserFacadeInstance.GetUser(username);
                ManageFacadeInstance.RemoveStoreManager((LoggedInUser)currUser, store, manager);
            }
            throw new UserHasNoPermissionException();
        }

        public IEnumerable<Message> ViewMessage(Store store)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.ViewMessage((LoggedInUser)currUser, store);
            }
            throw new UserHasNoPermissionException();
        }

        public void MessageReply(Message message, Store store, string description)
        {
            if(UserFacadeInstance.HasPermission)
            {
                ManageFacadeInstance.MessageReply((LoggedInUser)currUser, message, store, description);
            }
            throw new UserHasNoPermissionException();
        }
    }
}
