using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;

namespace SEWorkshop.Tests
{
    [TestFixture]
    public class StoreTests
    {
        SecurityAdapter _securityAdapter = new SecurityAdapter();
        //public void CloseStore()
        [Test]
        public void CloseStore_StoreIsOpen_StoreClosed()
        {
            Store closeStore1 = new Store(new LoggedInUser("cs_user1", _securityAdapter.Encrypt("1111")), "closeStore1");
            closeStore1.CloseStore();
            Assert.That(closeStore1.IsOpen, Is.False);
        }

        [Test]
        public void SearchProducts_ProductDoesNotExistInStore_ReturnEmptyList()
        {
            LoggedInUser sp_user1 = new LoggedInUser("sp_user1", _securityAdapter.Encrypt("1111"));
            Store sp_store1 = new Store(sp_user1, "sp_store1");
            sp_user1.AddProduct(sp_store1,"sp_prod1", "ninini", "cat1", 11.11, 1);
            var result = sp_store1.SearchProducts(product => product.Name.Contains("2"));
            Assert.That(result, Is.Empty);
        }
        
        [Test]
        public void GetProduct_ProductsExistsInStore_ReturnListOfProducts()
        {
            LoggedInUser sp_user2 = new LoggedInUser("sp_user1", _securityAdapter.Encrypt("1111"));
            Store sp_store2 = new Store(sp_user2, "sp_store2");
            sp_user2.AddProduct(sp_store2,"sp_prod2", "ninini", "cat1", 11.11, 1);
            var result = sp_store2.SearchProducts(product => product.Name.Contains("2"));
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void PurchaseBasket_NegativeInventory_ThrowException()
        {
            LoggedInUser pb_user1 = new LoggedInUser("pb_user1", _securityAdapter.Encrypt("1111"));
            Store pb_store1 = new Store(pb_user1, "pb_store2");
       
            pb_user1.AddProduct(pb_store1,"pb_prod1", "ninini", "cat1", 11.111, 1);
            var pb_prod1 = pb_store1.SearchProducts(product => product.Name.Equals("pb_prod1")).ElementAt(0);
            try
            {
                pb_store1.PurchaseBasket(new List<(Product, int)>() {(pb_prod1, 2)});
                Assert.Fail();
            }
            catch (NegativeInventoryException e)
            {
                Assert.True(true);
            }
            catch (Exception e)
            {
                Assert.Fail();
            }
        }

        [Test]
        public void PurchaseBasket_AllQuantitiesAreValid_PurchaseSuccessful()
        {
            LoggedInUser pb_user2 = new LoggedInUser("pb_user2", _securityAdapter.Encrypt("1111"));
            Store pb_store2 = new Store(pb_user2, "pb_store2");
           // pb_user2.Manage.Add(new Manages(pb_user2, pb_store2));
            pb_user2.AddProduct(pb_store2,"pb_prod2", "ninini", "cat1", 11.111, 1);
            var pb_prod2 = pb_store2.SearchProducts(product => product.Name.Equals("pb_prod2")).ElementAt(0);
            pb_store2.PurchaseBasket(new List<(Product, int)>(){(pb_prod2, 1)});
            Assert.That(pb_prod2.Quantity, Is.EqualTo(0));
        }
    }
}