using System;
using NUnit.Framework;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.Tests.AccaptanceTests;
using SEWorkshop.DataModels;

namespace SEWorkshop.Tests.AcceptanceTests
{
	[TestFixture]
	public class AcceptanceTests
	{
		private Bridge bridge = new ProxyServiceLayer();
        private const string DEF_SID = "1";

		[Test, Order(1)]
		public void Test_2_2()
        {
			Assert.That(() => bridge.Register(DEF_SID, "user", "1234"), Throws.Nothing);
			Assert.Throws<UserAlreadyExistsException>(delegate { bridge.Register(DEF_SID, "user", "1234"); });
			bridge.Register(DEF_SID, "secondOwner", "1234");
			bridge.Register(DEF_SID, "notManager", "1234");
			bridge.Register(DEF_SID, "managerFirstOwner", "1234");
			bridge.Register(DEF_SID, "managerSecondOwner", "1234");
		}

		[Test, Order(2)]
		public void Test_2_3()
		{
			Assert.Throws<UserDoesNotExistException>(delegate { bridge.Login(DEF_SID, "user3", "1234"); });
			Assert.Throws<UserDoesNotExistException>(delegate { bridge.Login(DEF_SID, "user", "12345"); });
			Assert.Throws<UserDoesNotExistException>(delegate { bridge.Login(DEF_SID, "user3", "12345"); });
			Assert.That(() => bridge.Login(DEF_SID, "user", "1234"), Throws.Nothing);
			Assert.Throws<UserAlreadyLoggedInException>(delegate { bridge.Login(DEF_SID, "user3", "12345"); });
			Assert.Throws<UserAlreadyLoggedInException>(delegate { bridge.Register(DEF_SID, "user3", "12345"); });
		}

		[Test, Order(13)]
		public void Test_2_4()
		{
			Assert.That(() => bridge.BrowseStores(), Throws.Nothing);
		}

		[Test, Order(7)]
		public void Test_2_5_1()
		{
			bridge.AddProduct(DEF_SID, "store1", "bisli", "snack", "food", 5, 1);
			bridge.AddProduct(DEF_SID, "store1", "doritos", "snack", "food", 6, 1);
			bridge.AddProduct(DEF_SID, "store1", "tv", "television", "electronics", 850, 1);
			bridge.AddProduct(DEF_SID, "store1", "flower", "smells nice", "plants", 25, 1);
			string tv = "tv";
			Assert.That(() => bridge.SearchProductsByName(ref tv), Throws.Nothing);
			Assert.AreEqual(bridge.SearchProductsByName(ref tv).Count(), 1);
		}

		[Test, Order(8)]
		public void Test_2_5_2()
		{
			string food = "food";
			Assert.That(() => bridge.SearchProductsByCategory(ref food), Throws.Nothing);
			Assert.AreEqual(bridge.SearchProductsByCategory(ref food).Count(), 2);
		}

		[Test, Order(9)]
		public void Test_2_5_3()
		{
			string bsli = "bsli";
			Assert.That(() => bridge.SearchProductsByKeywords(ref bsli), Throws.Nothing);
			Assert.AreEqual(bridge.SearchProductsByKeywords(ref bsli).Count(), 1);
		}

		[Test, Order(10)]
		public void Test_2_5_4()
		{
			string food = "food";
			string bisli = "bisli";
			var products = bridge.SearchProductsByCategory(ref food).ToList();
			bridge.SearchProductsByName(ref bisli);
			Assert.That(() => bridge.SearchProductsByName(ref bisli), Throws.Nothing);
			Assert.AreEqual(bridge.SearchProductsByName(ref bisli).Count(), 1);
		}

