using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;

namespace SEWorkshop.UnitTests
{
    [TestFixture]
    public class UserFacadeTest
    {
        private IUserFacade UsrFacade { get; set; }
        private IManageFacade MngrFacade { get; set; }
        private IStoreFacade StrFacade { get; set; }

        private ISecurityAdapter securityAdaprer = new SecurityAdapter();

        [OneTimeSetUp]
        public void SetUp()
        {
            MngrFacade = new ManageFacade();
            StrFacade = new StoreFacade();
            UsrFacade = new UserFacade(StrFacade);
        }

        //public LoggedInUser GetUser(string username)
        [Test]
        public void GetUser_UserExists_ReturnUser()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            UsrFacade.Register("gue1", securityAdaprer.Encrypt("1111"));
            LoggedInUser result = UsrFacade.GetUser("gue1");
            Assert.That(result.Username, Is.EqualTo("gue1"));
        }
        
        [Test]
        public void GetUser_UserNotExists_ThrowException()
        {
            try
            {
                UsrFacade.GetUser("gue2");
                Assert.Fail();
            }
            catch (UserDoesNotExistException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        //public LoggedInUser Register(string username, byte[] password)
        [Test]
        public void Register_UserIsLoggedIn_ThrowsException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            UsrFacade.Register("rul1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("rul1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.Register("rul2", securityAdaprer.Encrypt("pass"));
                
                Assert.Fail();
            }
            catch (UserAlreadyLoggedInException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Register_UserAlreadyExists_ThrowsException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            UsrFacade.Register("rue1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.Register("rue1", securityAdaprer.Encrypt("pass"));
                Assert.Fail();
            }
            catch (UserAlreadyExistsException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Register_AdministratorAlreadyExists_ThrowsException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            try
            {
                UsrFacade.Register("admin", securityAdaprer.Encrypt("sadnaTeam"));
                Assert.Fail();
            }
            catch (UserAlreadyExistsException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Register_UserNotExists_ReturnUser()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            var result = UsrFacade.Register("rune1", securityAdaprer.Encrypt("1111"));
            Assert.That(result.Username, Is.EqualTo("rune1"));
        }
        
        //public LoggedInUser Login(string username, byte[] password)
        [Test]
        public void Login_UserAlreadyLoggedIn_ThrowException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            UsrFacade.Register("luali1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("luali1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.Login("luali1", securityAdaprer.Encrypt("1111"));
                Assert.Fail();
            }
            catch (UserAlreadyLoggedInException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
        }
        
        [Test]
        public void Login_WrongUserName_ThrowException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }

            UsrFacade.Register("lwun1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.Login("wrong", securityAdaprer.Encrypt("1111"));
                Assert.Fail();
            }
            catch (UserDoesNotExistException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Login_WrongPassword_ThrowException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }

            UsrFacade.Register("lwp1", securityAdaprer.Encrypt("1111"));
            try
            {
                var result = UsrFacade.Login("lwp1", securityAdaprer.Encrypt("2222"));
                Assert.Fail();
            }
            catch (UserDoesNotExistException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void Login_UserNotLoggedIn_ReturnUser()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }

            UsrFacade.Register("lunl1", securityAdaprer.Encrypt("1111"));
            var result = UsrFacade.Login("lunl1", securityAdaprer.Encrypt("1111"));
            Assert.That(result.Username, Is.EqualTo("lunl1"));
            Assert.That(UsrFacade.HasPermission, Is.True);
        }

        [Test]
        public void Login_AdministratorWrongPassword_ThrowException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }

            try
            {
                UsrFacade.Login("admin", securityAdaprer.Encrypt("wrong"));
                Assert.Fail();
            }
            catch (UserDoesNotExistException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Login_AdministratorNotLoggedIn_AdministratorLoggedIn()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            UsrFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            Assert.That(UsrFacade.HasPermission, Is.True);
        }
        
        //public void Logout()
        [Test]
        public void Logout_NoPermission_ThrowsException()
        {
            try
            {
                UsrFacade.Logout();
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Logout_HasPermission_LogoutUser()
        {
            UsrFacade.Register("lhp1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("lhp1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.Logout();
                Assert.That(UsrFacade.HasPermission, Is.False);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            
        }
        
        //public void AddProductToCart(User user, Product product)
        [Test]
        public void AddProductToCart_BasketAlreadyInCart_OneBasketInCart()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser aptcbaic_user1 = UsrFacade.Register("aptcbaic_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("aptcbaic_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(aptcbaic_user1, "aptcbaic_store1");
            Store aptcbaic_store1 = StrFacade.SearchStore(store => store.Name.Equals("aptcbaic_store1")).ElementAt(0);
            MngrFacade.AddProduct(aptcbaic_user1, aptcbaic_store1, "aptcbaic_product1",
                "nininini", "cat1", 11.11, 1);
            MngrFacade.AddProduct(aptcbaic_user1, aptcbaic_store1, "aptcbaic_product2",
                "blablabla", "cat1", 11.11, 1);
            Product prod1 = aptcbaic_store1.Products.First(product => product.Name.Equals("aptcbaic_product1"));
            UsrFacade.AddProductToCart(aptcbaic_user1, prod1, 1);
            
            UsrFacade.AddProductToCart(aptcbaic_user1, aptcbaic_store1.Products.First(product => product.Name.Equals("aptcbaic_product2")), 1);
            
            Assert.That(aptcbaic_user1.Cart.Baskets.Count(), Is.EqualTo(1));
        }
        
        [Test]
        public void AddProductToCart_NewBasket_NewBasketInCart()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser aptcnb_user1 = UsrFacade.Register("aptcnb_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("aptcnb_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(aptcnb_user1, "aptcnb_store1");
            Store aptcnb_store1 = StrFacade.SearchStore(store => store.Name.Equals("aptcnb_store1")).ElementAt(0);
            StrFacade.CreateStore(aptcnb_user1, "aptcnb_store2");
            Store aptcnb_store2 = StrFacade.SearchStore(store => store.Name.Equals("aptcnb_store2")).ElementAt(0);
            MngrFacade.AddProduct(aptcnb_user1, aptcnb_store1, "aptcnb_product1", "nininini", "cat1", 11.11, 1);
            MngrFacade.AddProduct(aptcnb_user1, aptcnb_store2, "aptcnb_product2", "blablabla", "cat1", 11.11, 1);
            UsrFacade.AddProductToCart(aptcnb_user1, aptcnb_store1.Products.First(product => product.Name.Equals("aptcnb_product1")), 1);
            UsrFacade.AddProductToCart(aptcnb_user1, aptcnb_store2.Products.First(product => product.Name.Equals("aptcnb_product2")), 1);
            
            Assert.That(aptcnb_user1.Cart.Baskets.Count(), Is.EqualTo(2));
        }
        
        //public void RemoveProductFromCart(User user, Product product)
        //TODO: check if needed more general case (more stores)
        [Test]
        public void RemoveProductFromCart_ProductExists_ProductRemoved()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser rpfcpe_user1 = UsrFacade.Register("rpfcpe_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("rpfcpe_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(rpfcpe_user1, "rpfcpe_store1");
            Store rpfcpe_store1 = StrFacade.SearchStore(store => store.Name.Equals("rpfcpe_store1")).ElementAt(0);
            MngrFacade.AddProduct(rpfcpe_user1, rpfcpe_store1, "rpfcpe_product1", "nininini", "cat1", 11.11, 1);
            Product rpfcpe_product1 = rpfcpe_store1.Products.First(product => product.Name.Equals("rpfcpe_product1"));
            MngrFacade.AddProduct(rpfcpe_user1, rpfcpe_store1, "rpfcpe_product2", "blablabla", "cat1", 11.11, 1);
            Product rpfcpe_product2 = rpfcpe_store1.Products.First(product => product.Name.Equals("rpfcpe_product2"));
            UsrFacade.AddProductToCart(rpfcpe_user1, rpfcpe_product1, 1);
            UsrFacade.AddProductToCart(rpfcpe_user1, rpfcpe_product2, 1);
            int size = rpfcpe_user1.Cart.Baskets.Where(basket => basket.Store.Name.Equals(rpfcpe_store1.Name))
                .ElementAt(0).Products.Count();
            UsrFacade.RemoveProductFromCart(rpfcpe_user1, rpfcpe_product1, 1);
            
            Assert.That(rpfcpe_user1.Cart.Baskets.Where(basket => basket.Store.Name.Equals(rpfcpe_store1.Name)).ElementAt(0).Products.Count().Equals(size-1), Is.True);
        }

        //public void Purchase(User user, Basket basket)
        [Test]
        public void Purchase_EmptyBasket_ThrowsException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser peb_user1 = UsrFacade.Register("peb_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("peb_user1", securityAdaprer.Encrypt("1111"));
            Store peb_store1 = new Store(peb_user1, "peb_store1");
            string peb_creditCardNumber = "1234";
            Address peb_address1 = new Address("Beer Sheva", "Shderot Ben Gurion", "111");
            try
            {
                UsrFacade.Purchase(peb_user1, new Basket(peb_store1), peb_creditCardNumber, peb_address1);
                Assert.Fail();
            }
            catch (BasketIsEmptyException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void Purchase_HasPermission_LoggedInUserPurchase()
        {
            bool pass = false;
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser php_user1 = UsrFacade.Register("php_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("php_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(php_user1, "php_store1");
            Store php_store1 = StrFacade.SearchStore(store => store.Name.Equals("php_store1")).ElementAt(0);
            MngrFacade.AddProduct(php_user1, php_store1, "php_product1", "nininini", "cat1", 11.11, 1);
            Product php_product1 = php_store1.Products.First(product => product.Name.Equals("php_product1"));
            UsrFacade.AddProductToCart(php_user1, php_product1, 1);
            Purchase p = new Purchase(php_user1, php_user1.Cart.Baskets.ElementAt(0));
            string peb_creditCardNumber = "1234";
            Address peb_address = new Address("Beer Sheva", "Shderot Ben Gurion", "111");
            UsrFacade.Purchase(php_user1, php_user1.Cart.Baskets.ElementAt(0), peb_creditCardNumber, peb_address);
            /*userFacade.RemoveProductFromCart(php_user1, php_product1, 1);*/

            foreach (var purchase in UsrFacade.PurcahseHistory(php_user1))
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
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser phnp_user1 = UsrFacade.Register("phnp_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.PurcahseHistory(phnp_user1);
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void PurchaseHistory_UserNotExists_ReturnEmptyList()
        {
            var result = UsrFacade.PurcahseHistory(new LoggedInUser("phune_user1", securityAdaprer.Encrypt("5555")));
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public void PurchaseHistory_UserExists_ReturnPurchaseHistory()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser phue_user1 = UsrFacade.Register("phue_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("phue_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(phue_user1, "phue_store1");
            Store phue_store1 = StrFacade.SearchStore(store => store.Name.Equals("phue_store1")).ElementAt(0);
            MngrFacade.AddProduct(phue_user1, phue_store1, "phue_product1", "nininini", "cat1", 11.11, 1);
            Product phue_product1 = phue_store1.Products.First(product => product.Name.Equals("phue_product1"));
            MngrFacade.AddProduct(phue_user1, phue_store1, "phue_product2", "blablabla", "cat1", 11.11, 1);
            Product phue_product2 = phue_store1.Products.First(product => product.Name.Equals("phue_product2"));
            UsrFacade.AddProductToCart(phue_user1, phue_product1, 1);
            UsrFacade.AddProductToCart(phue_user1, phue_product2, 1);
            Purchase p = new Purchase(phue_user1, phue_user1.Cart.Baskets.ElementAt(0));
            string peb_creditCardNumber = "1234";
            Address peb_address = new Address("Beer Sheva", "Shderot Ben Gurion", "111");
            UsrFacade.Purchase(phue_user1, phue_user1.Cart.Baskets.ElementAt(0), peb_creditCardNumber, peb_address);
            var result = UsrFacade.PurcahseHistory(phue_user1);

            Assert.That(result.ElementAt(0).User, Is.EqualTo(p.User));
            Assert.That(result.ElementAt(0).Basket, Is.EqualTo(p.Basket));
        }
        
        //public IEnumerable<Purchase> UserPurchaseHistory(LoggedInUser requesting, LoggedInUser user)
        [Test]
        public void UserPurchaseHistory_RequestingNotAdministrator_ThrowsException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser uphrna_user1 = UsrFacade.Register("uphrna_user1", securityAdaprer.Encrypt("1111"));
            LoggedInUser uphrna_user2 = UsrFacade.Register("uphrna_user2", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("uphrna_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.UserPurchaseHistory(uphrna_user1, "uphrna_user2");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void UserPurchaseHistory_RequestingIsAdministrator_ReturnPurchaseHistory()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser uphria_user1 = UsrFacade.Register("uphria_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("uphria_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(uphria_user1, "uphria_store1");
            Store uphria_store1 = StrFacade.SearchStore(store => store.Name.Equals("uphria_store1")).ElementAt(0);
            MngrFacade.AddProduct(uphria_user1, uphria_store1, "uphria_product1", "nininini", "cat1", 11.11 ,1);
            Product uphria_product1 = uphria_store1.Products.First(product => product.Name.Equals("uphria_product1"));
            MngrFacade.AddProduct(uphria_user1, uphria_store1, "uphria_product2", "blablabla", "cat1", 11.11, 1);
            Product uphria_product2 = uphria_store1.Products.First(product => product.Name.Equals("uphria_product2"));
            UsrFacade.AddProductToCart(uphria_user1, uphria_product1, 1);
            UsrFacade.AddProductToCart(uphria_user1, uphria_product2, 1);
            string peb_creditCardNumber = "1234";
            Address peb_address = new Address("Beer Sheva", "Shderot Ben Gurion", "111");
            UsrFacade.Purchase(uphria_user1, uphria_user1.Cart.Baskets.ElementAt(0), peb_creditCardNumber, peb_address);
            UsrFacade.Logout();
            
            LoggedInUser uphria_admin1 = UsrFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            var result = UsrFacade.UserPurchaseHistory(uphria_admin1, "uphria_user1");
            UsrFacade.Logout();
            
            UsrFacade.Login("uphria_user1", securityAdaprer.Encrypt("1111"));
            
            CollectionAssert.AreEqual(UsrFacade.PurcahseHistory(uphria_user1), result);
        }
        
        //public IEnumerable<Purchase> StorePurchaseHistory(LoggedInUser requesting, Store store)
        [Test]
        public void StorePurchaseHistory_RequestingNotAdministrator_ThrowsException()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser sphrna_user1 = UsrFacade.Register("sphrna_user1", securityAdaprer.Encrypt("1111"));
            Store sphrna_store1 = new Store(sphrna_user1, "sphrna_store1");
            UsrFacade.Login("sphrna_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.StorePurchaseHistory(sphrna_user1, sphrna_store1);
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void StorePurchaseHistory_NoPurchasesForStore_ReturnEmpyList()
        {
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser sphnp_user1 = UsrFacade.Register("sphnp_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.Login("sphnp_user1", securityAdaprer.Encrypt("1111"));
            Store sphnp_store1 = new Store(sphnp_user1, "sphnp_store1");
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser sphnp_admin1 = UsrFacade.Login("admin", securityAdaprer.Encrypt("sadnaTeam"));
            Assert.That(UsrFacade.StorePurchaseHistory(sphnp_admin1, sphnp_store1), Is.Empty);
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
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser wrnp_user1 = UsrFacade.Register("wrnp_user1", securityAdaprer.Encrypt("1111"));
            Store wrnp_store1 = new Store(wrnp_user1, "wrnp_store1");
            Product wrnp_product1 = new Product(wrnp_store1, "wrnp_product2", "blablabla", "cat1", 11.11, 1);
            try
            {
                UsrFacade.WriteReview(wrnp_user1, wrnp_product1, "Great book!");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void WriteReview_HasPermission_AddReview()
        {
            bool passed = false;
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser wrhp_user1 = UsrFacade.Register("wrhp_user1", securityAdaprer.Encrypt("1111"));
            Store wrhp_store1 = new Store(wrhp_user1, "wrhp_store1");
            Product wrhp_product1 = new Product(wrhp_store1, "wrhp_product2", "blablabla", "cat1", 11.11, 1);
            UsrFacade.Login("wrhp_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.WriteReview(wrhp_user1, wrhp_product1, "This is a bad book :(");

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
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser wmnp_user1 = UsrFacade.Register("wmnp_user1", securityAdaprer.Encrypt("1111"));
            Store wmnp_store1 = new Store(wmnp_user1, "wmnp_store1");
            try
            {
                UsrFacade.WriteMessage(wmnp_user1, wmnp_store1, "Do you have any bananas?");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }
        
        [Test]
        public void WriteMessage_HasPermission_AddMessage()
        {
            bool passed = false;
            if (UsrFacade.HasPermission)
            {
                UsrFacade.Logout();
            }
            LoggedInUser wmhp_user1 = UsrFacade.Register("wmhp_user1", securityAdaprer.Encrypt("1111"));
            Store wmhp_store1 = new Store(wmhp_user1, "wmhp_store1");

            UsrFacade.Login("wmhp_user1", securityAdaprer.Encrypt("1111"));
            UsrFacade.WriteMessage(wmhp_user1, wmhp_store1, "I love your store");

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