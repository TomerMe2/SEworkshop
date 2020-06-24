using NUnit.Framework;
using System;
using System.Collections.Generic;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using System.Linq;
using SEWorkshop.Models;
using SEWorkshop.DAL;

namespace SEWorkshop.Tests.IntegrationTests
{
    [TestFixture]
    class UserManagerTests
    {
        private IUserManager Manager { get; set; }
        private const string DEF_ID = "UserManagerTests";

        [OneTimeSetUp]
        public void Init()
        {
            DatabaseProxy.MoveToTestDb();
            Manager = new UserManager();
        }

        [SetUp]
        public void InitEachTest()
        {
            try
            {
                Manager.Logout(DEF_ID);
            }
            catch { }
        }

        [Test]
        public void CartTest()
        {
            Manager.Register(DEF_ID, "CartTest1", "CartNINI");
            Manager.Login(DEF_ID, "CartTest1", "CartNINI");
            Manager.OpenStore(DEF_ID, "CartTestStr");
            Manager.AddProduct(DEF_ID, "CartTestStr", "CartTestItm1", "nini", "testCart", 123, 2);
            Manager.AddProduct(DEF_ID, "CartTestStr", "CartTestItm2", "nini", "testCart", 123, 7);
            Manager.AddProduct(DEF_ID, "CartTestStr", "CartTestItm3", "nini", "testCart", 123, 2);
            Manager.Logout(DEF_ID);
            Manager.Register(DEF_ID, "CartTest2", "hi");

            Manager.AddProductToCart(DEF_ID, "CartTestStr", "CartTestItm1", 1);
            var cart = Manager.MyCart(DEF_ID);
            Assert.IsTrue(cart.Count() == 1);
            var prod = cart.First().Products.First();
            Assert.IsTrue(prod.Product.Name.Equals("CartTestItm1") && prod.Quantity == 1);
            bool isExceptionThrown = false;
            try
            {
                Manager.AddProductToCart(DEF_ID, "CartTestStr", "CartTestItm3", 3);
            }
            catch(NegativeInventoryException)
            {
                isExceptionThrown = true;
            }
            Assert.IsTrue(isExceptionThrown);

            Manager.AddProductToCart(DEF_ID, "CartTestStr", "CartTestItm1", 1);
            cart = Manager.MyCart(DEF_ID);
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 1);
            prod = cart.First().Products.First();
            Assert.IsTrue(prod.Product.Name.Equals("CartTestItm1") && prod.Quantity == 2);

            Manager.AddProductToCart(DEF_ID, "CartTestStr", "CartTestItm2", 3);
            cart = Manager.MyCart(DEF_ID);
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 2);

