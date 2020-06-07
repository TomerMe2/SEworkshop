using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.DAL;

namespace SEWorkshop.Tests.IntegrationTests
{
    [TestFixture]
    public class UserFacadeTest
    {
        private IUserFacade UsrFacade { get; set; }
        private IManageFacade MngrFacade { get; set; }
        private IStoreFacade StrFacade { get; set; }

        private ISecurityAdapter securityAdaprer = new SecurityAdapter();
        
        const string CREDIT_CARD_NUMBER_STUB = "1234";
        const string CITY_NAME_STUB = "Beer Sheva";
        const string STREET_NAME_STUB = "Shderot Ben Gurion";
        const string HOUSE_NUMBER_STUB = "111";
        const string COUNTRY_STUB = "Israel";
        Address address = new Address(COUNTRY_STUB, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB);

        [OneTimeSetUp]
        public void SetUp()
        {
            MngrFacade = new ManageFacade();
            StrFacade = new StoreFacade(DatabaseProxy.Instance);
            UsrFacade = new UserFacade(StrFacade, DatabaseProxy.Instance);
        }

        //public LoggedInUser GetUser(string username)
        [Test]
        public void GetUser_UserExists_ReturnUser()
        {
            UsrFacade.Register("gue1", securityAdaprer.Encrypt("1111"));
            LoggedInUser result = UsrFacade.GetLoggedInUser("gue1");
            Assert.That(result.Username, Is.EqualTo("gue1"));
        }
        
