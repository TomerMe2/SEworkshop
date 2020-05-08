using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.DataModels;
using SEWorkshop.Models;
using System.Linq;
using SEWorkshop.Exceptions;
using NLog;

namespace SEWorkshop.Facades
{
    public class FacadesBridge : IFacadesBridge
    {
        private IUserFacade UserFacade { get; }
        private IManageFacade ManageFacade { get; }
        private IStoreFacade StoreFacade { get; }
        private User CurrUser { get; set; }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public FacadesBridge()
        {
            ManageFacade = new ManageFacade();
            StoreFacade = new StoreFacade();
            UserFacade = new UserFacade(StoreFacade);
            CurrUser = new GuestUser();
        }
        public DataProduct AddProduct(string storeName, string productName, string description, string category, double price, int quantity)
        {
            Log.Info(string.Format("AddProduct was invoked with storeName {0}, productName {1}, description {2}," +
                " category {3}, price {4}, quantity{5}", storeName, productName, description, category, price, quantity));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            Product product = ManageFacade.AddProduct((LoggedInUser)CurrUser, GetStore(storeName),
                                                          productName, description, category, price, quantity);

            return new DataProduct(product);
        }

        public LoggedInUser GetUser(string userName)
        {
            return UserFacade.GetUser(userName);
        }

        private Store GetStore(string storeName)
        {
            Store? store = StoreFacade.SearchStore(store => store.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                throw new StoreNotInTradingSystemException();
            }
            return store;
        }

        private Product GetProduct(string storeName, string productName)
        {
            Store store = GetStore(storeName);
            Product? product = store.Products.FirstOrDefault(prod => prod.Name.Equals(productName));
            if (product is null)
            {
                throw new ProductNotInTheStoreException();
            }
            return product;
        }