		[Test, Order(11)]
		public void Test_2_6()
		{
			bridge.OpenStore(DEF_SID, "store2");
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.AddProductToCart(DEF_SID, "store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.AddProductToCart(DEF_SID, "store1", "tv", -1); });
			bridge.Logout(DEF_SID);
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.AddProductToCart(DEF_SID, "store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.AddProductToCart(DEF_SID, "store1", "tv", -1); });
			
			Assert.That(() => bridge.AddProductToCart(DEF_SID, "store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 1);
			bridge.Login(DEF_SID, "user", "1234");
			Assert.That(() => bridge.AddProductToCart(DEF_SID, "store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 2);
		}

		[Test, Order(12)]
		public void Test_2_7()
		{
			Assert.That(() => bridge.MyCart(DEF_SID), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 2);
			Assert.Throws<ProductIsNotInCartException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store1", "flower", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store2", "bisli", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store3", "bisli", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store1", "bisli", -1); });
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 2);
			Assert.That(() => bridge.RemoveProductFromCart(DEF_SID, "store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 1);
			bridge.Logout(DEF_SID);
			Assert.That(() => bridge.MyCart(DEF_SID), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart(DEF_SID).Count() == 0);
			bridge.AddProductToCart(DEF_SID, "store1", "tv", 1);
			Assert.Throws<ProductIsNotInCartException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store1", "doritos", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store2", "tv", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store3", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.RemoveProductFromCart(DEF_SID, "store1", "tv", -1); });
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 1);
			Assert.That(() => bridge.RemoveProductFromCart(DEF_SID, "store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart(DEF_SID).First().Products.Count() == 0);
			bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(14)]
		public void Test_2_8()
		{
			bridge.AddProductToCart(DEF_SID, "store1", "bisli", 1);
			var basket = bridge.MyCart(DEF_SID).First();
			Assert.That(() => bridge.Purchase(DEF_SID, basket, "555", new Address("Israel", "Beer Sheva", "Ben Gurion", "99")), Throws.Nothing);
			Assert.Throws<BasketNotInSystemException>(delegate { bridge.Purchase(DEF_SID, basket, "555",
                                                        new Address("Israel", "Beer Sheva", "Ben Gurion", "99")); });
		}

		[Test, Order(30)]
		public void Test_3_1()
		{	
			Assert.That(() => bridge.Logout(DEF_SID), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.Logout(DEF_SID); });
		}

		[Test, Order(3)]
		public void Test_3_2()
		{
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.OpenStore(DEF_SID, "store1"); });
			bridge.Login(DEF_SID, "user", "1234");
			Assert.That(() => bridge.OpenStore(DEF_SID, "store1"), Throws.Nothing);
			Assert.Throws<StoreWithThisNameAlreadyExistsException>(delegate { bridge.OpenStore(DEF_SID, "store1"); });
		}

		[Test, Order(20)]
		public void Test_3_3()
		{
			Assert.Throws<ReviewIsEmptyException>(delegate { bridge.WriteReview(DEF_SID, "store1", "bisli", ""); });
			Assert.That(() => bridge.WriteReview(DEF_SID, "store1", "bisli", "product is good"), Throws.Nothing);	
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.WriteReview(DEF_SID, "store1", "bisli", "product is bad"); });
		}

