﻿using System;
using System.Collections.Generic;
using SEWorkshop.DataModels;
using SEWorkshop.Models;
using System.Linq;
using SEWorkshop.Exceptions;
using SEWorkshop.Enums;
using SEWorkshop.DAL;

namespace SEWorkshop.Facades
{
    public class FacadesBridge : IFacadesBridge
    {
        private IUserFacade UserFacade { get; }
        private IManageFacade ManageFacade { get; }
        private IStoreFacade StoreFacade { get; }

        public FacadesBridge()
        {
            ManageFacade = new ManageFacade();
            StoreFacade = new StoreFacade();
            UserFacade = new UserFacade(StoreFacade);
        }

        public DataProduct AddProduct(DataLoggedInUser user, string storeName, string productName, string description, string category, double price, int quantity)
        {
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
            Product? product = DatabaseProxy.Instance.Products.FirstOrDefault(prod => prod.StoreName.Equals(storeName) && prod.Name.Equals(productName));
            if (product is null || !product.Store.Name.Equals(storeName))
            {
                throw new ProductNotInTheStoreException();
            }
            return product;
        }

        public void AddProductToCart(DataUser user, string storeName, string productName, int quantity)
        {
            Store store = GetStore(storeName);
            if (store is null)
            {
                throw new StoreNotInTradingSystemException();
            }
            UserFacade.AddProductToCart(GetUser(user), GetProduct(storeName, productName), quantity);
        }

        public void AddStoreManager(DataLoggedInUser user, string storeName, string newManagerUserName)
        {
            ManageFacade.AddStoreManager(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(newManagerUserName));
        }

        public DataOwnershipRequest? AddStoreOwner(DataLoggedInUser user, string storeName, string newOwnerUserName)
        {
            var request = (ManageFacade.AddStoreOwner(GetLoggedInUsr(user),
                GetStore(storeName), GetLoggedInUsr(newOwnerUserName)));
            if (request != null)
            {
                return new DataOwnershipRequest(request);
            }
            return null;
        }

        public void AnswerOwnershipRequest(DataLoggedInUser user, string storeName, string newOwnerUserName, RequestState answer)
        {
            ManageFacade.AnswerOwnershipRequest(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(newOwnerUserName), answer);
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

        public void AddProductCategoryDiscount(DataLoggedInUser user, string storeName, string categoryName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            ManageFacade.AddProductCategoryDiscount(GetLoggedInUsr(user), GetStore(storeName), categoryName, deadline, percentage, op, indexInChain, disId, toLeft);
        }

        public void AddSpecificProductDiscount(DataLoggedInUser user, string storeName, string productName, DateTime deadline, double percentage,
                                                Operator op, int indexInChain, int disId, bool toLeft)
        {
            ManageFacade.AddSpecificProductDiscount(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName, productName), deadline, percentage, op, indexInChain, disId, toLeft);
        }

        public void RemoveDiscount(DataLoggedInUser user, string storeName, int indexInChain)
        {
            ManageFacade.RemoveDiscount(GetLoggedInUsr(user), GetStore(storeName), indexInChain);
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
            //preserve loggedIn user's cart that he gathered as a GuestUser.
            //Cart cart = GetUser(userAsGuest).Cart;
            LoggedInUser loggedIn = UserFacade.GetLoggedInUser(username, password);
            //loggedIn.Cart = cart;
            if (loggedIn is Administrator)
                return new DataAdministrator((Administrator)loggedIn);
            return new DataLoggedInUser(loggedIn);
        }

        public IEnumerable<DataPurchase> ManagingPurchaseHistory(DataLoggedInUser user, string storeName)
        {
            return ManageFacade.ViewPurchaseHistory(GetLoggedInUsr(user), GetStore(storeName)).Select(prchs => new DataPurchase(prchs));
        }

        public DataMessage MessageReply(DataLoggedInUser user, DataMessage message, string storeName, string description)
        {
            //message will always be the first message in the talk
            var store = GetStore(storeName);
            Message? firstMsg = store.Messages.FirstOrDefault(msg => message.Represents(msg));
            if (firstMsg is null)
            {
                throw new MessageNotInSystemException();
            }
            Message toAnswerOn = firstMsg;
            while (toAnswerOn.Next != null)
            {
                toAnswerOn = toAnswerOn.Next;
            }
            var loggedIn = GetLoggedInUsr(user);
            DataMessage msg;
            if(firstMsg.WrittenBy == loggedIn)
            {
                //It's the first user, and it's the non-manager who initiated the messages
                msg = new DataMessage(loggedIn.MessageReplyAsNotManager(toAnswerOn, description));
                return msg;
            }
            //It's the manager who should answer it
            msg = new DataMessage(ManageFacade.MessageReply(loggedIn, toAnswerOn, GetStore(storeName), description));
            return msg;
        }