        public void AddProductToCart(string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("AddProductToCart was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            var store = StoreFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone searched for a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            UserFacade.AddProductToCart(CurrUser, GetProduct(storeName, productName), quantity);
        }

        public void AddStoreManager(string storeName, string newManagerUserName)
        {
            Log.Info(string.Format("AddStoreManager was invoked with storeName {0}, username {1}",
               storeName, newManagerUserName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            else
            {
                ManageFacade.AddStoreManager((LoggedInUser)CurrUser, GetStore(storeName), GetUser(newManagerUserName));
            }
        }

        public void AddStoreOwner(string storeName, string newOwnerUserName)
        {
            Log.Info(string.Format("AddStoreOwner was invoked with storeName {0}, username {1}",
               storeName, newOwnerUserName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            ManageFacade.AddStoreOwner((LoggedInUser)CurrUser, GetStore(storeName), GetUser(newOwnerUserName));
        }

        public IEnumerable<DataStore> BrowseStores()
        {
            return StoreFacade.BrowseStores().Select(store => new DataStore(store));
        }

        public void EditProductCategory(string storeName, string productName, string category)
        {
            if (!UserFacade.HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ManageFacade.EditProductCategory((LoggedInUser)CurrUser, GetStore(storeName), GetProduct(storeName, productName), category);
        }

        public void EditProductDescription(string storeName, string productName, string description)
        {
            if (!UserFacade.HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ManageFacade.EditProductDescription((LoggedInUser)CurrUser, GetStore(storeName), GetProduct(storeName, productName), description);
        }

        public void EditProductName(string storeName, string productName, string name)
        {
            if (!UserFacade.HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ManageFacade.EditProductName((LoggedInUser)CurrUser, GetStore(storeName), GetProduct(storeName, productName), name);
        }

        public void EditProductPrice(string storeName, string productName, double price)
        {
            if (!UserFacade.HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ManageFacade.EditProductPrice((LoggedInUser)CurrUser, GetStore(storeName), GetProduct(storeName, productName), price);
        }

        public void EditProductQuantity(string storeName, string productName, int quantity)
        {
            if (!UserFacade.HasPermission)
            {
                throw new UserHasNoPermissionException();
            }
            ManageFacade.EditProductQuantity((LoggedInUser)CurrUser, GetStore(storeName), GetProduct(storeName, productName), quantity);
        }

        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            return products.Where(pred);
        }

        public void Login(string username, byte[] password)
        {
            Log.Info(string.Format("Login was invoked with username {0}", username));
            //preserve loggedIn user's cart that he gathered as a GuestUser.
            Cart cart = CurrUser.Cart;
            CurrUser = UserFacade.Login(username, password);
            CurrUser.Cart = cart;
        }

        public void Logout()
        {
            Log.Info("Logout was invoked");
            Cart cart = CurrUser.Cart;
            UserFacade.Logout();
            CurrUser = new GuestUser();
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(string storeName)
        {
            Log.Info(string.Format("ManagingPurchaseHistory was invoked with storeName {0}", storeName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            return ManageFacade.ViewPurchaseHistory((LoggedInUser)CurrUser, GetStore(storeName)).Select(prchs => new DataPurchase(prchs));
        }

        public DataMessage MessageReply(DataMessage message, string storeName, string description)
        {
            Log.Info(string.Format("MessageReply was invoked with storeName {0}", storeName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            var store = GetStore(storeName);
            DataMessage firstMsgData = message;
            // this whole manouver is because one can answer on a message that is not in the messages list
            // for example: answer to an answer.
            while (firstMsgData.Prev != null)
            {
                firstMsgData = firstMsgData.Prev;
            }
            Message? firstMsg = store.Messages.FirstOrDefault(msg => firstMsgData.Represents(msg));
            if (firstMsg is null)
            {
                Log.Info("message is not in the system");
                throw new MessageNotInSystemException();
            }
            Message toAnswerOn = firstMsg;
            while (!message.Represents(toAnswerOn) && toAnswerOn.Next != null)
            {
                toAnswerOn = toAnswerOn.Next;
            }
            if (!message.Represents(toAnswerOn))
            {
                Log.Info("message is not in the system");
                throw new MessageNotInSystemException();
            }
            return new DataMessage(ManageFacade.MessageReply((LoggedInUser)CurrUser, toAnswerOn, GetStore(storeName), description));
        }

        public IEnumerable<DataBasket> MyCart()
        {
            return CurrUser.Cart.Baskets.Select(bskt => new DataBasket(bskt));
        }

        public void OpenStore(string storeName)
        {
            Log.Info(string.Format("OpenStore was invoked with storeName {0}", storeName));
            if (CurrUser is GuestUser)
            {
                Log.Info(string.Format("GuestUser invoked OpenStore"));
                throw new UserHasNoPermissionException();
            }
            StoreFacade.CreateStore((LoggedInUser)CurrUser, storeName);
        }

        public IEnumerable<DataPurchase> PurchaseHistory()
        {
            return UserFacade.PurchaseHistory(CurrUser).Select(prchs => new DataPurchase(prchs));
        }

        public void Purchase(DataBasket basket, string creditCardNum, Address address)
        {
            Basket? trueBasket = CurrUser.Cart.Baskets.FirstOrDefault(bskt => basket.Represents(bskt));
            if (trueBasket is null)
            {
                throw new BasketNotInSystemException();
            }
            UserFacade.Purchase(CurrUser, trueBasket, creditCardNum, address);
        }

        public void Register(string username, byte[] password)
        {
            UserFacade.Register(username, password);
        }

        public void RemoveProduct(string storeName, string productName)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}", storeName, productName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            ManageFacade.RemoveProduct((LoggedInUser)CurrUser, store, product);
        }

        public void RemoveProductFromCart(string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("RemoveProductFromCart was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            var store = StoreFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone tried to remove product from cart with a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            UserFacade.RemoveProductFromCart(CurrUser, GetProduct(storeName, productName), quantity);
        }

        public void RemoveStoreManager(string storeName, string username)
        {
            Log.Info(string.Format("RemoveStoreManager was invoked with storeName {0}, username {1}",
               storeName, username));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            LoggedInUser manager = UserFacade.GetUser(username);
            ManageFacade.RemoveStoreManager((LoggedInUser)CurrUser, GetStore(storeName), manager);
        }

        private IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            Log.Info("SearchProducts was invoked");
            return StoreFacade.SearchProducts(pred);
        }

        public IEnumerable<DataProduct> SearchProductsByCategory(string input)
        {
            Log.Info("SearchProductsByCategory was invoked with input {0}", input);
            string localInput = input;
            return SearchProducts(product => product.Category.ToLower().Replace('_', ' ').Equals(localInput)).Select(prod => new DataProduct(prod)); 
        }

        public IEnumerable<DataProduct> SearchProductsByKeywords(string input)
        {
            Log.Info("SearchProductsByKeywords was invoked with input {0}", input);
            bool HasWordInsideOther(string[] words1, List<string> words2)
            {
                foreach (string word1 in words1)
                {
                    foreach (string word2 in words2)
                    {
                        if (word1.ToLower().Equals(word2.ToLower()))
                        {
                            return true;
                        }
                    }
                }
                return false;
            };
            bool hasWordInsideInput(string[] words) => HasWordInsideOther(words, input.Split(' ').ToList());   // curry version
            bool predicate(Product product) => hasWordInsideInput(product.Name.Split(' ')) ||
                                            hasWordInsideInput(product.Category.Split(' ')) ||
                                            hasWordInsideInput(product.Description.Split(' '));
            return SearchProducts(predicate).Select(product => new DataProduct(product));
        }

        public IEnumerable<DataProduct> SearchProductsByName(string input)
        {
            Log.Info("SearchProductsByName was invoked with input {0}", input);
            string localInput = input;
            return SearchProducts(product => product.Name.ToLower().Replace('_', ' ').Equals(localInput)).Select(prod => new DataProduct(prod));
        }

        public void SetPermissionsOfManager(string storeName, string username, Authorizations authorization)
        {
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            ManageFacade.SetPermissionsOfManager((LoggedInUser)CurrUser, GetStore(storeName), GetUser(username), authorization);
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(string storeName)
        {
            Log.Info(string.Format("StorePurchaseHistory was invoked with storeName {0}", storeName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            return UserFacade.StorePurchaseHistory((LoggedInUser)CurrUser, GetStore(storeName)).Select(prchs => new DataPurchase(prchs));

        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(string userNm)
        {
            Log.Info(string.Format("WriteReview was invoked with userName {0}", userNm));
            if (!UserFacade.HasPermission)
            {
                Log.Info(string.Format("userName {0} has no permission", userNm));
                throw new UserHasNoPermissionException();
            }
            return UserFacade.UserPurchaseHistory((LoggedInUser)CurrUser, userNm).Select(prchs => new DataPurchase(prchs));


        }

        public IEnumerable<DataMessage> ViewMessage(string storeName)
        {
            Log.Info(string.Format("ViewMessage was invoked with storeName {0}", storeName));
            if (!UserFacade.HasPermission)
            {
                Log.Info("user has no permission");
                throw new UserHasNoPermissionException();
            }
            return ManageFacade.ViewMessage((LoggedInUser)CurrUser, GetStore(storeName)).Select(msg => new DataMessage(msg));
        }

        public void WriteMessage(string storeName, string description)
        {
            Log.Info(string.Format("WriteMessage was invoked with storeName {0}, description {1}",
                storeName, description));
            UserFacade.WriteMessage(CurrUser, GetStore(storeName), description);
        }

        public void WriteReview(string storeName, string productName, string description)
        {
            Log.Info(string.Format("WriteReview was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            UserFacade.WriteReview(CurrUser, GetProduct(storeName, productName), description);
        }
    }
}
