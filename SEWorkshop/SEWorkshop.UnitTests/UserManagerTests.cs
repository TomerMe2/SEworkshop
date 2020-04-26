using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using System.Linq;

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

            string catNotGood = "snicks";
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

        }

        public void RemoveProductFromCart()
        {

        }

        public void PurcahseHistory()
        {

        }

        public void WriteReview()
        {

        }

        public void WriteMessage()
        {

        }

        public void UserPurchaseHistory()
        {

        }

        public void StorePurchaseHistory()
        {

        }

        public void ManagingPurchaseHistory()
        {

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
            //TODO: ADD A PRODUCT TO A STORE THAT IS NOT MINE
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
        public void AddStoreManagerAndSetPermissionTest()
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
        }

        public void SetPermissionsOfManager()
        {

        }

        public void RemoveStoreManager()
        {

        }

        public void ViewMessage()
        {

        }

        public void MessageReply()
        {

        }

        public void EditProductName()
        {

        }

        public void EditProductCategory()
        {

        }

        public void EditProductPrice()
        {

        }

        public void EditProductQuantity()
        {

        }

        public void EditProductDescription()
        {

        }
    }
}