		[Test, Order(21)]
		public void Test_3_5()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.WriteMessage(DEF_SID, "store1", "store is bad"); });
			bridge.Login(DEF_SID, "user", "1234");
			Assert.Throws<MessageIsEmptyException>(delegate { bridge.WriteMessage(DEF_SID, "store1", ""); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.WriteMessage(DEF_SID, "store3", "store is ok"); });
			Assert.That(() => bridge.WriteMessage(DEF_SID, "store1", "store is good"), Throws.Nothing);
		}

		[Test, Order(22)]
		public void Test_3_7()
		{
			Assert.That(() => bridge.PurchaseHistory(DEF_SID), Throws.Nothing);
			Assert.NotNull(bridge.PurchaseHistory(DEF_SID));
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.PurchaseHistory(DEF_SID); });
		}

		[Test, Order(4)]
		public void Test_4_1_1()
		{
			Assert.That(() => bridge.AddProduct(DEF_SID, "store1", "bamba", "peanut snack", "food", 4.5, 1), Throws.Nothing);
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddProduct(DEF_SID, "store1", "bamba2", "peanut snack", "food", 4.5, 1); });
			bridge.Login(DEF_SID, "secondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddProduct(DEF_SID, "store1", "bamba3", "peanut snack", "food", 4.5, 1); });
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(6)]
		public void Test_4_1_2()
		{
			string bamba2 = "bamba2";
			Assert.That(() => bridge.RemoveProduct(DEF_SID, "store1", "bamba2"), Throws.Nothing);
			Assert.IsEmpty(bridge.SearchProductsByName(ref bamba2));
		}

		[Test, Order(5)]
		public void Test_4_1_3()
		{
			string bamba = "bamba";
			string bamba2 = "bamba2";
			var bamb = bridge.SearchProductsByName(ref bamba).First();
			Assert.That(() => bridge.EditProductName(DEF_SID, "store1", "bamba", "bamba2"), Throws.Nothing);
			Assert.That(() => bridge.EditProductCategory(DEF_SID, "store1", "bamba2", "electronics"), Throws.Nothing);
			Assert.That(() => bridge.EditProductPrice(DEF_SID, "store1", "bamba2", 215), Throws.Nothing);
			Assert.That(() => bridge.EditProductDescription(DEF_SID, "store1", "bamba2", "electric bamba"), Throws.Nothing);
			Assert.AreEqual(bridge.SearchProductsByName(ref bamba2).Count(), 1);
			Assert.AreEqual(bridge.SearchProductsByName(ref bamba).Count(), 0);
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.EditProductName(DEF_SID, "store1", "bamba2", "bamba3"); });
			bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(33)]
		public void Test_4_2_1()
		{
			string username = "Noa Kirel";
			string password = "1234";
			string storeName = "Waist Pouches";
			bridge.Logout(DEF_SID);
			bridge.Register(DEF_SID, username, password);
			bridge.Login(DEF_SID, username, password);
			bridge.OpenStore(DEF_SID, storeName);
			string productName = "pouch1";
			bridge.AddProduct(DEF_SID, storeName, productName, "very cool", "Pouches for women", 50, 300);
			Assert.That(() => bridge.AddSingleProductQuantityPolicy(DEF_SID, storeName, Enums.Operator.Or, productName, 5, 10), Throws.Nothing);

			bridge.Logout(DEF_SID);
			bridge.Register(DEF_SID, "user1", "password");
			bridge.Login(DEF_SID, "user1", "password");
			bridge.AddProductToCart(DEF_SID, storeName, productName, 7);
			IEnumerable<DataBasket> cart = bridge.MyCart(DEF_SID);
			Address address = new Address("Israel", "Haifa", "Haim Nahman", "33");
			Assert.That(() => bridge.Purchase(DEF_SID, cart.First(), "123456789", address), Throws.Nothing);
			bridge.AddProductToCart(DEF_SID, storeName, productName, 12);
			Assert.Throws<PolicyIsFalse>(delegate { bridge.Purchase(DEF_SID, cart.First(), "123456789", address); });

			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, username, password);
			Assert.That(() => bridge.RemovePolicy(DEF_SID, storeName, 0), Throws.Nothing);

			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "user1", "password");
			IEnumerable<DataBasket> cart2 = bridge.MyCart(DEF_SID);
			Assert.That(() => bridge.Purchase(DEF_SID, cart2.First(), "123456789", address), Throws.Nothing);
		}

		[Test, Order(34)]
		public void Test_4_2_2()
		{
			string username = "Noa Kirel";
			string password = "1234";
			string storeName = "Waist Pouches";
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, username, password);
			string productName2 = "pouch2";
			bridge.AddProduct(DEF_SID, storeName, productName2, "very cool", "Pouches for women", 50, 300);
			DateTime deadline = DateTime.Now.AddYears(1);
			Assert.That(() => bridge.AddSpecificProductDiscount(DEF_SID, storeName, productName2, deadline, 20, Enums.Operator.And, 0), Throws.Nothing);
			Assert.That(() => bridge.AddProductCategoryDiscount(DEF_SID, storeName, "Pouches for women", deadline, 20, Enums.Operator.And, 1), Throws.Nothing);

			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "user1", "password");
			bridge.AddProductToCart(DEF_SID, storeName, productName2, 4);
			Assert.AreEqual(30*4, bridge.MyCart(DEF_SID).First().PriceAfterDiscount);
		}

		[Test, Order(23)]
		public void Test_4_3()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddStoreOwner(DEF_SID, "store1", "secondOwner"); });
			bridge.Login(DEF_SID, "user", "1234");
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.AddStoreOwner(DEF_SID, "store3", "secondOwner"); });
			Assert.That(() => bridge.AddStoreOwner(DEF_SID, "store1", "secondOwner"), Throws.Nothing);
			bridge.AddStoreManager(DEF_SID, "store1", "managerFirstOwner");
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "secondOwner", "1234");
			Assert.Throws<UserIsAlreadyStoreOwnerException>(delegate { bridge.AddStoreOwner(DEF_SID, "store1", "user"); });
		}

		[Test, Order(24)]
		public void Test_4_5()
		{
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.AddStoreManager(DEF_SID, "store3", "managerSecondOwner"); });
			Assert.That(() => bridge.AddStoreManager(DEF_SID, "store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserIsAlreadyStoreManagerException>(delegate { bridge.AddStoreManager(DEF_SID, "store1", "managerFirstOwner"); });
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddStoreManager(DEF_SID, "store1", "notManager"); });
			bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(25)]
		public void Test_4_6()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.SetPermissionsOfManager(DEF_SID, "store1", "managerSecondOwner", "Products"); });
			Assert.That(() => bridge.SetPermissionsOfManager(DEF_SID, "store1", "managerFirstOwner", "Watching"), Throws.Nothing);
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "secondOwner", "1234");
			Assert.That(() => bridge.SetPermissionsOfManager(DEF_SID, "store1", "managerSecondOwner", "Products"), Throws.Nothing);
		}

		[Test, Order(29)]
		public void Test_4_7()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager(DEF_SID, "store1", "managerSecondOwner"); });
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "secondOwner", "1234");
			//Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager("store2", "user"); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.RemoveStoreManager(DEF_SID, "store3", "managerFirstOwner"); });
			Assert.That(() => bridge.RemoveStoreManager(DEF_SID, "store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager(DEF_SID, "store1", "managerFirstOwner"); });
			Assert.Throws<UserIsNotMangerOfTheStoreException>(delegate { bridge.RemoveStoreManager(DEF_SID, "store1", "notManager"); });
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager(DEF_SID, "store1", "managerFirstOwner"); });
			bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(26)]
		public void Test_4_9()
		{
			Assert.That(() => bridge.ViewMessage(DEF_SID, "store1"), Throws.Nothing);
			var m = bridge.ViewMessage(DEF_SID, "store1").Last();
			Assert.That(() => bridge.MessageReply(DEF_SID, m, "store1", "thank you"), Throws.Nothing);
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "managerSecondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.MessageReply(DEF_SID, m, "store1", "not thank you"); });
		}

		[Test, Order(28)]
		public void Test_4_10()
		{
			Assert.That(() => bridge.ManagingPurchaseHistory(DEF_SID, "store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.ManagingPurchaseHistory(DEF_SID, "store3"); });
			bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.ManagingPurchaseHistory(DEF_SID, "store1"); });
			bridge.Login(DEF_SID, "user", "1234");
			Assert.That(() => bridge.ManagingPurchaseHistory(DEF_SID, "store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.ManagingPurchaseHistory(DEF_SID, "store3"); });
		}

		[Test, Order(27)]
		public void Test_5_1()
		{
			var m = bridge.ViewMessage(DEF_SID, "store1").Last();
			Assert.That(() => bridge.AddProduct(DEF_SID, "store1", "chair", "with wheels", "furniture", 350, 1), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddStoreManager(DEF_SID, "store1", "notManager"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.MessageReply(DEF_SID, m, "store1", "not thank you"); });
			Assert.That(() => bridge.ViewMessage(DEF_SID, "store1"), Throws.Nothing);
		}

		[Test, Order(31)]
		public void Test_6_4()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.StorePurchaseHistory(DEF_SID, "store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.UserPurchaseHistory(DEF_SID, "user"); });
			bridge.Login(DEF_SID, "notManager", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.StorePurchaseHistory(DEF_SID, "store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.UserPurchaseHistory(DEF_SID, "user"); });
			bridge.Logout(DEF_SID);
			bridge.Login(DEF_SID, "admin", "sadnaTeam");
			Assert.That(() => bridge.StorePurchaseHistory(DEF_SID, "store1"), Throws.Nothing);
			Assert.That(() => bridge.UserPurchaseHistory(DEF_SID, "user"), Throws.Nothing);
		}

        [Test, Order(32)]
        public void Test_10()
        {
            const string SECOND_SID = "Test_10_Sid";
            var obsrv = new PurchaseObserverMock();
            bridge.RegisterPurchaseObserver(obsrv);
            bridge.Logout(DEF_SID);
            bridge.Register(DEF_SID, "test_10_usr", "1234");
            bridge.Login(DEF_SID, "test_10_usr", "1234");
            bridge.OpenStore(DEF_SID, "test_10_str");
            bridge.AddProduct(DEF_SID, "test_10_str", "some_prod", "ninini", "some_cat", 999, 50);
            bridge.Register(SECOND_SID, "test_10_user2", "1234");
            bridge.Login(SECOND_SID, "test_10_user2", "1234");
            bridge.AddProductToCart(SECOND_SID, "test_10_str", "some_prod", 1);
            var basket = bridge.MyCart(SECOND_SID).First(bskt => bskt.Store.Name.Equals("test_10_str"));
            var adrs = new Address("Israel", "Beer Sheva", "Ben Gurion", "14");
            bridge.Purchase(SECOND_SID, basket, "1234", adrs);

            Assert.IsTrue(obsrv.Purchases.Count == 1);
            var prchs = obsrv.Purchases[0];
            Assert.IsTrue(prchs.Address.Equals(adrs));
            Assert.IsTrue(prchs.Basket.Equals(basket));
        }
	}
}