        public IEnumerable<DataBasket> MyCart(DataUser user)
        {
            var cart = DatabaseProxy.Instance.Carts.FirstOrDefault(cart => user.Username == cart.Username);
            if (cart == default)
                return user.Cart.Baskets.ToList();
            return cart.Baskets.ToList().Select(bskt => new DataBasket(bskt));
        }

        public void OpenStore(DataLoggedInUser user, string storeName)
        {
            StoreFacade.CreateStore(GetLoggedInUsr(user), storeName);
        }

        public IEnumerable<DataPurchase> PurchaseHistory(DataLoggedInUser user)
        {
            return UserFacade.PurchaseHistory(GetLoggedInUsr(user)).Select(prchs => new DataPurchase(prchs));
        }

        public DataPurchase Purchase(DataUser dataUser, DataBasket basket, string creditCardNum, DateTime expirationDate, string cvv, Address address, string username, string id)
        {
            var user = GetUser(dataUser);
            Basket? trueBasket = user.Cart.Baskets.FirstOrDefault(bskt => basket.Id == bskt.Id);
            if (trueBasket is null)
            {
                throw new BasketNotInSystemException();
            }
            var prchs = new DataPurchase(UserFacade.Purchase(user, trueBasket, creditCardNum, expirationDate, cvv, address, username, id));
            return prchs;
        }

        public void Register(string username, byte[] password)
        {
            UserFacade.Register(username, password);
        }

        public void RemoveProduct(DataLoggedInUser user, string storeName, string productName)
        {
            Store store = GetStore(storeName);
            Product product = GetProduct(storeName, productName);
            ManageFacade.RemoveProduct(GetLoggedInUsr(user), store, product);
        }

        public void RemoveProductFromCart(DataUser user, string storeName, string productName, int quantity)
        {
            var store = StoreFacade.SearchStore(str => str.Name.Equals(storeName)).FirstOrDefault();
            if (store is null)
            {
                throw new StoreNotInTradingSystemException();
            }
            UserFacade.RemoveProductFromCart(GetUser(user), GetProduct(storeName, productName), quantity);
        }

        public void RemoveStoreManager(DataLoggedInUser user, string storeName, string username)
        {
            LoggedInUser manager = UserFacade.GetLoggedInUser(username);
            ManageFacade.RemoveStoreManager(GetLoggedInUsr(user), GetStore(storeName), manager);
        }

        public void RemoveStoreOwner(DataLoggedInUser user, string storeName, string username)
        {
            LoggedInUser owner = UserFacade.GetLoggedInUser(username);
            ManageFacade.RemoveStoreOwner(GetLoggedInUsr(user), GetStore(storeName), owner);
        }

        private IEnumerable<Product> SearchProducts(Func<Product, bool> pred)
        {
            return StoreFacade.SearchProducts(pred);
        }

        public IEnumerable<DataProduct> SearchProductsByCategory(string input)
        {
            string localInput = input;
            return SearchProducts(product => product.Category.ToLower().Replace('_', ' ').Equals(localInput)).Select(prod => new DataProduct(prod)); 
        }

        public IEnumerable<DataProduct> SearchProductsByKeywords(string input)
        {
            static bool HasWordInsideOther(string[] words1, List<string> words2)
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
            string localInput = input;
            return SearchProducts(product => product.Name.ToLower().Replace('_', ' ').Equals(localInput)).Select(prod => new DataProduct(prod));
        }

        public void SetPermissionsOfManager(DataLoggedInUser user, string storeName, string username, Authorizations authorization)
        {
            ManageFacade.SetPermissionsOfManager(GetLoggedInUsr(user), GetStore(storeName), GetLoggedInUsr(username), authorization);
        }

        public IEnumerable<DataPurchase> StorePurchaseHistory(DataLoggedInUser user, string storeName)
        {
            return UserFacade.StorePurchaseHistory(GetLoggedInUsr(user), GetStore(storeName)).Select(prchs => new DataPurchase(prchs));
        }

        public IEnumerable<DataPurchase> UserPurchaseHistory(DataLoggedInUser user, string userNm)
        {
            return UserFacade.UserPurchaseHistory(GetLoggedInUsr(user), userNm).Select(prchs => new DataPurchase(prchs));
        }

        public IEnumerable<DataMessage> ViewMessage(DataLoggedInUser user, string storeName)
        {
            return ManageFacade.ViewMessage(GetLoggedInUsr(user), GetStore(storeName)).Select(msg => new DataMessage(msg));
        }

