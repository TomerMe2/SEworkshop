using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using System.Linq;
using SEWorkshop.Exceptions;

namespace SEWorkshop.UnitTests
{
    [TestFixture]
    class StoreFacadeTests
    {
        private IStoreFacade Facade { get; set; }
        private ISecurityAdapter SecurityAdapter { get; set; } 

        [OneTimeSetUp]
        public void Init()
        {
            Facade = StoreFacade.GetInstance();
            SecurityAdapter = new SecurityAdapter();
        }

        [Test]
        public void CreateStoreTest()
        {
            const string STORE_NAME = "Elit Factory Store";
            LoggedInUser usr = new LoggedInUser("check1", SecurityAdapter.Encrypt("1234"));
            Store store = Facade.CreateStore(usr, STORE_NAME);
            Assert.IsTrue(Facade.SearchStore(str => str == store).Any());
            //check that the same store name cannot be opened
            bool isThrownEx = false;
            try
            {
                Facade.CreateStore(usr, STORE_NAME);
            }
            catch (StoreWithThisNameAlreadyExistsException)
            {
                isThrownEx = true;
            }
            Assert.IsTrue(isThrownEx);
        }

        [Test]
        public void BrowseStoresTest()
        {
            LoggedInUser usr1 = new LoggedInUser("checkNiNi", SecurityAdapter.Encrypt("789"));
            LoggedInUser usr2 = new LoggedInUser("checkNaNa", SecurityAdapter.Encrypt("910"));
            Store store1 = Facade.CreateStore(usr1, "Nini");
            Store store2 = Facade.CreateStore(usr2, "Nana");
            var stores = Facade.BrowseStores();
            Assert.IsTrue(stores.Contains(store1) && stores.Contains(store2));
        }

        [Test]
        public void SearchStoreTest()
        {
            LoggedInUser usr1 = new LoggedInUser("SearchNiNi", SecurityAdapter.Encrypt("789"));
            LoggedInUser usr2 = new LoggedInUser("SearchNaNa", SecurityAdapter.Encrypt("910"));
            Store store1 = Facade.CreateStore(usr1, "StrNini");
            Store store2 = Facade.CreateStore(usr2, "StrNana");
            var result = Facade.SearchStore(store => store.Owners.ContainsKey(usr1));
            Assert.IsTrue(result.Contains(store1) && result.Count == 1);
            result = Facade.SearchStore(store => store.Name.Equals("StrNana"));
            Assert.IsTrue(result.Contains(store2) && result.Count == 1);
            result = Facade.SearchStore(store => store.Name.Equals("StrNana") || store.Name.Equals("StrNini"));
            Assert.IsTrue(result.Contains(store1) && result.Contains(store2) && result.Count == 2);
        }

        [Test]
        public void IsProductExistTest()
        {
            LoggedInUser usr = new LoggedInUser("IsProductExistTest", SecurityAdapter.Encrypt("555555555"));
            Store store = Facade.CreateStore(usr, "ProductExistStore");
            Product prod1 = new Product(store, "Bamba", "Ahla shel hatif", "Snacks", 3.5, 100);
            Product prod2 = new Product(store, "Bisli", "Pahot ahla shel hatif", "Snacks", 3.7, 81);
            store.Products.Add(prod1);
            Assert.IsTrue(Facade.IsProductExists(prod1));
            Assert.IsFalse(Facade.IsProductExists(prod2));
            store.Products.Add(prod2);
            Assert.IsTrue(Facade.IsProductExists(prod2));
        }

        [Test]
        public void SearchProducts()
        {
            LoggedInUser usr1 = new LoggedInUser("SearchProduct1", SecurityAdapter.Encrypt("123456789"));
            Store store1 = Facade.CreateStore(usr1, "SearchProducts1");
            LoggedInUser usr2 = new LoggedInUser("SearchProduct2", SecurityAdapter.Encrypt("123456789"));
            Store store2 = Facade.CreateStore(usr2, "SearchProduct2");
            Product prod1 = new Product(store1, "Search1", "nini", "nana", 123, 7);
            Product prod2 = new Product(store1, "Search2", "wello", "wallak", 777, 1);
            store1.Products.Add(prod1);
            Assert.IsTrue(Facade.SearchProducts(prod => prod.Name.Equals("Search1")).FirstOrDefault() == prod1);
            Assert.IsTrue(Facade.SearchProducts(prod => prod.Name.Equals("Search2")).FirstOrDefault() == null);
            store1.Products.Add(prod2);
            Assert.IsTrue(Facade.SearchProducts(prod => prod.Name.Equals("Search2")).FirstOrDefault() == prod2);
            Product prod3 = new Product(store2, "Search3", "welcome", "nana", 999, 2);
            store2.Products.Add(prod3);
            Assert.IsTrue(Facade.SearchProducts(prod => prod.Name.Equals("Search3")).FirstOrDefault() == prod3);
            var result = Facade.SearchProducts(prod => prod.Price >= 777 && prod.Price <= 999);
            Assert.IsTrue(result.Contains(prod2) && result.Contains(prod3) && !result.Contains(prod1));
        }

        [Test]
        public void FilterProductsTest()
        {
            LoggedInUser usr1 = new LoggedInUser("Filter1", SecurityAdapter.Encrypt("12345677777"));
            LoggedInUser usr2 = new LoggedInUser("Filter2", SecurityAdapter.Encrypt("789999"));
            Store store1 = Facade.CreateStore(usr1, "Filter1Str");
            Store store2 = Facade.CreateStore(usr1, "Filter2Str");
            Store store3 = Facade.CreateStore(usr2, "Filter3Str");
            var prod1 = new Product(store1, "Filter1Prod", "long long long long description", "FilterCat", 11, 99999);
            var prod2 = new Product(store1, "Filter2Prod", "short", "FilterCat", 99.99, 1);
            var prod3 = new Product(store1, "Filter3Prod", "NaN", "FilterCat2", 50, 5);
            var prod4 = new Product(store2, "Filter4Prod", "Unique", "FilterCat2", 15, 7);
            var prod5 = new Product(store2, "Filter5Prod", "short", "FilterCat", 115.2, 11);
            var prod6 = new Product(store3, "Filter6Prod", "nininana", "FilterCat", 766, 16);
            store1.Products.Add(prod1);
            store1.Products.Add(prod2);
            store1.Products.Add(prod3);
            store2.Products.Add(prod4);
            store2.Products.Add(prod5);
            store3.Products.Add(prod6);
            var collection = new List<Product>() { prod1, prod2, prod3, prod4, prod5, prod6 };
            var result1 = Facade.FilterProducts(collection, prod => prod.Price > 11);
            Assert.IsTrue(!result1.Contains(prod1) && result1.Count == 5);
            var result2 = Facade.FilterProducts(result1, prod => prod.Description.Equals("short"));
            Assert.IsTrue(result2.Contains(prod2) && result2.Contains(prod5) && result2.Count == 2);
            var result3 = Facade.FilterProducts(result2, prod => prod.Store.Name.Equals("Filter1Str"));
            Assert.IsTrue(result3.Contains(prod2) && result3.Count == 1);
        }
    }
}