        [Test]
        public void GetUser_UserNotExists_ThrowException()
        {
            try
            {
                UsrFacade.GetLoggedInUser("gue2");
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
        public void Register_UserAlreadyExists_ThrowsException()
        {
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
            var result = UsrFacade.Register("rune1", securityAdaprer.Encrypt("1111"));
            Assert.That(result.Username, Is.EqualTo("rune1"));
        }
        /*
        
        [Test]
        public void Login_WrongUserName_ThrowException()
        {
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
        
            */
        [Test]
        public void AddProductToCart_BasketAlreadyInCart_OneBasketInCart()
        {
            LoggedInUser aptcbaic_user1 = UsrFacade.Register("aptcbaic_user1", securityAdaprer.Encrypt("1111"));
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
            LoggedInUser aptcnb_user1 = UsrFacade.Register("aptcnb_user1", securityAdaprer.Encrypt("1111"));
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
        
        [Test]
        public void RemoveProductFromCart_ProductExists_ProductRemoved()
        {
            LoggedInUser rpfcpe_user1 = UsrFacade.Register("rpfcpe_user1", securityAdaprer.Encrypt("1111"));
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

        [Test]
        public void Purchase_EmptyBasket_ThrowsException()
        {
            LoggedInUser peb_user1 = UsrFacade.Register("peb_user1", securityAdaprer.Encrypt("1111"));
            Store peb_store1 = new Store(peb_user1, "peb_store1");
            string peb_creditCardNumber = "1234";
            Address peb_address1 = new Address("Israel", "Beer Sheva", "Shderot Ben Gurion", "111");
            try
            {
                UsrFacade.Purchase(peb_user1, new Basket(peb_store1, peb_user1.Cart), peb_creditCardNumber, peb_address1);
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
            LoggedInUser php_user1 = UsrFacade.Register("php_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(php_user1, "php_store1");
            Store php_store1 = StrFacade.SearchStore(store => store.Name.Equals("php_store1")).ElementAt(0);
            MngrFacade.AddProduct(php_user1, php_store1, "php_product1", "nininini", "cat1", 11.11, 1);
            Product php_product1 = php_store1.Products.First(product => product.Name.Equals("php_product1"));
            UsrFacade.AddProductToCart(php_user1, php_product1, 1);
            Purchase p = new Purchase(php_user1, php_user1.Cart.Baskets.ElementAt(0), address);
            string peb_creditCardNumber = "1234";
            Address peb_address = new Address("Israel", "Beer Sheva", "Shderot Ben Gurion", "111");
            UsrFacade.Purchase(php_user1, php_user1.Cart.Baskets.ElementAt(0), peb_creditCardNumber, peb_address);

            foreach (var purchase in UsrFacade.PurchaseHistory(php_user1))
            {
                if (purchase.Basket.Store.Name.Equals(p.Basket.Store.Name))
                {
                    pass = true;
                }
            }
            Assert.True(pass);
        }

        /*
        [Test]
        public void PurchaseHistory_NoPermission_ThrowsException()
        {
            LoggedInUser phnp_user1 = UsrFacade.Register("phnp_user1", securityAdaprer.Encrypt("1111"));
            try
            {
                UsrFacade.PurchaseHistory(phnp_user1);
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                Assert.True(true);
            }
        }
        */
        
        [Test]
        public void PurchaseHistory_UserNotExists_ReturnEmptyList()
        {
            var result = UsrFacade.PurchaseHistory(new LoggedInUser("phune_user1", securityAdaprer.Encrypt("5555")));
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public void PurchaseHistory_UserExists_ReturnPurchaseHistory()
        {
            LoggedInUser phue_user1 = UsrFacade.Register("phue_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(phue_user1, "phue_store1");
            Store phue_store1 = StrFacade.SearchStore(store => store.Name.Equals("phue_store1")).ElementAt(0);
            MngrFacade.AddProduct(phue_user1, phue_store1, "phue_product1", "nininini", "cat1", 11.11, 1);
            Product phue_product1 = phue_store1.Products.First(product => product.Name.Equals("phue_product1"));
            MngrFacade.AddProduct(phue_user1, phue_store1, "phue_product2", "blablabla", "cat1", 11.11, 1);
            Product phue_product2 = phue_store1.Products.First(product => product.Name.Equals("phue_product2"));
            UsrFacade.AddProductToCart(phue_user1, phue_product1, 1);
            UsrFacade.AddProductToCart(phue_user1, phue_product2, 1);
            var basket = phue_user1.Cart.Baskets.ElementAt(0);
            Purchase p = new Purchase(phue_user1, basket, address);
            UsrFacade.Purchase(phue_user1, phue_user1.Cart.Baskets.ElementAt(0), CREDIT_CARD_NUMBER_STUB, address);
            var result = UsrFacade.PurchaseHistory(phue_user1);

            Assert.That(result.ElementAt(0).User, Is.EqualTo(p.User));
            Assert.That(result.ElementAt(0).Basket, Is.EqualTo(p.Basket));
        }
        
        [Test]
        public void UserPurchaseHistory_RequestingNotAdministrator_ThrowsException()
        {
            LoggedInUser uphrna_user1 = UsrFacade.Register("uphrna_user1", securityAdaprer.Encrypt("1111"));
            LoggedInUser uphrna_user2 = UsrFacade.Register("uphrna_user2", securityAdaprer.Encrypt("1111"));
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
            LoggedInUser uphria_user1 = UsrFacade.Register("uphria_user1", securityAdaprer.Encrypt("1111"));
            StrFacade.CreateStore(uphria_user1, "uphria_store1");
            Store uphria_store1 = StrFacade.SearchStore(store => store.Name.Equals("uphria_store1")).ElementAt(0);
            MngrFacade.AddProduct(uphria_user1, uphria_store1, "uphria_product1", "nininini", "cat1", 11.11 ,1);
            Product uphria_product1 = uphria_store1.Products.First(product => product.Name.Equals("uphria_product1"));
            MngrFacade.AddProduct(uphria_user1, uphria_store1, "uphria_product2", "blablabla", "cat1", 11.11, 1);
            Product uphria_product2 = uphria_store1.Products.First(product => product.Name.Equals("uphria_product2"));
            UsrFacade.AddProductToCart(uphria_user1, uphria_product1, 1);
            UsrFacade.AddProductToCart(uphria_user1, uphria_product2, 1);
            string peb_creditCardNumber = "1234";
            Address peb_address = new Address("Israel", "Beer Sheva", "Shderot Ben Gurion", "111");
            UsrFacade.Purchase(uphria_user1, uphria_user1.Cart.Baskets.ElementAt(0), peb_creditCardNumber, peb_address);
            
            LoggedInUser uphria_admin1 = UsrFacade.GetLoggedInUser("admin", securityAdaprer.Encrypt("sadnaTeam"));
            var result = UsrFacade.UserPurchaseHistory(uphria_admin1, "uphria_user1");
                        
            CollectionAssert.AreEqual(UsrFacade.PurchaseHistory(uphria_user1), result);
        }
        
        [Test]
        public void StorePurchaseHistory_RequestingNotAdministrator_ThrowsException()
        {
            LoggedInUser sphrna_user1 = UsrFacade.Register("sphrna_user1", securityAdaprer.Encrypt("1111"));
            Store sphrna_store1 = new Store(sphrna_user1, "sphrna_store1");
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
            LoggedInUser sphnp_user1 = UsrFacade.Register("sphnp_user1", securityAdaprer.Encrypt("1111"));
            Store sphnp_store1 = new Store(sphnp_user1, "sphnp_store1");
            LoggedInUser sphnp_admin1 = UsrFacade.GetLoggedInUser("admin", securityAdaprer.Encrypt("sadnaTeam"));
            Assert.That(UsrFacade.StorePurchaseHistory(sphnp_admin1, sphnp_store1), Is.Empty);
        }
        

        [Test]
        public void WriteReview_HasPermission_AddReview()
        {
            bool passed = false;
            LoggedInUser wrhp_user1 = UsrFacade.Register("wrhp_user1", securityAdaprer.Encrypt("1111"));
            Store wrhp_store1 = new Store(wrhp_user1, "wrhp_store1");
            Product wrhp_product1 = new Product(wrhp_store1, "wrhp_product2", "blablabla", "cat1", 11.11, 1);
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

        /*
        
        //public void WriteMessage(User user, Store store, string description)
        [Test]
        public void WriteMessage_NoPermission_ThrowsException()
        {
            LoggedInUser wmnp_user1 = UsrFacade.Register("wmnp_user1", securityAdaprer.Encrypt("1111"));
            Store wmnp_store1 = new Store(wmnp_user1, "wmnp_store1");
            try
            {
                UsrFacade.WriteMessage(wmnp_user1, wmnp_store1, "Do you have any bananas?");
                Assert.Fail();
            }
            catch (UserHasNoPermissionException)
            {
                
            }
        }

    */
        
        [Test]
        public void WriteMessage_HasPermission_AddMessage()
        {
            bool passed = false;
            LoggedInUser wmhp_user1 = UsrFacade.Register("wmhp_user1", securityAdaprer.Encrypt("1111"));
            Store wmhp_store1 = new Store(wmhp_user1, "wmhp_store1");

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

        [Test]
        public void IncomeInDateTest()
        {
            //THIS TEST CAN FAIL AROUND MIDNIGHT. DO NOT RUN IT AROUND MIDNIGHT.
            double incomeBefore = UsrFacade.GetIncomeInDate(DateTime.Now);
            var usr1 = UsrFacade.Register("IncomeInDateTestUsr1", new byte[] { 0 });
            var usr2 = UsrFacade.Register("IncomeInDateTestUsr2", new byte[] { 0 });
            var store = StrFacade.CreateStore(usr1, "IncomeInDateTestStr");
            var prod = new Product(store, "someProd", "nini", "someCat", 123, 99);
            store.Products.Add(prod);
            UsrFacade.AddProductToCart(usr2, prod, 5);
            var basket = usr2.Cart.Baskets.First(bskt => bskt.Store.Name.Equals(store.Name));
            UsrFacade.Purchase(usr2, basket, "1234", new Address("nini", "nana", "wallak", "ahla"));
            double increaseShouldBe = basket.PriceAfterDiscount();
            double incomeNow = UsrFacade.GetIncomeInDate(DateTime.Now);
            // This weird compare is done to avoid floating number representation issues
            Assert.IsTrue(Math.Abs((incomeNow - incomeBefore) - increaseShouldBe) <= 0.0001);
        }
    }
}