        public DataMessage WriteMessage(DataLoggedInUser user, string storeName, string description)
        {
            var msg = new DataMessage(UserFacade.WriteMessage(GetLoggedInUsr(user), GetStore(storeName), description));
            return msg;
        }

        public void WriteReview(DataLoggedInUser user, string storeName, string productName, string description)
        {
            UserFacade.WriteReview(GetLoggedInUsr(user), GetProduct(storeName, productName), description);
            DatabaseProxy.Instance.SaveChanges();
        }

        public DataGuestUser CreateGuest()
        {
            var guestUsr = UserFacade.CreateGuestUser();
            return new DataGuestUser(guestUsr);
        }

        public void AddAlwaysTruePolicy(DataLoggedInUser user, string storeName, Operator op)
        {
            ManageFacade.AddAlwaysTruePolicy(GetLoggedInUsr(user), GetStore(storeName), op);
        }

        public void AddSingleProductQuantityPolicy(DataLoggedInUser user, string storeName, Operator op, string productName, int minQuantity, int maxQuantity)
        {
            ManageFacade.AddSingleProductQuantityPolicy(GetLoggedInUsr(user), GetStore(storeName), op,
                    GetProduct(storeName, productName), minQuantity, maxQuantity);
        }

        public void AddSystemDayPolicy(DataLoggedInUser user, string storeName, Operator op, Weekday cantBuyIn)
        {
            ManageFacade.AddSystemDayPolicy(GetLoggedInUsr(user), GetStore(storeName), op, cantBuyIn);
        }

        public void AddUserCityPolicy(DataLoggedInUser user, string storeName, Operator op, string requiredCity)
        {
            ManageFacade.AddUserCityPolicy(GetLoggedInUsr(user), GetStore(storeName), op, requiredCity);
        }

        public void AddUserCountryPolicy(DataLoggedInUser user, string storeName, Operator op, string requiredCountry)
        {
            ManageFacade.AddUserCountryPolicy(GetLoggedInUsr(user), GetStore(storeName), op, requiredCountry);
        }

        public void AddWholeStoreQuantityPolicy(DataLoggedInUser user, string storeName, Operator op, int minQuantity, int maxQuantity)
        {
            ManageFacade.AddWholeStoreQuantityPolicy(GetLoggedInUsr(user), GetStore(storeName), op, minQuantity, maxQuantity);
        }

        public void RemovePolicy(DataLoggedInUser user, string storeName, int indexInChain)
        {
            ManageFacade.RemovePolicy(GetLoggedInUsr(user), GetStore(storeName), indexInChain);
        }

        public void MarkAllDiscussionAsRead(DataLoggedInUser user, string storeName, DataMessage msg)
        {
            var store = GetStore(storeName);
            Message? firstMsg = store.Messages.FirstOrDefault(message => msg.Represents(message));
            if (firstMsg is null)
            {
                return;
            }
            if (msg.WrittenBy.Equals(user))
            {
                Message? currMsg = firstMsg;
                while(currMsg != null)
                {
                    currMsg.ClientSawIt = true;
                    currMsg = currMsg.Next;
                }
            }
            else
            {
                Message? currMsg = firstMsg;
                while (currMsg != null)
                {
                    currMsg.StoreSawIt = true;
                    currMsg = currMsg.Next;
                }
            }
        }

        public void AddBuySomeGetSomeDiscount(DataLoggedInUser user, string storeName, string prod1Name, string prod2Name, int buySome, int getSome, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft)
        {
            ManageFacade.AddBuySomeGetSomeDiscount(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName,prod1Name), GetProduct(storeName, prod2Name), buySome, getSome, deadline, percentage, op, indexInChain, disId, toLeft);
        }

        public void AddBuyOverDiscount(DataLoggedInUser user, string storeName, string productName, double minSum, DateTime deadline, double percentage, Operator op, int indexInChain, int disId, bool toLeft)
        {
            ManageFacade.AddBuyOverDiscount(GetLoggedInUsr(user), GetStore(storeName), GetProduct(storeName,productName), percentage, deadline, minSum, op, indexInChain, disId, toLeft);
        }
        public IEnumerable<string> GetRegisteredUsers()
        {
            return UserFacade.GetRegisteredUsers();
        }

        public double GetIncomeInDate(DateTime date)
        {
            return UserFacade.GetIncomeInDate(date);
        }

        public IDictionary<DateTime, IDictionary<KindOfUser, int>> GetUseRecord(DateTime dateFrom, DateTime dateTo)
        {
            return UserFacade.GetUseRecord(dateFrom, dateTo);
        }

        public IDictionary<KindOfUser, int> GetUsersByCategory(DateTime today)
        {
            return UserFacade.GetUsersByCategory(today);
        }
    }
}
