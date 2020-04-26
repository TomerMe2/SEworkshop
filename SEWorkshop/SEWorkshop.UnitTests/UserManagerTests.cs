using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using System.Linq;
using SEWorkshop.Models;

namespace SEWorkshop.UnitTests
{
    [TestFixture]
    class UserManagerTests
    {
        private IUserManager Manager { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            Manager = new UserManager();
        }

        [SetUp]
        public void InitEachTest()
        {
            try
            {
                Manager.Logout();
            }
            catch { }
        }

        
        [Test]
        public void CartTest()
        {
            Manager.Register("CartTest1", "CartNINI");
            Manager.Login("CartTest1", "CartNINI");
            Manager.OpenStore("CartTestStr");
            Manager.AddProduct("CartTestStr", "CartTestItm1", "nini", "testCart", 123, 2);
            Manager.AddProduct("CartTestStr", "CartTestItm2", "nini", "testCart", 123, 7);
            Manager.AddProduct("CartTestStr", "CartTestItm3", "nini", "testCart", 123, 2);
            Manager.Logout();
            Manager.Register("CartTest2", "hi");

            Manager.AddProductToCart("CartTestStr", "CartTestItm1", 1);
            var cart = Manager.MyCart();
            Assert.IsTrue(cart.Count() == 1);
            var (prod, quantity) = cart.First().Products.First();
            Assert.IsTrue(prod.Name.Equals("CartTestItm1") && quantity == 1);
            bool isExceptionThrown = false;
            try
            {
                Manager.AddProductToCart("CartTestStr", "CartTestItm3", 3);
            }
            catch(NegativeInventoryException)
            {
                isExceptionThrown = true;
            }
            Assert.IsTrue(isExceptionThrown);

            Manager.AddProductToCart("CartTestStr", "CartTestItm1", 1);
            cart = Manager.MyCart();
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 1);
            (prod, quantity) = cart.First().Products.First();
            Assert.IsTrue(prod.Name.Equals("CartTestItm1") && quantity == 2);

            Manager.AddProductToCart("CartTestStr", "CartTestItm2", 3);
            cart = Manager.MyCart();
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 2);

