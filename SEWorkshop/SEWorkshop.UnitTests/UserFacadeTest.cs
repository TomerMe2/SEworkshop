using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.ServiceLayer;

namespace SEWorkshop.UnitTests
{
    [TestFixture]
    public class UserFacadeTest
    {
        private UserFacade userFacade = UserFacade.GetInstance();
        private ManageFacade manageFacade = ManageFacade.GetInstance();
        private StoreFacade storeFacade = StoreFacade.GetInstance();
        private ISecurityAdapter securityAdaprer = new SecurityAdapter();

        //public LoggedInUser GetUser(string username)
        [Test]
        public void GetUser_UserExists_ReturnUser()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            userFacade.Register("gue1", securityAdaprer.Encrypt("1111"));
            LoggedInUser result = userFacade.GetUser("gue1");
            Assert.That(result.Username, Is.EqualTo("gue1"));
        }
        
        [Test]
        public void GetUser_UserNotExists_ThrowException()
        {
            try
            {
                userFacade.GetUser("gue2");
                Assert.Fail();
            }
            catch (UserDoesNotExistException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        //public LoggedInUser Register(string username, byte[] password)
        [Test]
        public void Register_UserIsLoggedIn_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            userFacade.Register("rul1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("rul1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.Register("rul2", securityAdaprer.Encrypt("pass"));
                
                Assert.Fail();
            }
            catch (UserAlreadyLoggedInException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Register_UserAlreadyExists_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            userFacade.Register("rue1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.Register("rue1", securityAdaprer.Encrypt("pass"));
                Assert.Fail();
            }
            catch (UserAlreadyExistsException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Register_AdministratorAlreadyExists_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            try
            {
                userFacade.Register("admin", securityAdaprer.Encrypt("sadnaTeam"));
                Assert.Fail();
            }
            catch (UserAlreadyExistsException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Register_UserNotExists_ReturnUser()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            var result = userFacade.Register("rune1", securityAdaprer.Encrypt("1111"));
            Assert.That(result.Username, Is.EqualTo("rune1"));
        }
        
        //public LoggedInUser Login(string username, byte[] password)
        [Test]
        public void Login_UserAlreadyLoggedIn_ThrowException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            userFacade.Register("luali1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("luali1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.Login("luali1", securityAdaprer.Encrypt("1111"));
                Assert.Fail();
            }
            catch (UserAlreadyLoggedInException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
            
        }
        
        [Test]
        public void Login_WrongUserName_ThrowException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }

            userFacade.Register("lwun1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.Login("wrong", securityAdaprer.Encrypt("1111"));
                Assert.Fail();
            }
            catch (UserDoesNotExistException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Login_WrongPassword_ThrowException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }

            userFacade.Register("lwp1", securityAdaprer.Encrypt("1111"));
            try
            {
                var result = userFacade.Login("lwp1", securityAdaprer.Encrypt("2222"));
                Assert.Fail();
            }
            catch (UserDoesNotExistException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Login_UserNotLoggedIn_ReturnUser()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }

            userFacade.Register("lunl1", securityAdaprer.Encrypt("1111"));
            var result = userFacade.Login("lunl1", securityAdaprer.Encrypt("1111"));
            Assert.That(result.Username, Is.EqualTo("lunl1"));
            Assert.That(userFacade.HasPermission, Is.True);
        }

        [Test]
        public void Login_AdministratorWrongPassword_ThrowException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }

            try
            {
                userFacade.Login("admin", securityAdaprer.Encrypt("wrong"));
                Assert.Fail();
            }
            catch (UserDoesNotExistException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Login_AdministratorNotLoggedIn_AdministratorLoggedIn()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            userFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            Assert.That(userFacade.HasPermission, Is.True);
        }
        
        //public void Logout()
        [Test]
        public void Logout_NoPermission_ThrowsException()
        {
            try
            {
                userFacade.Logout();
                Assert.Fail();
            }
            catch (UserHasNoPermissionException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Logout_HasPermission_LogoutUser()
        {
            userFacade.Register("lhp1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("lhp1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.Logout();
                Assert.That(userFacade.HasPermission, Is.False);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
            
        }
        
        //public void AddProductToCart(User user, Product product)
        [Test]
        public void AddProductToCart_BasketAlreadyInCart_OneBasketInCart()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser aptcbaic_user1 = userFacade.Register("aptcbaic_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("aptcbaic_user1", securityAdaprer.Encrypt("1111"));
            storeFacade.CreateStore(aptcbaic_user1, "aptcbaic_store1");
            Store aptcbaic_store1 = storeFacade.SearchStore(store => store.Name.Equals("aptcbaic_store1")).ElementAt(0);
            manageFacade.AddProduct(aptcbaic_user1, aptcbaic_store1, "aptcbaic_product1",
                "nininini", "cat1", 11.11, 1);
            manageFacade.AddProduct(aptcbaic_user1, aptcbaic_store1, "aptcbaic_product2",
                "blablabla", "cat1", 11.11, 1);
            Product prod1 = aptcbaic_store1.Products.First(product => product.Name.Equals("aptcbaic_product1"));
            userFacade.AddProductToCart(aptcbaic_user1, prod1, 1);
            
            userFacade.AddProductToCart(aptcbaic_user1, aptcbaic_store1.Products.First(product => product.Name.Equals("aptcbaic_product2")), 1);
            
            Assert.That(aptcbaic_user1.Cart.Baskets.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void AddProductToCart_NewBasket_NewBasketInCart()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser aptcnb_user1 = userFacade.Register("aptcnb_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("aptcnb_user1", securityAdaprer.Encrypt("1111"));
            storeFacade.CreateStore(aptcnb_user1, "aptcnb_store1");
            Store aptcnb_store1 = storeFacade.SearchStore(store => store.Name.Equals("aptcnb_store1")).ElementAt(0);
            storeFacade.CreateStore(aptcnb_user1, "aptcnb_store2");
            Store aptcnb_store2 = storeFacade.SearchStore(store => store.Name.Equals("aptcnb_store2")).ElementAt(0);
            manageFacade.AddProduct(aptcnb_user1, aptcnb_store1, "aptcnb_product1", "nininini", "cat1", 11.11, 1);
            manageFacade.AddProduct(aptcnb_user1, aptcnb_store2, "aptcnb_product2", "blablabla", "cat1", 11.11, 1);
            userFacade.AddProductToCart(aptcnb_user1, aptcnb_store1.Products.First(product => product.Name.Equals("aptcnb_product1")), 1);
            userFacade.AddProductToCart(aptcnb_user1, aptcnb_store2.Products.First(product => product.Name.Equals("aptcnb_product2")), 1);
            
            Assert.That(aptcnb_user1.Cart.Baskets.Count(), Is.EqualTo(2));
        }
        
        //public void RemoveProductFromCart(User user, Product product)
        //TODO: check if needed more general case (more stores)
        [Test]
        public void RemoveProductFromCart_ProductExists_ProductRemoved()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser rpfcpe_user1 = userFacade.Register("rpfcpe_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("rpfcpe_user1", securityAdaprer.Encrypt("1111"));
            storeFacade.CreateStore(rpfcpe_user1, "rpfcpe_store1");
            Store rpfcpe_store1 = storeFacade.SearchStore(store => store.Name.Equals("rpfcpe_store1")).ElementAt(0);
            manageFacade.AddProduct(rpfcpe_user1, rpfcpe_store1, "rpfcpe_product1", "nininini", "cat1", 11.11, 1);
            Product rpfcpe_product1 = rpfcpe_store1.Products.First(product => product.Name.Equals("rpfcpe_product1"));
            manageFacade.AddProduct(rpfcpe_user1, rpfcpe_store1, "rpfcpe_product2", "blablabla", "cat1", 11.11, 1);
            Product rpfcpe_product2 = rpfcpe_store1.Products.First(product => product.Name.Equals("rpfcpe_product2"));
            userFacade.AddProductToCart(rpfcpe_user1, rpfcpe_product1, 1);
            userFacade.AddProductToCart(rpfcpe_user1, rpfcpe_product2, 1);
            int size = rpfcpe_user1.Cart.Baskets.Where(basket => basket.Store.Name.Equals(rpfcpe_store1.Name))
                .ElementAt(0).Products.Count();
            userFacade.RemoveProductFromCart(rpfcpe_user1, rpfcpe_product1, 1);
            
            Assert.That(rpfcpe_user1.Cart.Baskets.Where(basket => basket.Store.Name.Equals(rpfcpe_store1.Name)).ElementAt(0).Products.Count().Equals(size-1), Is.True);
        }

        //public void Purchase(User user, Basket basket)
        [Test]
        public void Purchase_EmptyBasket_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser peb_user1 = userFacade.Register("peb_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("peb_user1", securityAdaprer.Encrypt("1111"));
            Store peb_store1 = new Store(peb_user1, "peb_store1");
            try
            {
                userFacade.Purchase(peb_user1, new Basket(peb_store1));
                Assert.Fail();
            }
            catch (BasketIsEmptyException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Purchase_HasPermission_LoggedInUserPurchase()
        {
            bool pass = false;
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser php_user1 = userFacade.Register("php_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("php_user1", securityAdaprer.Encrypt("1111"));
            storeFacade.CreateStore(php_user1, "php_store1");
            Store php_store1 = storeFacade.SearchStore(store => store.Name.Equals("php_store1")).ElementAt(0);
            manageFacade.AddProduct(php_user1, php_store1, "php_product1", "nininini", "cat1", 11.11, 1);
            Product php_product1 = php_store1.Products.First(product => product.Name.Equals("php_product1"));
            userFacade.AddProductToCart(php_user1, php_product1, 1);
            Purchase p = new Purchase(php_user1, php_user1.Cart.Baskets.ElementAt(0));
            userFacade.Purchase(php_user1, php_user1.Cart.Baskets.ElementAt(0));
            /*userFacade.RemoveProductFromCart(php_user1, php_product1, 1);*/

            foreach (var purchase in userFacade.PurcahseHistory(php_user1))
            {
                if (purchase.Basket.Store.Name.Equals(p.Basket.Store.Name))
                {
                    pass = true;
                }
            }
            Assert.True(pass);
        }

        //public IEnumerable<Purchase> PurcahseHistory(User user)
        [Test]
        public void PurchaseHistory_NoPermission_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser phnp_user1 = userFacade.Register("phnp_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.PurcahseHistory(phnp_user1);
                Assert.Fail();
            }
            catch (UserHasNoPermissionException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void PurchaseHistory_UserNotExists_ReturnEmptyList()
        {
            var result = userFacade.PurcahseHistory(new LoggedInUser("phune_user1", securityAdaprer.Encrypt("5555")));
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public void PurchaseHistory_UserExists_ReturnPurchaseHistory()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser phue_user1 = userFacade.Register("phue_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("phue_user1", securityAdaprer.Encrypt("1111"));
            storeFacade.CreateStore(phue_user1, "phue_store1");
            Store phue_store1 = storeFacade.SearchStore(store => store.Name.Equals("phue_store1")).ElementAt(0);
            manageFacade.AddProduct(phue_user1, phue_store1, "phue_product1", "nininini", "cat1", 11.11, 1);
            Product phue_product1 = phue_store1.Products.First(product => product.Name.Equals("phue_product1"));
            manageFacade.AddProduct(phue_user1, phue_store1, "phue_product2", "blablabla", "cat1", 11.11, 1);
            Product phue_product2 = phue_store1.Products.First(product => product.Name.Equals("phue_product2"));
            userFacade.AddProductToCart(phue_user1, phue_product1, 1);
            userFacade.AddProductToCart(phue_user1, phue_product2, 1);
            Purchase p = new Purchase(phue_user1, phue_user1.Cart.Baskets.ElementAt(0));
            userFacade.Purchase(phue_user1, phue_user1.Cart.Baskets.ElementAt(0));
            var result = userFacade.PurcahseHistory(phue_user1);

            Assert.That(result.ElementAt(0).User, Is.EqualTo(p.User));
            Assert.That(result.ElementAt(0).Basket, Is.EqualTo(p.Basket));
        }
        
        //public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, LoggedInUser user)
        [Test]
        public void UserPurchaseHistory_RequestingNotAdministrator_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser uphrna_user1 = userFacade.Register("uphrna_user1", securityAdaprer.Encrypt("1111"));
            LoggedInUser uphrna_user2 = userFacade.Register("uphrna_user2", securityAdaprer.Encrypt("1111"));
            userFacade.Login("uphrna_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.UserPurchaseHistory(uphrna_user1, "uphrna_user2");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void UserPurchaseHistory_RequestingIsAdministrator_ReturnPurchaseHistory()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser uphria_user1 = userFacade.Register("uphria_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("uphria_user1", securityAdaprer.Encrypt("1111"));
            storeFacade.CreateStore(uphria_user1, "uphria_store1");
            Store uphria_store1 = storeFacade.SearchStore(store => store.Name.Equals("uphria_store1")).ElementAt(0);
            manageFacade.AddProduct(uphria_user1, uphria_store1, "uphria_product1", "nininini", "cat1", 11.11 ,1);
            Product uphria_product1 = uphria_store1.Products.First(product => product.Name.Equals("uphria_product1"));
            manageFacade.AddProduct(uphria_user1, uphria_store1, "uphria_product2", "blablabla", "cat1", 11.11, 1);
            Product uphria_product2 = uphria_store1.Products.First(product => product.Name.Equals("uphria_product2"));
            userFacade.AddProductToCart(uphria_user1, uphria_product1, 1);
            userFacade.AddProductToCart(uphria_user1, uphria_product2, 1);
            userFacade.Purchase(uphria_user1, uphria_user1.Cart.Baskets.ElementAt(0));
            userFacade.Logout();
            
            LoggedInUser uphria_admin1 = userFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            var result = userFacade.UserPurchaseHistory(uphria_admin1, "uphria_user1");
            userFacade.Logout();
            
            userFacade.Login("uphria_user1", securityAdaprer.Encrypt("1111"));
            
            CollectionAssert.AreEqual(userFacade.PurcahseHistory(uphria_user1), result);
        }
        
        //public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store)
        [Test]
        public void StorePurchaseHistory_RequestingNotAdministrator_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser sphrna_user1 = userFacade.Register("sphrna_user1", securityAdaprer.Encrypt("1111"));
            Store sphrna_store1 = new Store(sphrna_user1, "sphrna_store1");
            userFacade.Login("sphrna_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                userFacade.StorePurchaseHistory(sphrna_user1, sphrna_store1);
                Assert.Fail();
            }
            catch (UserHasNoPermissionException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void StorePurchaseHistory_NoPurchasesForStore_ReturnEmpyList()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser sphnp_user1 = userFacade.Register("sphnp_user1", securityAdaprer.Encrypt("1111"));
            userFacade.Login("sphnp_user1", securityAdaprer.Encrypt("1111"));
            Store sphnp_store1 = new Store(sphnp_user1, "sphnp_store1");
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser sphnp_admin1 = userFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            Assert.That(userFacade.StorePurchaseHistory(sphnp_admin1, sphnp_store1), Is.Empty);
        }
        
        /*[Test]
        public void StorePurchaseHistory_StoreHasPurchases_ReturnPurchaseHistory()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }

            userFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            var result = userFacade.StorePurchaseHistory(amit, store1);
            ICollection<Purchase> expected = new List<Purchase>();

            foreach (var user in userFacade.Users)
            {
                foreach (var purchase in userFacade.PurcahseHistory(user))
                {
                    if (purchase.Basket.Store.Name.Equals("store1"))
                    {
                        expected.Add(purchase);
                    }
                }
            }
            CollectionAssert.AreEqual(expected, result);
        }*/
        
        //public void WriteReview(User user, Product product, string description)
        [Test]
        public void WriteReview_NoPermission_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser wrnp_user1 = userFacade.Register("wrnp_user1", securityAdaprer.Encrypt("1111"));
            Store wrnp_store1 = new Store(wrnp_user1, "wrnp_store1");
            Product wrnp_product1 = new Product(wrnp_store1, "wrnp_product2", "blablabla", "cat1", 11.11, 1);
            try
            {
                userFacade.WriteReview(wrnp_user1, wrnp_product1, "Great book!");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void WriteReview_HasPermission_AddReview()
        {
            bool passed = false;
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser wrhp_user1 = userFacade.Register("wrhp_user1", securityAdaprer.Encrypt("1111"));
            Store wrhp_store1 = new Store(wrhp_user1, "wrhp_store1");
            Product wrhp_product1 = new Product(wrhp_store1, "wrhp_product2", "blablabla", "cat1", 11.11, 1);
            userFacade.Login("wrhp_user1", securityAdaprer.Encrypt("1111"));
            userFacade.WriteReview(wrhp_user1, wrhp_product1, "This is a bad book :(");

            foreach (var review in wrhp_user1.Reviews)
            {
                if (review.Description.Equals("This is a bad book :("))
                {
                    passed = true;
                }
            }
            Assert.True(passed);
        }
        
        //public void WriteMessage(User user, Store store, string description)
        [Test]
        public void WriteMessage_NoPermission_ThrowsException()
        {
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser wmnp_user1 = userFacade.Register("wmnp_user1", securityAdaprer.Encrypt("1111"));
            Store wmnp_store1 = new Store(wmnp_user1, "wmnp_store1");
            try
            {
                userFacade.WriteMessage(wmnp_user1, wmnp_store1, "Do you have any bananas?");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void WriteMessage_HasPermission_AddMessage()
        {
            bool passed = false;
            if (userFacade.HasPermission)
            {
                userFacade.Logout();
            }
            LoggedInUser wmhp_user1 = userFacade.Register("wmhp_user1", securityAdaprer.Encrypt("1111"));
            Store wmhp_store1 = new Store(wmhp_user1, "wmhp_store1");

            userFacade.Login("wmhp_user1", securityAdaprer.Encrypt("1111"));
            userFacade.WriteMessage(wmhp_user1, wmhp_store1, "I love your store");

            foreach (var message in wmhp_user1.Messages)
            {
                if (message.Description.Equals("I love your store"))
                {
                    passed = true;
                }
            }
            Assert.True(passed);
        }
    }
}