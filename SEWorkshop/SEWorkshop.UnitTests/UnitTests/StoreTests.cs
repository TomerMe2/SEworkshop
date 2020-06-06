using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.DAL;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace SEWorkshop.Tests.UnitTests
{
    [TestFixture]
    public class StoreTests
    {
        SecurityAdapter _securityAdapter = new SecurityAdapter();
        const string CREDIT_CARD_NUMBER_STUB = "1234";
        const string CITY_NAME_STUB = "Beer Sheva";
        const string STREET_NAME_STUB = "Shderot Ben Gurion";
        const string HOUSE_NUMBER_STUB = "111";
        const string COUNTRY_STUB = "Israel";
        Address address = new Address(COUNTRY_STUB, CITY_NAME_STUB, STREET_NAME_STUB, HOUSE_NUMBER_STUB);

        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void Cleanup()
        {
            //DbContext.Database.Delete();
        }

        //Testing CloseStore()
        [Test]
        public void CloseStore_StoreIsOpen_StoreClosed()
        {
            LoggedInUser sp_user1 = new LoggedInUser("sp_user1", _securityAdapter.Encrypt("1111"));
            //DbContext.LoggedInUsers.Add(sp_user1);
            //DbContext.SaveChanges();
            Store closeStore1 = new Store(sp_user1, "sp_store1");
            //DbContext.Stores.Add(closeStore1);
            //DbContext.SaveChanges();
            closeStore1.CloseStore();
            Assert.That(closeStore1.IsOpen, Is.False);
        }

        //Testing SearchProducts(Func<Product, bool> pred)
        [Test]
        public void SearchProducts_ProductDoesNotExistInStore_ReturnEmptyList()
        {
            LoggedInUser sp_user1 = new LoggedInUser("sp_user1", _securityAdapter.Encrypt("1111"));
            //DbContext.LoggedInUsers.Add(sp_user1);
            //DbContext.SaveChanges();
            Store sp_store1 = new Store(sp_user1, "sp_store1");
            //DbContext.Stores.Add(sp_store1);
            //DbContext.SaveChanges();
            Owns ownership = new Owns(sp_user1, sp_store1, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            sp_store1.Ownership.Add(ownership);
            sp_user1.Owns.Add(ownership);
            //DbContext.AuthorityHandlers.Add(ownership);
            sp_user1.AddProduct(sp_store1,"sp_prod1", "ninini", "cat1", 11.11, 1);
            var result = sp_store1.SearchProducts(product => product.Name.Contains("2"));
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public void SearchProducts_ProductsExistsInStore_ReturnListOfProducts()
        {
            LoggedInUser sp_user2 = new LoggedInUser("sp_user1", _securityAdapter.Encrypt("1111"));
            Store sp_store2 = new Store(sp_user2, "sp_store2");
            Owns ownership = new Owns(sp_user2, sp_store2, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            sp_store2.Ownership.Add(ownership);
            sp_user2.Owns.Add(ownership);
            sp_user2.AddProduct(sp_store2,"sp_prod2", "ninini", "cat1", 11.11, 1);
            var result = sp_store2.SearchProducts(product => product.Name.Contains("2"));
            Assert.That(result, Is.Not.Empty);
        }

        //Testing PurchaseBasket(ICollection<(Product, int)> itemsList, string creditCardNumber, Address address)
        [Test]
        public void PurchaseBasket_NegativeInventory_ThrowException()
        {
            LoggedInUser pb_user1 = new LoggedInUser("pb_user1", _securityAdapter.Encrypt("1111"));
            Store pb_store1 = new Store(pb_user1, "pb_store2");
            Owns ownership = new Owns(pb_user1, pb_store1, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            pb_store1.Ownership.Add(ownership);
            pb_user1.Owns.Add(ownership);
       
            pb_user1.AddProduct(pb_store1,"pb_prod1", "ninini", "cat1", 11.111, 1);
            var pb_prod1 = pb_store1.SearchProducts(product => product.Name.Equals("pb_prod1")).ElementAt(0);
            Basket bskt = new Basket(pb_store1, pb_user1.Cart);
            bskt.Products.Add(new ProductsInBasket(bskt, pb_prod1, 2));
            try
            {
                pb_store1.PurchaseBasket(bskt, CREDIT_CARD_NUMBER_STUB, address, pb_user1);
                Assert.Fail();
            }
            catch (NegativeInventoryException)
            {
                Assert.True(true);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void PurchaseBasket_AllQuantitiesAreValid_PurchaseSuccessful()
        {
            LoggedInUser pb_user2 = new LoggedInUser("pb_user2", _securityAdapter.Encrypt("1111"));
            Store pb_store2 = new Store(pb_user2, "pb_store2");
            Owns ownership = new Owns(pb_user2, pb_store2, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            pb_store2.Ownership.Add(ownership);
            pb_user2.Owns.Add(ownership);
            pb_user2.AddProduct(pb_store2,"pb_prod2", "ninini", "cat1", 11.111, 1);
            var pb_prod2 = pb_store2.SearchProducts(product => product.Name.Equals("pb_prod2")).ElementAt(0);
            Basket bskt = new Basket(pb_store2, pb_user2.Cart);
            bskt.Products.Add(new ProductsInBasket(bskt, pb_prod2, 1));
            pb_store2.PurchaseBasket(bskt, CREDIT_CARD_NUMBER_STUB, address, pb_user2);
            Assert.That(pb_prod2.Quantity, Is.EqualTo(0));
        }
    }
}