            Manager.RemoveProductFromCart("CartTestStr", "CartTestItm2", 2);
            cart = Manager.MyCart();
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 2);
            Assert.IsTrue(cart.First().Products.Where(tup => tup.Item1.Name.Equals("CartTestItm2") && tup.Item2 == 1).Any());

            Manager.RemoveProductFromCart("CartTestStr", "CartTestItm2", 1);
            cart = Manager.MyCart();
            Assert.IsTrue(cart.Count() == 1 && cart.First().Products.Count() == 1);
            Assert.IsFalse(cart.First().Products.Where(tup => tup.Item1.Name.Equals("CartTestItm2")).Any());


        }
        

        [Test]
        public void BrowseStoresTest()
        {
            Manager.Register("BrowseStoresTest", "77777");
            Manager.Login("BrowseStoresTest", "77777");
            int countBeforeAdd = Manager.BrowseStores().Count();
            Manager.OpenStore("Browse1");
            Manager.OpenStore("Browse2");
            var stores = Manager.BrowseStores();
            Assert.IsTrue(countBeforeAdd + 2 == stores.Count());
            Assert.IsTrue(stores.Where(store => store.Name.Equals("Browse1")).Count() > 0);
            Assert.IsTrue(stores.Where(store => store.Name.Equals("Browse2")).Count() > 0);
        }

        [Test]
        public void SearchAndFilterTest()
        {
            Manager.Register("SearchAndFilter", "999");
            Manager.Login("SearchAndFilter", "999");
            Manager.OpenStore("SearchAndFilterStr");
            Manager.AddProduct("SearchAndFilterStr", "bamba", "yummi", "snacks", 3.5, 9);
            Manager.AddProduct("SearchAndFilterStr", "bisli", "yummmmiii", "snacks", 4, 15);
            Manager.AddProduct("SearchAndFilterStr", "Drill", "POWERHOUSE", "tools", 450, 2);
            string cat = "snacks";
            var res = Manager.SearchProductsByCategory(ref cat);
            Assert.IsTrue(res.All(prod => prod.Category.Equals(cat)));
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bamba")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bisli")).Count() > 0);

            res = Manager.FilterProducts(res.ToList(), prod => prod.Price < 4);
            Assert.IsTrue(res.Count() > 0);
            Assert.IsTrue(res.All(prod => prod.Price < 4));

            string catNotGood = "snackks";
            res = Manager.SearchProductsByCategory(ref catNotGood);
            Assert.AreEqual("snacks", catNotGood);
            Assert.IsTrue(res.All(prod => prod.Category.Equals(catNotGood)));
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bamba")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bisli")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("Drill")).Count() == 0);

            string name = "bamba";
            res = Manager.SearchProductsByName(ref name);
            Assert.IsTrue(res.Count() > 0);
            Assert.IsTrue(res.All(prod => prod.Name.Equals("bamba")));

            string nameNotGood = "bambi";
            res = Manager.SearchProductsByName(ref nameNotGood);
            Assert.AreEqual("bamba", nameNotGood);
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
            Assert.AreEqual("yummi powerhouse", keywordsNotGood);
            Assert.IsTrue(res.Count() > 1);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bamba")).Count() > 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("bisli")).Count() == 0);
            Assert.IsTrue(res.Where(prod => prod.Name.Equals("Drill")).Count() > 0);
        }

        [Test]
        public void RegisterLoginLogoutTest()
        {
            Manager.Register("LoginTest", "1234");
            bool exceptionIsThrown = false;
            try
            {
                Manager.Login("LoginTestNotInThere", "789");
            }
            catch(UserDoesNotExistException)
            {
                exceptionIsThrown = true;
            }
            Assert.IsTrue(exceptionIsThrown);
            exceptionIsThrown = false;
            try
            {
                Manager.Login("LoginTest", "789");
            }
            catch (UserDoesNotExistException)
            {
                exceptionIsThrown = true;
            }
            Assert.IsTrue(exceptionIsThrown);
            Manager.Login("LoginTest", "1234");
            exceptionIsThrown = false;
            try
            {
                Manager.Login("LoginTest", "1234");
            }
            catch (UserAlreadyLoggedInException)
            {
                exceptionIsThrown = true;
            }
            Assert.IsTrue(exceptionIsThrown);
            Manager.Logout();
            Manager.Login("LoginTest", "1234");
        }

        [Test]
        public void OpenRemoveStoreTest()
        {
            Manager.Register("OpenStoreTest", "777");
            bool isExcetionThrown = false;
            try
            {
                Manager.OpenStore("check");
            }
            catch(UserHasNoPermissionException)
            {
                isExcetionThrown = true;
            }
            Assert.IsTrue(isExcetionThrown);
            isExcetionThrown = false;
            Manager.Login("OpenStoreTest", "777");
            Manager.OpenStore("check");
            try
            {
                Manager.OpenStore("check");
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
            Manager.Register("PurcahseHistory_user1", "1111");
            Manager.Register("PurchaseHistory_user2", "1111");
            Manager.Login("rPurchaseHistory_user1", "1111");
            Manager.OpenStore("PurchaseHistory_store1");
            Manager.AddStoreOwner("PurchaseHistory_store1", "PurchaseHistory_user2");
            Manager.SetPermissionsOfManager("UserPurchaseHistory_store1", "PurchaseHistory_user2", "Products");
            Manager.AddProduct("PurchaseHistory_store1", "ph_prod1", "ninini", "cat1", 11, 1);
            Manager.AddProductToCart("PurchaseHistoryy_store1", "ph_prod1", 1);
            var basket = Manager.MyCart().ElementAt(0);
            Manager.Purchase(basket);
            var result = Manager.PurcahseHistory();
            Assert.That(result.ElementAt(0).Basket, Is.EqualTo(basket));
        }

        [Test]
        public void WriteReview()
        {
            Manager.Register("WriteReviewUser1", "1111");
            Manager.Register("WriteReviewUser2", "1111");
            Manager.Login("WriteReviewUser1", "1111");
            Manager.OpenStore("WriteReviewStore1");
            Manager.AddStoreManager("WriteReviewStore1", "WriteReviewUser2");
            Manager.SetPermissionsOfManager("WriteReviewStore1", "WriteReviewUser2", "Products");
            Manager.Logout();
            Manager.Login("WriteReviewUser2", "1111");
            Manager.AddProduct("WriteReviewStore1", "wr_prod1", "ninini", "cat1", 11.11, 1);
            Manager.WriteReview("WriteReviewStore1", "wr_prod1", "bad review");
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
            Manager.Register("UserPurchaseHistory_user1", "1111");
            Manager.Register("UserPurchaseHistory_user2", "1111");
            Manager.Login("UserPurchaseHistory_user1", "1111");
            Manager.OpenStore("UserPurchaseHistory_store1");
            Manager.AddStoreManager("UserPurchaseHistory_store1", "UserPurchaseHistory_user2");
            Manager.SetPermissionsOfManager("UserPurchaseHistory_store1", "UserPurchaseHistory_user2", "Products");
            Manager.AddProduct("UserPurchaseHistory_store1", "uph_prod1", "ninini", "cat1", 11, 1);
            Manager.AddProductToCart("UserPurchaseHistory_store1", "uph_prod1", 1);
            var basket = Manager.MyCart().First(bskt => bskt.Store.Name.Equals("UserPurchaseHistory_store1"));
            Manager.Purchase(basket);
            var result = Manager.PurcahseHistory();
            Assert.That(result.First(prchs => prchs.Basket.Store.Name.Equals("UserPurchaseHistory_store1")).Basket, Is.EqualTo(basket));
        }

        public void StorePurchaseHistory()
        {
            //not sure what is the difference from ManagingPurchaseHistory
        }

        [Test]
        public void ManagingPurchaseHistory()
        {
            Manager.Register("ManagingPurchaseHistory_user1", "1111");
            Manager.Register("ManagingPurchaseHistory_user2", "1111");
            Manager.Login("ManagingPurchaseHistory_user1", "1111");
            Manager.OpenStore("ManagingPurchaseHistory_store1");
            Manager.AddStoreManager("ManagingPurchaseHistory_store1", "ManagingPurchaseHistory_user2");
            Manager.SetPermissionsOfManager("ManagingPurchaseHistory_store1", "ManagingPurchaseHistory_user2", "Products");
            Manager.AddProduct("ManagingPurchaseHistory_store1", "productUnoutchedlLolUnique", "ninini", "cat1", 11, 1);
            Manager.AddProductToCart("ManagingPurchaseHistory_store1", "productUnoutchedlLolUnique", 1);
            var checkcheck = Manager.MyCart();
            var basket = Manager.MyCart().First(bskt => bskt.Store.Name.Equals("ManagingPurchaseHistory_store1"));
            Manager.Purchase(basket);
            var result = Manager.ManagingPurchaseHistory("ManagingPurchaseHistory_store1");
            Assert.That(result.ElementAt(0).Basket, Is.EqualTo(basket));
        }

        [Test]
        public void AddProductTest()
        {
            Manager.Register("AddProduct", "999");
            Manager.Login("AddProduct", "999");
            Manager.OpenStore("AddProductStr");
            Manager.AddProduct("AddProductStr", "Wakanda", "forever", "lol", 111111111, 1);
            var searchStr = "Wakanda";
            Assert.IsTrue(Manager.SearchProductsByName(ref searchStr).Any());
            Assert.AreEqual(searchStr, "Wakanda");
            bool exceptionWasThrown = false;
            try
            {
                Manager.AddProduct("AddProductStr", "Wakanda", "forever", "lol", 111111111, 1);
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
            Manager.Register("RemoveProduct", "999");
            Manager.Login("RemoveProduct", "999");
            Manager.OpenStore("RemoveProductStr");
            Manager.AddProduct("RemoveProductStr", "RemTestItem", "nini", "nana", 12, 14);
            Manager.RemoveProduct("RemoveProductStr", "RemTestItem");
            string searchStr = "RemTestItem";
            var res = Manager.SearchProductsByName(ref searchStr);
            Assert.IsTrue(!searchStr.Equals("RemTestItem") || !res.Any());
        }

        [Test]
        public void AddStoreOwner()
        {
            Manager.Register("AddStoreOwner1", "999");
            Manager.Register("AddStoreOwner2", "wallak");
            Manager.Login("AddStoreOwner1", "999");
            Manager.OpenStore("AddStoreOwnerStr");
            Manager.AddStoreOwner("AddStoreOwnerStr", "AddStoreOwner2");
            Manager.Logout();
            Manager.Login("AddStoreOwner2", "wallak");
            Manager.AddProduct("AddStoreOwnerStr", "kloom", "hi", "mashoo", 666, 666);
        }

        [Test]
        public void AddStoreManagerAndSetPermissionAndRemoveManagerTest()
        {
            Manager.Register("AddStoreManager1", "999");
            Manager.Register("AddStoreManager2", "wallak");
            Manager.Login("AddStoreManager1", "999");
            Manager.OpenStore("AddStoreManagerStr");
            Manager.AddStoreManager("AddStoreManagerStr", "AddStoreManager2");
            Manager.SetPermissionsOfManager("AddStoreManagerStr", "AddStoreManager2", "Products");
            Manager.Logout();
            Manager.Login("AddStoreManager2", "wallak");
            Manager.AddProduct("AddStoreManagerStr", "kloom", "hi", "mashoo", 666, 666);
            Manager.Logout();
            Manager.Login("AddStoreManager1", "999");
            Manager.RemoveStoreManager("AddStoreManagerStr", "AddStoreManager2");
            Manager.Logout();
            Manager.Login("AddStoreManager2", "wallak");
            bool isException = false;
            try
            {
                Manager.AddProduct("AddStoreManagerStr", "kloom2", "hi", "mashoo", 666, 666);
            }
            catch (UserHasNoPermissionException)
            {
                isException = true;
            }
            Assert.IsTrue(isException);
        }

        public void WirteViewMessage()
        {
            Manager.Register("ViewMessageUser1", "1111");
            Manager.Register("ViewMessageUser2", "1111");
            Manager.Login("ViewMessageUser1", "1111");
            Manager.OpenStore("ViewMessageStore1");
            Manager.AddStoreManager("ViewMessageStore1", "ViewMessageUser2");
            Manager.SetPermissionsOfManager("ViewMessageStore1", "ViewMessageUser2", "Replying");
            Manager.SetPermissionsOfManager("ViewMessageStore1", "ViewMessageUser2", "Watching");
            Manager.SetPermissionsOfManager("ViewMessageStore1", "ViewMessageUser2", "Products");
            Manager.Logout();
            Manager.Login("ViewMessageUser2", "1111");
            Manager.WriteMessage("ViewMessageStore1", "Hello");
            var message = Manager.ViewMessage("ViewMessageStore1").ElementAt(0);
            Assert.That(message.Description, Is.EqualTo("hello"));
        }

        [Test]
        public void MessageReply()
        {
            Manager.Register("MessageReplyUser1", "1111");
            Manager.Register("MessageReplyUser2", "1111");
            Manager.Login("MessageReplyUser1", "1111");
            Manager.OpenStore("MessageReplyStore1");
            Manager.AddStoreManager("MessageReplyStore1", "MessageReplyUser2");
            Manager.SetPermissionsOfManager("MessageReplyStore1", "MessageReplyUser2", "Replying");
            Manager.Logout();
            Manager.Login("MessageReplyUser2", "1111");
            Manager.WriteMessage("MessageReplyStore1", "Hello");
            var message = Manager.ViewMessage("MessageReplyStore1").ElementAt(0);
            Manager.MessageReply(message, "MessageReplyStore1", "reply message");
            var reply = message.Next;
            Assert.That(reply.Description, Is.EqualTo("reply message"));
        }

        [Test]
        public void EditProductName()
        {
            Manager.Register("EditProductNameUser1", "1111");
            Manager.Register("EditProductNameUser2", "1111");
            Manager.Login("EditProductNameUser1", "1111");
            Manager.OpenStore("EditProductNameStore1");
            Manager.AddStoreManager("EditProductNameStore1", "EditProductNameUser2");
            Manager.SetPermissionsOfManager("EditProductNameStore1", "EditProductNameUser2", "Products");
            Manager.Logout();
            Manager.Login("EditProductNameUser2", "1111");
            Manager.AddProduct("EditProductNameStore1", "epn_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductName("EditProductNameStore1", "epn_prod1", "newName");
            string searchStr = "newName";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Name, Is.EqualTo("newName"));
            Assert.That(res.Name, Is.Not.EqualTo("epn_prod1"));
        }

        [Test]
        public void EditProductCategory()
        {
            Manager.Register("EditProductCategoryUser1", "1111");
            Manager.Register("EditProductCategoryUser2", "1111");
            Manager.Login("EditProductCategoryUser1", "1111");
            Manager.OpenStore("EditProductCategoryStore1");
            Manager.AddStoreManager("EditProductCategoryStore1", "EditProductCategoryUser2");
            Manager.SetPermissionsOfManager("EditProductCategoryStore1", "EditProductCategoryUser2", "Products");
            Manager.Logout();
            Manager.Login("EditProductCategoryUser2", "1111");
            Manager.AddProduct("EditProductCategoryStore1", "epc_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductCategory("EditProductCategoryStore1", "epc_prod1", "newCat");
            string searchStr = "epp_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Category, Is.EqualTo("newCat"));
            Assert.That(res.Category, Is.Not.EqualTo("cat1"));
        }

        [Test]
        public void EditProductPrice()
        {
            Manager.Register("EditProductPriceUser1", "1111");
            Manager.Register("EditProductPriceUser2", "1111");
            Manager.Login("EditProductPriceUser1", "1111");
            Manager.OpenStore("EditProductPriceStore1");
            Manager.AddStoreManager("EditProductPriceStore1", "EditProductPriceUser2");
            Manager.SetPermissionsOfManager("EditProductPriceStore1", "EditProductPriceUser2", "Products");
            Manager.Logout();
            Manager.Login("EditProductPriceUser2", "1111");
            Manager.AddProduct("EditProductPriceStore1", "epp_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductPrice("EditProductPriceStore1", "epp_prod1", 12.12);
            string searchStr = "epp_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Price, Is.EqualTo(12.12));
            Assert.That(res.Price, Is.Not.EqualTo(11.11));
        }

        [Test]
        public void EditProductQuantity()
        {
            Manager.Register("EditProductQuantityUser1", "1111");
            Manager.Register("EditProductQuantityUser2", "1111");
            Manager.Login("EditProductQuantityUser1", "1111");
            Manager.OpenStore("EditProductQuantityStore1");
            Manager.AddStoreManager("EditProductQuantityStore1", "EditProductQuantityUser2");
            Manager.SetPermissionsOfManager("EditProductQuantityStore1", "EditProductQuantityUser2", "Products");
            Manager.Logout();
            Manager.Login("EditProductQuantityUser2", "1111");
            Manager.AddProduct("EditProductQuantityStore1", "epq_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductQuantity("EditProductQuantityStore1", "epq_prod1", 3);
            string searchStr = "epq_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Quantity, Is.EqualTo(3));
            Assert.That(res.Quantity, Is.Not.EqualTo(1));
        }

        
        [Test]
        public void EditProductDescription()
        {
            Manager.Register("EditProductDescriptionUser1", "1111");
            Manager.Register("EditProductDescriptionUser2", "1111");
            Manager.Login("EditProductDescriptionUser1", "1111");
            Manager.OpenStore("EditProductDescriptionStore1");
            Manager.AddStoreManager("EditProductDescriptionStore1", "EditProductDescriptionUser2");
            Manager.SetPermissionsOfManager("EditProductDescriptionStore1", "EditProductDescriptionUser2", "Products");
            Manager.Logout();
            Manager.Login("EditProductDescriptionUser2", "1111");
            Manager.AddProduct("EditProductDescriptionStore1", "epd_prod1", "old", "cat1", 11.11, 1);
            Manager.EditProductDescription("EditProductDescriptionStore1", "epd_prod1", "new");
            string searchStr = "epd_prod1";
            var res = Manager.SearchProductsByName(ref searchStr).ElementAt(0);
            Assert.That(res.Description, Is.EqualTo("new"));
            Assert.That(res.Description, Is.Not.EqualTo("old"));
        }
    }
}