            Manager.RemoveProductFromCart(DEF_ID, "CartTestStr", "CartTestItm2", 2);
            cart = Manager.MyCart(DEF_ID);
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 2);
            Assert.IsTrue(cart.First().Products.Where(tup => tup.Product.Name.Equals("CartTestItm2") && tup.Quantity == 1).Any());

            Manager.RemoveProductFromCart(DEF_ID, "CartTestStr", "CartTestItm2", 1);
            cart = Manager.MyCart(DEF_ID);
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 1);
            Assert.IsFalse(cart.First().Products.Where(tup => tup.Product.Name.Equals("CartTestItm2")).Any());
        }

        [Test]
        public void BrowseStoresTest()
        {
            Manager.Register(DEF_ID, "BrowseStoresTest", "77777");
            Manager.Login(DEF_ID, "BrowseStoresTest", "77777");
            int countBeforeAdd = Manager.BrowseStores().Count();
            Manager.OpenStore(DEF_ID, "Browse1");
            Manager.OpenStore(DEF_ID, "Browse2");
            var stores = Manager.BrowseStores();
            Assert.IsTrue(countBeforeAdd + 2 == stores.Count());
            Assert.IsTrue(stores.Where(store => store.Name.Equals("Browse1")).Count() > 0);
            Assert.IsTrue(stores.Where(store => store.Name.Equals("Browse2")).Count() > 0);
        }

        [Test]
        public void SearchAndFilterTest()
        {
            Manager.Register(DEF_ID, "SearchAndFilter", "999");
            Manager.Login(DEF_ID, "SearchAndFilter", "999");
            Manager.OpenStore(DEF_ID, "SearchAndFilterStr");
            Manager.AddProduct(DEF_ID, "SearchAndFilterStr", "bamba", "yummi", "snacks", 3.5, 9);
            Manager.AddProduct(DEF_ID, "SearchAndFilterStr", "bisli", "yummmmiii", "snacks", 4, 15);
            Manager.AddProduct(DEF_ID, "SearchAndFilterStr", "Drill", "POWERHOUSE", "tools", 450, 2);
            string cat = "snacks";
            var res = Manager.SearchProductsByCategory(ref cat);
            Assert.IsTrue(res.All(prod => prod.Category.Equals(cat)));
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bamba")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bisli")).Count() > 0);

            res = Manager.FilterProducts(res.ToList(), prod => prod.Price < 4);
            Assert.IsTrue(res.Count() > 0);
            Assert.IsTrue(res.All(prod => prod.Price < 4));
            
            string name = "bamba";
            res = Manager.SearchProductsByName(ref name);
            Assert.IsTrue(res.Count() > 0);
            Assert.IsTrue(res.All(prod => prod.Name.Equals("bamba")));

            string keywords = "yummi POWERHOUSE";
            res = Manager.SearchProductsByKeywords(ref keywords);
            Assert.IsTrue(res.Count() > 1);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bamba")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bisli")).Count() == 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("Drill")).Count() > 0);

            string keywordsNotGood = "yumi POERHOUSE";
            res = Manager.SearchProductsByKeywords(ref keywordsNotGood);
            Assert.AreEqual("yummi powerhouse", keywordsNotGood.ToLower());
            Assert.IsTrue(res.Count() > 1);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bamba")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bisli")).Count() == 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("Drill")).Count() > 0);
        }

        [Test]
        public void RegisterLoginLogoutTest()
        {
            Manager.Register(DEF_ID, "LoginTest", "1234");
            bool exceptionIsThrown = false;
            try
            {
                Manager.Login(DEF_ID, "LoginTestNotInThere", "789");
            }
            catch(UserDoesNotExistException)
            {
                exceptionIsThrown = true;
            }
            Assert.IsTrue(exceptionIsThrown);
            exceptionIsThrown = false;
            try
            {
                Manager.Login(DEF_ID, "LoginTest", "789");
            }
            catch (UserDoesNotExistException)
            {
                exceptionIsThrown = true;
            }
            Assert.IsTrue(exceptionIsThrown);
            Manager.Login(DEF_ID, "LoginTest", "1234");
            exceptionIsThrown = false;
            try
            {
                Manager.Login(DEF_ID, "LoginTest", "1234");
            }
            catch (UserAlreadyLoggedInException)
            {
                exceptionIsThrown = true;
            }
            Assert.IsTrue(exceptionIsThrown);
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "LoginTest", "1234");
        }

        [Test]
        public void OpenRemoveStoreTest()
        {
            Manager.Register(DEF_ID, "OpenStoreTest", "777");
            bool isExcetionThrown = false;
            try
            {
                Manager.OpenStore(DEF_ID, "check");
            }
            catch(UserHasNoPermissionException)
            {
                isExcetionThrown = true;
            }
            Assert.IsTrue(isExcetionThrown);
            isExcetionThrown = false;
            Manager.Login(DEF_ID, "OpenStoreTest", "777");
            Manager.OpenStore(DEF_ID, "check");
            try
            {
                Manager.OpenStore(DEF_ID, "check");
            }
            catch (StoreWithThisNameAlreadyExistsException)
            {
                isExcetionThrown = true;
            }
            Assert.IsTrue(isExcetionThrown);
        }

        public void Purchase()
        {
            //already tested in other functions
        }

        public void PurchaseAndPurcahseHistory()
        {
            Manager.Register(DEF_ID, "PurcahseHistory_user1", "1111");
            Manager.Register(DEF_ID, "PurchaseHistory_user2", "1111");
            Manager.Login(DEF_ID, "rPurchaseHistory_user1", "1111");
            Manager.OpenStore(DEF_ID, "PurchaseHistory_store1");
            Manager.AddStoreOwner(DEF_ID, "PurchaseHistory_store1", "PurchaseHistory_user2");
            Manager.SetPermissionsOfManager(DEF_ID, "UserPurchaseHistory_store1", "PurchaseHistory_user2", "Products");
            Manager.AddProduct(DEF_ID, "PurchaseHistory_store1", "ph_prod1", "ninini", "cat1", 11, 1);
            Manager.AddProductToCart(DEF_ID, "PurchaseHistoryy_store1", "ph_prod1", 1);
            var basket = Manager.MyCart(DEF_ID).ElementAt(0);
            string creditCardNumber = "1234";
            Address address = new Address("Israel", "Beer Sheva", "Shderot Ben Gurion", "76");
            Manager.Purchase(DEF_ID, basket, creditCardNumber, address);
            var result = Manager.PurchaseHistory(DEF_ID);
            Assert.That(result.ElementAt(0).Basket, Is.EqualTo(basket));
        }

        [Test]
        public void WriteReview()
        {
            Manager.Register(DEF_ID, "WriteReviewUser1", "1111");
            Manager.Register(DEF_ID, "WriteReviewUser2", "1111");
            Manager.Login(DEF_ID, "WriteReviewUser1", "1111");
            Manager.OpenStore(DEF_ID, "WriteReviewStore1");
            Manager.AddStoreManager(DEF_ID, "WriteReviewStore1", "WriteReviewUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "WriteReviewStore1", "WriteReviewUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "WriteReviewUser2", "1111");
            Manager.AddProduct(DEF_ID, "WriteReviewStore1", "wr_prod1", "ninini", "cat1", 11.11, 1);
            Manager.WriteReview(DEF_ID, "WriteReviewStore1", "wr_prod1", "bad review");
            string searchString = "wr_prod1";
            var review = Manager.SearchProductsByName(ref searchString).ElementAt(0).Reviews.ElementAt(0);
            Assert.That(review.Description, Is.EqualTo("bad review"));
        }

        public void WriteMessage()
        {
            //already checked in other test
        }

        [Test]
        public void UserPurchaseHistory()
        {
            Manager.Register(DEF_ID, "UserPurchaseHistory_user1", "1111");
            Manager.Register(DEF_ID, "UserPurchaseHistory_user2", "1111");
            Manager.Login(DEF_ID, "UserPurchaseHistory_user1", "1111");
            Manager.OpenStore(DEF_ID, "UserPurchaseHistory_store1");
            Manager.AddStoreManager(DEF_ID, "UserPurchaseHistory_store1", "UserPurchaseHistory_user2");
            Manager.SetPermissionsOfManager(DEF_ID, "UserPurchaseHistory_store1", "UserPurchaseHistory_user2", "Products");
            Manager.AddProduct(DEF_ID, "UserPurchaseHistory_store1", "uph_prod1", "ninini", "cat1", 11, 1);
            Manager.AddProductToCart(DEF_ID, "UserPurchaseHistory_store1", "uph_prod1", 1);
            var basket = Manager.MyCart(DEF_ID).First(bskt => bskt.Store.Name.Equals("UserPurchaseHistory_store1"));
            string creditCardNumber = "1234";
            Address address = new Address("Israel", "Beer Sheva", "Shderot Ben Gurion", "76");
            Manager.Purchase(DEF_ID, basket, creditCardNumber, address);
            var result = Manager.PurchaseHistory(DEF_ID);
            Assert.That(result.First(prchs => prchs.Basket.Store.Name.Equals("UserPurchaseHistory_store1")).Basket, Is.EqualTo(basket));
        }

        [Test]
        public void ManagingPurchaseHistory()
        {
            Manager.Register(DEF_ID, "ManagingPurchaseHistory_user1", "1111");
            Manager.Register(DEF_ID, "ManagingPurchaseHistory_user2", "1111");
            Manager.Login(DEF_ID, "ManagingPurchaseHistory_user1", "1111");
            Manager.OpenStore(DEF_ID, "ManagingPurchaseHistory_store1");
            Manager.AddStoreManager(DEF_ID, "ManagingPurchaseHistory_store1", "ManagingPurchaseHistory_user2");
            Manager.SetPermissionsOfManager(DEF_ID, "ManagingPurchaseHistory_store1", "ManagingPurchaseHistory_user2", "Products");
            Manager.AddProduct(DEF_ID, "ManagingPurchaseHistory_store1", "productUnoutchedlLolUnique", "ninini", "cat1", 11, 1);
            Manager.AddProductToCart(DEF_ID, "ManagingPurchaseHistory_store1", "productUnoutchedlLolUnique", 1);
            var checkcheck = Manager.MyCart(DEF_ID);
            var basket = Manager.MyCart(DEF_ID).First(bskt => bskt.Store.Name.Equals("ManagingPurchaseHistory_store1"));
            string creditCardNumber = "1234";
            Address address = new Address("Israel", "Beer Sheva", "Shderot Ben Gurion", "76");
            Manager.Purchase(DEF_ID, basket, creditCardNumber, address);
            var result = Manager.ManagingPurchaseHistory(DEF_ID, "ManagingPurchaseHistory_store1");
            Assert.That(result.First().Basket, Is.EqualTo(basket));
        }

        [Test]
        public void AddProductTest()
        {
            Manager.Register(DEF_ID, "AddProduct", "999");
            Manager.Login(DEF_ID, "AddProduct", "999");
            Manager.OpenStore(DEF_ID, "AddProductStr");
            Manager.AddProduct(DEF_ID, "AddProductStr", "Wakanda", "forever", "lol", 111111111, 1);
            var searchStr = "Wakanda";
            Assert.IsTrue(Manager.SearchProductsByName(ref searchStr).Any());
            Assert.AreEqual(searchStr, "wakanda");
            bool exceptionWasThrown = false;
            try
            {
                Manager.AddProduct(DEF_ID, "AddProductStr", "Wakanda", "forever", "lol", 111111111, 1);
            }
            catch (ProductAlreadyExistException)
            {
                exceptionWasThrown = true;
            }
            Assert.IsTrue(exceptionWasThrown);
        }

        [Test]
        public void RemoveProductTest()
        {
            Manager.Register(DEF_ID, "RemoveProduct", "999");
            Manager.Login(DEF_ID, "RemoveProduct", "999");
            Manager.OpenStore(DEF_ID, "RemoveProductStr");
            Manager.AddProduct(DEF_ID, "RemoveProductStr", "RemTestItem", "nini", "nana", 12, 14);
            Manager.RemoveProduct(DEF_ID, "RemoveProductStr", "RemTestItem");
            string searchStr = "RemTestItem";
            var res = Manager.SearchProductsByName(ref searchStr);
            Assert.IsTrue(!searchStr.Equals("RemTestItem") || !res.Any());
        }

        [Test]
        public void AddStoreOwner()
        {
            Manager.Register(DEF_ID, "AddStoreOwner1", "999");
            Manager.Register(DEF_ID, "AddStoreOwner2", "wallak");
            Manager.Login(DEF_ID, "AddStoreOwner1", "999");
            Manager.OpenStore(DEF_ID, "AddStoreOwnerStr");
            Manager.AddStoreOwner(DEF_ID, "AddStoreOwnerStr", "AddStoreOwner2");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "AddStoreOwner2", "wallak");
            Manager.AddProduct(DEF_ID, "AddStoreOwnerStr", "kloom", "hi", "mashoo", 666, 666);
        }

        [Test]
        public void AddStoreManagerAndSetPermissionAndRemoveManagerTest()
        {
            Manager.Register(DEF_ID, "AddStoreManager1", "999");
            Manager.Register(DEF_ID, "AddStoreManager2", "wallak");
            Manager.Login(DEF_ID, "AddStoreManager1", "999");
            Manager.OpenStore(DEF_ID, "AddStoreManagerStr");
            Manager.AddStoreManager(DEF_ID, "AddStoreManagerStr", "AddStoreManager2");
            Manager.SetPermissionsOfManager(DEF_ID, "AddStoreManagerStr", "AddStoreManager2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "AddStoreManager2", "wallak");
            Manager.AddProduct(DEF_ID, "AddStoreManagerStr", "kloom", "hi", "mashoo", 666, 666);
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "AddStoreManager1", "999");
            Manager.RemoveStoreManager(DEF_ID, "AddStoreManagerStr", "AddStoreManager2");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "AddStoreManager2", "wallak");
            bool isException = false;
            try
            {
                Manager.AddProduct(DEF_ID, "AddStoreManagerStr", "kloom2", "hi", "mashoo", 666, 666);
            }
            catch (UserHasNoPermissionException)
            {
                isException = true;
            }
            Assert.IsTrue(isException);
        }

        public void WirteViewMessage()
        {
            Manager.Register(DEF_ID, "ViewMessageUser1", "1111");
            Manager.Register(DEF_ID, "ViewMessageUser2", "1111");
            Manager.Login(DEF_ID, "ViewMessageUser1", "1111");
            Manager.OpenStore(DEF_ID, "ViewMessageStore1");
            Manager.AddStoreManager(DEF_ID, "ViewMessageStore1", "ViewMessageUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "ViewMessageStore1", "ViewMessageUser2", "Replying");
            Manager.SetPermissionsOfManager(DEF_ID, "ViewMessageStore1", "ViewMessageUser2", "Watching");
            Manager.SetPermissionsOfManager(DEF_ID, "ViewMessageStore1", "ViewMessageUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "ViewMessageUser2", "1111");
            Manager.WriteMessage(DEF_ID, "ViewMessageStore1", "Hello");
            var message = Manager.ViewMessage(DEF_ID, "ViewMessageStore1").ElementAt(0);
            Assert.That(message.Description, Is.EqualTo("hello"));
        }

        [Test]
        public void MessageReply()
        {
            Manager.Register(DEF_ID, "MessageReplyUser1", "1111");
            Manager.Register(DEF_ID, "MessageReplyUser2", "1111");
            Manager.Login(DEF_ID, "MessageReplyUser1", "1111");
            Manager.OpenStore(DEF_ID, "MessageReplyStore1");
            Manager.AddStoreManager(DEF_ID, "MessageReplyStore1", "MessageReplyUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "MessageReplyStore1", "MessageReplyUser2", "Replying");
            Manager.SetPermissionsOfManager(DEF_ID, "MessageReplyStore1", "MessageReplyUser2", "Watching");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "MessageReplyUser2", "1111");
            Manager.WriteMessage(DEF_ID, "MessageReplyStore1", "Hello");
            var message = Manager.ViewMessage(DEF_ID, "MessageReplyStore1").First();
            Manager.MessageReply(DEF_ID, message, "MessageReplyStore1", "reply message");
            var reply = message.Next;
            Assert.That(reply.Description, Is.EqualTo("reply message"));
        }

        [Test]
        public void EditProductName()
        {
            Manager.Register(DEF_ID, "EditProductNameUser1", "1111");
            Manager.Register(DEF_ID, "EditProductNameUser2", "1111");
            Manager.Login(DEF_ID, "EditProductNameUser1", "1111");
            Manager.OpenStore(DEF_ID, "EditProductNameStore1");
            Manager.AddStoreManager(DEF_ID, "EditProductNameStore1", "EditProductNameUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "EditProductNameStore1", "EditProductNameUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "EditProductNameUser2", "1111");
            Manager.AddProduct(DEF_ID, "EditProductNameStore1", "epn_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductName(DEF_ID, "EditProductNameStore1", "epn_prod1", "newName");
            string searchStr = "newName";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Name, Is.EqualTo("newName"));
            Assert.That(res.Name, Is.Not.EqualTo("epn_prod1"));
        }

        [Test]
        public void EditProductCategory()
        {
            Manager.Register(DEF_ID, "EditProductCategoryUser1", "1111");
            Manager.Register(DEF_ID, "EditProductCategoryUser2", "1111");
            Manager.Login(DEF_ID, "EditProductCategoryUser1", "1111");
            Manager.OpenStore(DEF_ID, "EditProductCategoryStore1");
            Manager.AddStoreManager(DEF_ID, "EditProductCategoryStore1", "EditProductCategoryUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "EditProductCategoryStore1", "EditProductCategoryUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "EditProductCategoryUser2", "1111");
            Manager.AddProduct(DEF_ID, "EditProductCategoryStore1", "epc_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductCategory(DEF_ID, "EditProductCategoryStore1", "epc_prod1", "newCat");
            string searchStr = "epp_prod1";    // theres a typo here
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Category, Is.EqualTo("newCat"));
            Assert.That(res.Category, Is.Not.EqualTo("cat1"));
        }

        [Test]
        public void EditProductPrice()
        {
            Manager.Register(DEF_ID, "EditProductPriceUser1", "1111");
            Manager.Register(DEF_ID, "EditProductPriceUser2", "1111");
            Manager.Login(DEF_ID, "EditProductPriceUser1", "1111");
            Manager.OpenStore(DEF_ID, "EditProductPriceStore1");
            Manager.AddStoreManager(DEF_ID, "EditProductPriceStore1", "EditProductPriceUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "EditProductPriceStore1", "EditProductPriceUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "EditProductPriceUser2", "1111");
            Manager.AddProduct(DEF_ID, "EditProductPriceStore1", "epp_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductPrice(DEF_ID, "EditProductPriceStore1", "epp_prod1", 12.12);
            string searchStr = "epp_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Price, Is.EqualTo(12.12));
            Assert.That(res.Price, Is.Not.EqualTo(11.11));
        }

        [Test]
        public void EditProductQuantity()
        {
            Manager.Register(DEF_ID, "EditProductQuantityUser1", "1111");
            Manager.Register(DEF_ID, "EditProductQuantityUser2", "1111");
            Manager.Login(DEF_ID, "EditProductQuantityUser1", "1111");
            Manager.OpenStore(DEF_ID, "EditProductQuantityStore1");
            Manager.AddStoreManager(DEF_ID, "EditProductQuantityStore1", "EditProductQuantityUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "EditProductQuantityStore1", "EditProductQuantityUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "EditProductQuantityUser2", "1111");
            Manager.AddProduct(DEF_ID, "EditProductQuantityStore1", "epq_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductQuantity(DEF_ID, "EditProductQuantityStore1", "epq_prod1", 3);
            string searchStr = "epq_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Quantity, Is.EqualTo(3));
            Assert.That(res.Quantity, Is.Not.EqualTo(1));
        }

        [Test]
        public void EditProductDescription()
        {
            Manager.Register(DEF_ID, "EditProductDescriptionUser1", "1111");
            Manager.Register(DEF_ID, "EditProductDescriptionUser2", "1111");
            Manager.Login(DEF_ID, "EditProductDescriptionUser1", "1111");
            Manager.OpenStore(DEF_ID, "EditProductDescriptionStore1");
            Manager.AddStoreManager(DEF_ID, "EditProductDescriptionStore1", "EditProductDescriptionUser2");
            Manager.SetPermissionsOfManager(DEF_ID, "EditProductDescriptionStore1", "EditProductDescriptionUser2", "Products");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "EditProductDescriptionUser2", "1111");
            Manager.AddProduct(DEF_ID, "EditProductDescriptionStore1", "epd_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductDescription(DEF_ID, "EditProductDescriptionStore1", "epd_prod1", "new");
            string searchStr = "epd_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Description, Is.EqualTo("new"));
            Assert.That(res.Description, Is.Not.EqualTo("old"));
        }

        [Test]
        public void LoginFromDifferentSessions()
        {
            Manager.Register("SessionLoginFromDifferentSessions1", "LoginFromDifferentSessions1", "1234");
            Manager.Register("SessionLoginFromDifferentSessions2", "LoginFromDifferentSessions2", "1234");
            Manager.Login("SessionLoginFromDifferentSessions1", "LoginFromDifferentSessions1", "1234");
            Manager.Login("SessionLoginFromDifferentSessions2", "LoginFromDifferentSessions2", "1234");
            Assert.Throws<UserAlreadyLoggedInException>(() => Manager.Register("SessionLoginFromDifferentSessions1",
                "LoginFromDifferentSessions3", "1234"));
            Assert.Throws<UserAlreadyLoggedInException>(() => Manager.Register("SessionLoginFromDifferentSessions2",
                "LoginFromDifferentSessions4", "1234"));
            Manager.Logout("SessionLoginFromDifferentSessions1");
            Manager.Register("SessionLoginFromDifferentSessions1", "LoginFromDifferentSessions3", "1234");
            Assert.Throws<UserAlreadyLoggedInException>(() => Manager.Register("SessionLoginFromDifferentSessions2",
                "LoginFromDifferentSessions4", "1234"));
        }

        [Test]
        public void StorePurchaseHistory_StoreHasPurchases_ReturnPurchaseHistory()
        {
            Manager.Login(DEF_ID, "admin", "sadnaTeam");
            Manager.OpenStore(DEF_ID, "StorePurchaseHistory_StoreHasPurchases_ReturnPurchaseHistory");
            var result = Manager.StorePurchaseHistory(DEF_ID, "StorePurchaseHistory_StoreHasPurchases_ReturnPurchaseHistory");
            ICollection<Purchase> expected = new List<Purchase>();
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void WriteReview_Multiple_Sessions()
        {
            const string OTHER_ID = "WriteReview_NoPermission_ThrowsException";
            Manager.Register(DEF_ID, "wrnp_user1", "1111");
            Manager.Register(OTHER_ID, "wrnp_user2", "1111");
            Manager.Login(OTHER_ID, "wrnp_user2", "1111");
            Manager.OpenStore(OTHER_ID, "WriteReviewMultipleStore");
            Manager.AddProduct(OTHER_ID, "WriteReviewMultipleStore", "someItem", "someDesc", "anotherCat", 11.1, 3);
            Assert.Throws<UserHasNoPermissionException>( () => Manager.WriteReview(DEF_ID, "WriteReviewMultipleStore",
                                "someItem", "bad review!!!!"));
            Manager.Login(DEF_ID, "wrnp_user1", "1111");
            Manager.WriteReview(DEF_ID, "WriteReviewMultipleStore", "someItem", "bad review!!!!");
        }

        [Test]
        public void PoliciesTest()
        {
            Manager.Register(DEF_ID, "p_user1", "1234");
            Manager.Login(DEF_ID, "p_user1", "1234");
            Manager.OpenStore(DEF_ID, "p_store1");
            Manager.AddSystemDayPolicy(DEF_ID, "p_store1", Enums.Operator.Or, Enums.Weekday.Sunday);
            Manager.AddSystemDayPolicy(DEF_ID, "p_store1", Enums.Operator.Or, Enums.Weekday.Monday);
            Manager.AddSystemDayPolicy(DEF_ID, "p_store1", Enums.Operator.Or, Enums.Weekday.Tuesday);
            Assert.That(Manager.SearchStore("p_store1").Policy is DataModels.Policies.DataSystemDayPolicy);
            Assert.True(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("p_store1").Policy).CantBuyIn == Enums.Weekday.Sunday);
            Assert.True(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("p_store1").Policy.InnerPolicy).CantBuyIn == Enums.Weekday.Monday);
            Assert.True(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("p_store1").Policy.InnerPolicy.InnerPolicy).CantBuyIn == Enums.Weekday.Tuesday);
            Manager.RemovePolicy(DEF_ID, "p_store1", 0);
            Assert.True(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("p_store1").Policy).CantBuyIn == Enums.Weekday.Monday);
            Assert.True(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("p_store1").Policy.InnerPolicy).CantBuyIn == Enums.Weekday.Tuesday);
        }

        [Test]
        public void ActionsTest()
        {
            //DO NOT RUN THIS TEST AROUND MIDNIGHT. IT CAN FAIL AROUND MIDNIGHT.
            Manager.AccessSystem("ActionsTest1");
            Manager.AccessSystem("ActionsTest2");
            Manager.AccessSystem("ActionsTest3");
            Manager.AccessSystem("ActionsTest4");
            //guest
            Manager.AccessSystem("ActionsTest5");

            //admin
            Manager.Login("ActionsTest1", "admin", "sadnaTeam");

            //logged in user no own no manager
            Manager.Register("ActionsTest2", "ActionsTest2", "1234");
            Manager.Login("ActionsTest2", "ActionsTest2", "1234");
            Manager.OpenStore("ActionsTest2", "ActionsTest2Store");

            Manager.Register("ActionsTest4", "ActionsTest4", "1234");
            Manager.AddStoreManager("ActionsTest2", "ActionsTest2Store", "ActionsTest4");

            //logged in user owner
            Manager.Login("ActionsTest3", "ActionsTest2", "1234");

            //logged in user no owner yes manager
            Manager.Login("ActionsTest4", "ActionsTest4", "1234");

            var now = DateTime.Now;

            var dctAll = Manager.GetUseRecord("ActionsTest1", now, now);
            Assert.IsTrue((dctAll.Values).First().Values.First() >= 5);

            Assert.IsTrue((dctAll.Values).First()[Enums.KindOfUser.Guest] >= 1);

            Assert.IsTrue((dctAll.Values).First()[Enums.KindOfUser.Admin] >= 1);

            Assert.IsTrue((dctAll.Values).First()[Enums.KindOfUser.LoggedInNotOwnNotManage] >= 1);

            Assert.IsTrue((dctAll.Values).First()[Enums.KindOfUser.LoggedInYesOwn] >= 1);

            Assert.IsTrue((dctAll.Values).First()[Enums.KindOfUser.LoggedInNoOwnYesManage] >= 1);
        }
    }
}