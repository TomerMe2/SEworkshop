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

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public FacadesBridge()
        {
            ManageFacade = new ManageFacade();
            StoreFacade = new StoreFacade();
            UserFacade = new UserFacade(StoreFacade);
        }

        public DataProduct AddProduct(DataLoggedInUser user, string storeName, string productName, string description, string category, double price, int quantity)
        {
            Log.Info(string.Format("AddProduct was invoked with storeName {0}, productName {1}, description {2}," +
                " category {3}, price {4}, quantity{5}", storeName, productName, description, category, price, quantity));
            Product product = ManageFacade.AddProduct(GetLoggedInUsr(user), GetStore(storeName),
                                                          productName, description, category, price, quantity);
            return new DataProduct(product);
        }

        public DataStore SearchStore(string storeName)
        {
            return new DataStore(GetStore(storeName));
        }

        private LoggedInUser GetLoggedInUsr(DataLoggedInUser user)
        {
            return UserFacade.GetLoggedInUser(user.Username);
        }

        private LoggedInUser GetLoggedInUsr(string userName)
        {
            return UserFacade.GetLoggedInUser(userName);
        }

        private User GetUser(DataUser dataUser)
        {
            return dataUser switch
            {
                DataGuestUser guest => UserFacade.GetGuestUser(guest.Id),
                DataLoggedInUser loggedIn => UserFacade.GetLoggedInUser(loggedIn.Username),
                _ => throw new Exception("Not implemented for this user type"),
            };
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

        public void AddProductToCart(DataUser user, string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("AddProductToCart was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            var store = StoreFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone searched for a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            UserFacade.AddProductToCart(GetUser(user), GetProduct(storeName, productName), quantity);
        }

        public void AddStoreManager(DataLoggedInUser user, string storeName, string newManagerUserName)
        {
            Log.Info(string.Format("AddStoreManager was invoked with storeName {0}, username {1}",
               storeName, newManagerUserName));
            ManageFacade.AddStoreManager(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(newManagerUserName));
        }

        public void AddStoreOwner(DataLoggedInUser user, string storeName, string newOwnerUserName)
        {
            Log.Info(string.Format("AddStoreOwner was invoked with storeName {0}, username {1}",
               storeName, newOwnerUserName));
            ManageFacade.AddStoreOwner(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(newOwnerUserName));
        }

        public IEnumerable<DataStore> BrowseStores()
        {
            return StoreFacade.BrowseStores().Select(store => new DataStore(store));
        }

        public void EditProductCategory(DataLoggedInUser user, string storeName, string productName, string category)
        {
            ManageFacade.EditProductCategory(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName, productName), category);
        }

        public void EditProductDescription(DataLoggedInUser user, string storeName, string productName, string description)
        {
            ManageFacade.EditProductDescription(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName, productName), description);
        }

        public void RemovePermissionsOfManager(DataLoggedInUser user, string storeName, string username, Authorizations authorization)
        {
            ManageFacade.RemovePermissionsOfManager(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(username), authorization);
        }

        public void AddProductCategoryDiscount(DataLoggedInUser user, string storeName, string categoryName)
        {
            GetLoggedInUsr(user).AddProductCategoryDiscount(GetStore(storeName), categoryName);
        }

        public void AddSpecificProductDiscount(DataLoggedInUser user, string storeName, string productName)
        {
            GetLoggedInUsr(user).AddSpecificProductDiscount(GetStore(storeName), GetProduct(storeName, productName));
        }

        public void RemoveDiscount(DataLoggedInUser user, string storeName, int indexInChain)
        {
            GetLoggedInUsr(user).RemoveDiscount(GetStore(storeName), indexInChain);
        }

        public void EditProductName(DataLoggedInUser user, string storeName, string productName, string name)
        {
            ManageFacade.EditProductName(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName, productName), name);
        }

        public void EditProductPrice(DataLoggedInUser user, string storeName, string productName, double price)
        {
            ManageFacade.EditProductPrice(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName, productName), price);
        }

        public void EditProductQuantity(DataLoggedInUser user, string storeName, string productName, int quantity)
        {
            ManageFacade.EditProductQuantity(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName, productName), quantity);
        }

        public IEnumerable<DataProduct> FilterProducts(ICollection<DataProduct> products, Func<DataProduct, bool> pred)
        {
            return products.Where(pred);
        }

        public DataLoggedInUser GetLoggedInUserAndApplyCart(string username, byte[] password, DataGuestUser userAsGuest)
        {
            Log.Info(string.Format("Login was invoked with username {0}", username));
            //preserve loggedIn user's cart that he gathered as a GuestUser.
            Cart cart = GetUser(userAsGuest).Cart;
            LoggedInUser loggedIn = UserFacade.GetLoggedInUser(username, password);
            loggedIn.Cart = cart;
            return new DataLoggedInUser(loggedIn);
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(DataLoggedInUser user, string storeName)
        {
            Log.Info(string.Format("ManagingPurchaseHistory was invoked with storeName {0}", storeName));
            return ManageFacade.ViewPurchaseHistory(GetLoggedInUsr(user), GetStore(storeName)).Select(prchs => new DataPurchase(prchs));
        }

        public DataMessage MessageReply(DataLoggedInUser user, DataMessage message, string storeName, string description)
        {
            Log.Info(string.Format("MessageReply was invoked with storeName {0}", storeName));
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
            return new DataMessage(ManageFacade.MessageReply(GetLoggedInUsr(user), toAnswerOn, GetStore(storeName), description));
        }

        public IEnumerable<DataBasket> MyCart(DataUser user)
        {
            return GetUser(user).Cart.Baskets.Select(bskt => new DataBasket(bskt));
        }

        public void OpenStore(DataLoggedInUser user, string storeName)
        {
            Log.Info(string.Format("OpenStore was invoked with storeName {0}", storeName));
            StoreFacade.CreateStore(GetLoggedInUsr(user), storeName);
        }

        public IEnumerable<DataPurchase> PurchaseHistory(DataLoggedInUser user)
        {
            return UserFacade.PurchaseHistory(GetLoggedInUsr(user)).Select(prchs => new DataPurchase(prchs));
        }

        public void Purchase(DataUser dataUser, DataBasket basket, string creditCardNum, Address address)
        {
            var user = GetUser(dataUser);
            Basket? trueBasket = user.Cart.Baskets.FirstOrDefault(bskt => basket.Represents(bskt));
            if (trueBasket is null)
            {
                throw new BasketNotInSystemException();
            }
            UserFacade.Purchase(user, trueBasket, creditCardNum, address);
        }

        public void Register(string username, byte[] password)
        {
            UserFacade.Register(username, password);
        }

        public void RemoveProduct(DataLoggedInUser user, string storeName, string productName)
        {
            Log.Info(string.Format("RemoveProduct was invoked with storeName {0}, productName {1}", storeName, productName));
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            ManageFacade.RemoveProduct(GetLoggedInUsr(user), store, product);
        }

        public void RemoveProductFromCart(DataUser user, string storeName, string productName, int quantity)
        {
            Log.Info(string.Format("RemoveProductFromCart was invoked with storeName {0}, productName {1}, quantity {2}",
                storeName, productName, quantity));
            var store = StoreFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                Log.Info(string.Format("Someone tried to remove product from cart with a non existing store with name {0}", storeName));
                throw new StoreNotInTradingSystemException();
            }
            UserFacade.RemoveProductFromCart(GetUser(user), GetProduct(storeName, productName), quantity);
        }

        public void RemoveStoreManager(DataLoggedInUser user, string storeName, string username)
        {
            Log.Info(string.Format("RemoveStoreManager was invoked with storeName {0}, username {1}",
               storeName, username));
            LoggedInUser manager = UserFacade.GetLoggedInUser(username);
            ManageFacade.RemoveStoreManager(GetLoggedInUsr(user), GetStore(storeName), manager);
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

        public void SetPermissionsOfManager(DataLoggedInUser user, string storeName, string username, Authorizations authorization)
        {
            ManageFacade.SetPermissionsOfManager(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(username), authorization);
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(DataLoggedInUser user, string storeName)
        {
            Log.Info(string.Format("StorePurchaseHistory was invoked with storeName {0}", storeName));
            return UserFacade.StorePurchaseHistory(GetLoggedInUsr(user), GetStore(storeName)).Select(prchs => new DataPurchase(prchs));
        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(DataLoggedInUser user, string userNm)
        {
            Log.Info(string.Format("WriteReview was invoked with userName {0}", userNm));
            return UserFacade.UserPurchaseHistory(GetLoggedInUsr(user), userNm).Select(prchs => new DataPurchase(prchs));
        }

        public IEnumerable<DataMessage> ViewMessage(DataLoggedInUser user, string storeName)
        {
            Log.Info(string.Format("ViewMessage was invoked with storeName {0}", storeName));
            return ManageFacade.ViewMessage(GetLoggedInUsr(user), GetStore(storeName)).Select(msg => new DataMessage(msg));
        }

        public void WriteMessage(DataLoggedInUser user, string storeName, string description)
        {
            Log.Info(string.Format("WriteMessage was invoked with storeName {0}, description {1}",
                storeName, description));
            UserFacade.WriteMessage(GetLoggedInUsr(user), GetStore(storeName), description);
        }

        public void WriteReview(DataLoggedInUser user, string storeName, string productName, string description)
        {
            Log.Info(string.Format("WriteReview was invoked with storeName {0}, productName {1}, description {2}",
                storeName, productName, description));
            UserFacade.WriteReview(GetLoggedInUsr(user), GetProduct(storeName, productName), description);
        }

        public DataGuestUser CreateGuest()
        {
            var guestUsr = UserFacade.CreateGuestUser();
            return new DataGuestUser(guestUsr);
        }
    }
}
