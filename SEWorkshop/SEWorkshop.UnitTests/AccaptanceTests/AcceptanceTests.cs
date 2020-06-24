using System;
using NUnit.Framework;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using SEWorkshop.DataModels;
using System.Linq;
using SEWorkshop.Tests.AccaptanceTests;
using SEWorkshop.DAL;

namespace SEWorkshop.Tests.AcceptanceTests
{
	[TestFixture]
	public class AcceptanceTests
	{
		private Bridge Bridge { get; set; }
		private const string DEF_SID = "1";
		private const string CVV = "512";
		private const string NAME = "Ben Zini";
		private const string ID = "1";
		private DateTime ExpirationDate = new DateTime(2021, 3, 1);

		[OneTimeSetUp]
		public void Init()
		{
			DatabaseProxy.MoveToTestDb();
			Bridge = new ProxyServiceLayer();
		}

		[Test, Order(1)]
		public void Test_2_2()
		{
			Assert.That(() => Bridge.Register(DEF_SID, "user", "1234"), Throws.Nothing);
			Assert.Throws<UserAlreadyExistsException>(delegate { Bridge.Register(DEF_SID, "user", "1234"); });
			Bridge.Register(DEF_SID, "secondOwner", "1234");
			Bridge.Register(DEF_SID, "notManager", "1234");
			Bridge.Register(DEF_SID, "managerFirstOwner", "1234");
			Bridge.Register(DEF_SID, "managerSecondOwner", "1234");
		}

		[Test, Order(2)]
		public void Test_2_3()
		{
			Assert.Throws<UserDoesNotExistException>(delegate { Bridge.Login(DEF_SID, "user3", "1234"); });
			Assert.Throws<UserDoesNotExistException>(delegate { Bridge.Login(DEF_SID, "user", "12345"); });
			Assert.Throws<UserDoesNotExistException>(delegate { Bridge.Login(DEF_SID, "user3", "12345"); });
			Assert.That(() => Bridge.Login(DEF_SID, "user", "1234"), Throws.Nothing);
			Assert.Throws<UserAlreadyLoggedInException>(delegate { Bridge.Login(DEF_SID, "user3", "12345"); });
			Assert.Throws<UserAlreadyLoggedInException>(delegate { Bridge.Register(DEF_SID, "user3", "12345"); });
		}

		[Test, Order(13)]
		public void Test_2_4()
		{
			Assert.That(() => Bridge.BrowseStores(), Throws.Nothing);
		}

		[Test, Order(7)]
		public void Test_2_5_1()
		{
			Bridge.AddProduct(DEF_SID, "store1", "bisli", "snack", "food", 5, 1);
			Bridge.AddProduct(DEF_SID, "store1", "doritos", "snack", "food", 6, 1);
			Bridge.AddProduct(DEF_SID, "store1", "tv", "television", "electronics", 850, 1);
			Bridge.AddProduct(DEF_SID, "store1", "flower", "smells nice", "plants", 25, 1);
			string tv = "tv";
			Assert.That(() => Bridge.SearchProductsByName(ref tv), Throws.Nothing);
			Assert.AreEqual(Bridge.SearchProductsByName(ref tv).Count(), 1);
		}

		[Test, Order(8)]
		public void Test_2_5_2()
		{
			string food = "food";
			Assert.That(() => Bridge.SearchProductsByCategory(ref food), Throws.Nothing);
			Assert.AreEqual(Bridge.SearchProductsByCategory(ref food).Count(), 2);
		}

		[Test, Order(9)]
		public void Test_2_5_3()
		{
			string bsli = "bsli";
			Assert.That(() => Bridge.SearchProductsByKeywords(ref bsli), Throws.Nothing);
			Assert.AreEqual(Bridge.SearchProductsByKeywords(ref bsli).Count(), 1);
		}

		[Test, Order(10)]
		public void Test_2_5_4()
		{
			string food = "food";
			string bisli = "bisli";
			var products = Bridge.SearchProductsByCategory(ref food).ToList();
			Bridge.SearchProductsByName(ref bisli);
			Assert.That(() => Bridge.SearchProductsByName(ref bisli), Throws.Nothing);
			Assert.AreEqual(Bridge.SearchProductsByName(ref bisli).Count(), 1);
		}

		[Test, Order(11)]
		public void Test_2_6()
		{
			Bridge.OpenStore(DEF_SID, "store2");
			Assert.Throws<ProductNotInTheStoreException>(delegate { Bridge.AddProductToCart(DEF_SID, "store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { Bridge.AddProductToCart(DEF_SID, "store1", "tv", -1); });
			Bridge.Logout(DEF_SID);
			Assert.Throws<ProductNotInTheStoreException>(delegate { Bridge.AddProductToCart(DEF_SID, "store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { Bridge.AddProductToCart(DEF_SID, "store1", "tv", -1); });

			Bridge.Login(DEF_SID, "user", "1234");
			Assert.That(() => Bridge.AddProductToCart(DEF_SID, "store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(Bridge.MyCart(DEF_SID).First().Products.Count() == 1);
			Assert.That(() => Bridge.AddProductToCart(DEF_SID, "store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(Bridge.MyCart(DEF_SID).First().Products.Count() == 2);
		}

		[Test, Order(12)]
		public void Test_2_7()
		{
			Assert.That(() => Bridge.MyCart(DEF_SID), Throws.Nothing);
			Assert.IsTrue(Bridge.MyCart(DEF_SID).First().Products.Count() == 2);
			Assert.Throws<ProductIsNotInCartException>(delegate { Bridge.RemoveProductFromCart(DEF_SID, "store1", "flower", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { Bridge.RemoveProductFromCart(DEF_SID, "store2", "bisli", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.RemoveProductFromCart(DEF_SID, "store3", "bisli", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { Bridge.RemoveProductFromCart(DEF_SID, "store1", "bisli", -1); });
			Assert.IsTrue(Bridge.MyCart(DEF_SID).First().Products.Count() == 2);
			Assert.That(() => Bridge.RemoveProductFromCart(DEF_SID, "store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(Bridge.MyCart(DEF_SID).First().Products.Count() == 1);
		}

		[Test, Order(14)]
		public void Test_2_8()
		{
			Bridge.AddProductToCart(DEF_SID, "store1", "bisli", 1);
			var basket = Bridge.MyCart(DEF_SID).First();
			Assert.That(() => Bridge.Purchase(DEF_SID, basket, "555", ExpirationDate, CVV,
					new Address("Israel", "Beer Sheva", "Ben Gurion", "99", "1234"), NAME, ID), Throws.Nothing);
			Assert.Throws<BasketNotInSystemException>(delegate {
				Bridge.Purchase(DEF_SID, basket, "555", ExpirationDate, CVV,
	   new Address("Israel", "Beer Sheva", "Ben Gurion", "99", "1234"), NAME, ID);
			});
		}

		[Test, Order(30)]
		public void Test_3_1()
		{
			Assert.That(() => Bridge.Logout(DEF_SID), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.Logout(DEF_SID); });
		}

		[Test, Order(3)]
		public void Test_3_2()
		{
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.OpenStore(DEF_SID, "store1"); });
			Bridge.Login(DEF_SID, "user", "1234");
			Assert.That(() => Bridge.OpenStore(DEF_SID, "store1"), Throws.Nothing);
			Assert.Throws<StoreWithThisNameAlreadyExistsException>(delegate { Bridge.OpenStore(DEF_SID, "store1"); });
		}

		[Test, Order(20)]
		public void Test_3_3()
		{
			Assert.Throws<ReviewIsEmptyException>(delegate { Bridge.WriteReview(DEF_SID, "store1", "bisli", ""); });
			Assert.That(() => Bridge.WriteReview(DEF_SID, "store1", "bisli", "product is good"), Throws.Nothing);
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.WriteReview(DEF_SID, "store1", "bisli", "product is bad"); });
		}

		[Test, Order(21)]
		public void Test_3_5()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.WriteMessage(DEF_SID, "store1", "store is bad"); });
			Bridge.Login(DEF_SID, "user", "1234");
			Assert.Throws<MessageIsEmptyException>(delegate { Bridge.WriteMessage(DEF_SID, "store1", ""); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.WriteMessage(DEF_SID, "store3", "store is ok"); });
			Assert.That(() => Bridge.WriteMessage(DEF_SID, "store1", "store is good"), Throws.Nothing);
		}

		[Test, Order(22)]
		public void Test_3_7()
		{
			Assert.That(() => Bridge.PurchaseHistory(DEF_SID), Throws.Nothing);
			Assert.NotNull(Bridge.PurchaseHistory(DEF_SID));
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.PurchaseHistory(DEF_SID); });
		}

		[Test, Order(4)]
		public void Test_4_1_1()
		{
			Assert.That(() => Bridge.AddProduct(DEF_SID, "store1", "bamba", "peanut snack", "food", 4.5, 1), Throws.Nothing);
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.AddProduct(DEF_SID, "store1", "bamba2", "peanut snack", "food", 4.5, 1); });
			Bridge.Login(DEF_SID, "secondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.AddProduct(DEF_SID, "store1", "bamba3", "peanut snack", "food", 4.5, 1); });
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(6)]
		public void Test_4_1_2()
		{
			string bamba2 = "bamba2";
			Assert.That(() => Bridge.RemoveProduct(DEF_SID, "store1", "bamba2"), Throws.Nothing);
			Assert.IsEmpty(Bridge.SearchProductsByName(ref bamba2));
		}

		[Test, Order(5)]
		public void Test_4_1_3()
		{
			string bamba = "bamba";
			string bamba2 = "bamba2";

			var bamb = Bridge.SearchProductsByName(ref bamba).First();
			Assert.That(() => Bridge.EditProductName(DEF_SID, "store1", "bamba", "bamba2"), Throws.Nothing);
			Assert.That(() => Bridge.EditProductCategory(DEF_SID, "store1", "bamba2", "electronics"), Throws.Nothing);
			Assert.That(() => Bridge.EditProductPrice(DEF_SID, "store1", "bamba2", 215), Throws.Nothing);
			Assert.That(() => Bridge.EditProductDescription(DEF_SID, "store1", "bamba2", "electric bamba"), Throws.Nothing);
			Assert.AreEqual(Bridge.SearchProductsByName(ref bamba2).Count(), 1);
			Assert.AreEqual(Bridge.SearchProductsByName(ref bamba).Count(), 0);
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.EditProductName(DEF_SID, "store1", "bamba2", "bamba3"); });
			Bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(33)]
		public void Test_4_2_1()
		{
			string username = "NoaKirel";
			string password = "1234";
			string storeName = "Waist Pouches";
			//bridge.Logout(DEF_SID);
			Bridge.Register(DEF_SID, username, password);
			Bridge.Login(DEF_SID, username, password);
			Bridge.OpenStore(DEF_SID, storeName);
			string productName = "pouch1";
			Bridge.AddProduct(DEF_SID, storeName, productName, "very cool", "Pouches for women", 50, 300);
			Assert.That(() => Bridge.AddSingleProductQuantityPolicy(DEF_SID, storeName, Enums.Operator.Or, productName, 5, 10), Throws.Nothing);

			Bridge.Logout(DEF_SID);
			Bridge.Register(DEF_SID, "user1", "password");
			Bridge.Login(DEF_SID, "user1", "password");
			Bridge.AddProductToCart(DEF_SID, storeName, productName, 7);
			IEnumerable<DataBasket> cart = Bridge.MyCart(DEF_SID);
			Address address = new Address("Israel", "Haifa", "Haim Nahman", "33", "1234");
			Assert.That(() => Bridge.Purchase(DEF_SID, cart.First(), "123456789", ExpirationDate, CVV, address, NAME, ID), Throws.Nothing);
			IEnumerable<DataBasket> cart4 = Bridge.MyCart(DEF_SID);  //degub: cart should be empty
			Bridge.AddProductToCart(DEF_SID, storeName, productName, 12);
			IEnumerable<DataBasket> cart5 = Bridge.MyCart(DEF_SID);  //degub: cart should be empty
			Assert.Throws<PolicyIsFalse>(delegate { Bridge.Purchase(DEF_SID, cart5.First(), "123456789", ExpirationDate, CVV, address, NAME, ID); });
			IEnumerable<DataBasket> cart3 = Bridge.MyCart(DEF_SID); //debug: check that cart is not empty

			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, username, password);
			Assert.That(() => Bridge.RemovePolicy(DEF_SID, storeName, 0), Throws.Nothing);

			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "user1", "password");
			IEnumerable<DataBasket> cart2 = Bridge.MyCart(DEF_SID);
			Assert.That(() => Bridge.Purchase(DEF_SID, cart2.First(), "123456789", ExpirationDate, CVV, address, NAME, ID), Throws.Nothing);
		}

		[Test, Order(34)]
		public void Test_4_2_2()
		{
			string username = "NoaKirel";
			string password = "1234";
			string storeName = "Waist Pouches";
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, username, password);
			string productName2 = "pouch2";
			Bridge.AddProduct(DEF_SID, storeName, productName2, "very cool", "Pouches for women", 50, 300);
			DateTime deadline = DateTime.Now.AddYears(1);
			Assert.That(() => Bridge.AddSpecificProductDiscount(DEF_SID, storeName, productName2, deadline, 20, Enums.Operator.And, -1, 0, true), Throws.Nothing);
			Assert.That(() => Bridge.AddProductCategoryDiscount(DEF_SID, storeName, "Pouches for women", deadline, 20, Enums.Operator.And, -1, 0, true), Throws.Nothing);

			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "user1", "password");
			Bridge.AddProductToCart(DEF_SID, storeName, productName2, 4);
			Assert.AreEqual(30 * 4, Bridge.MyCart(DEF_SID).First().PriceAfterDiscount);
		}

		[Test, Order(23)]
		public void Test_4_3()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.AddStoreOwner(DEF_SID, "store1", "secondOwner"); });
			Bridge.Login(DEF_SID, "user", "1234");
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.AddStoreOwner(DEF_SID, "store3", "secondOwner"); });
			Assert.That(() => Bridge.AddStoreOwner(DEF_SID, "store1", "secondOwner"), Throws.Nothing);
			Bridge.AddStoreManager(DEF_SID, "store1", "managerFirstOwner");
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "secondOwner", "1234");
			Assert.Throws<UserIsAlreadyStoreOwnerException>(delegate { Bridge.AddStoreOwner(DEF_SID, "store1", "user"); });
		}

		[Test, Order(24)]
		public void Test_4_5()
		{
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.AddStoreManager(DEF_SID, "store3", "managerSecondOwner"); });
			Assert.That(() => Bridge.AddStoreManager(DEF_SID, "store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserIsAlreadyStoreManagerException>(delegate { Bridge.AddStoreManager(DEF_SID, "store1", "managerFirstOwner"); });
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.AddStoreManager(DEF_SID, "store1", "notManager"); });
			Bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(25)]
		public void Test_4_6()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.SetPermissionsOfManager(DEF_SID, "store1", "managerSecondOwner", "Products"); });
			Assert.That(() => Bridge.SetPermissionsOfManager(DEF_SID, "store1", "managerFirstOwner", "Watching"), Throws.Nothing);
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "secondOwner", "1234");
			Assert.That(() => Bridge.SetPermissionsOfManager(DEF_SID, "store1", "managerSecondOwner", "Products"), Throws.Nothing);
		}

		[Test, Order(29)]
		public void Test_4_7()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.RemoveStoreManager(DEF_SID, "store1", "managerSecondOwner"); });
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "secondOwner", "1234");
			//Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager("store2", "user"); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.RemoveStoreManager(DEF_SID, "store3", "managerFirstOwner"); });
			Assert.That(() => Bridge.RemoveStoreManager(DEF_SID, "store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.RemoveStoreManager(DEF_SID, "store1", "managerFirstOwner"); });
			Assert.Throws<UserIsNotMangerOfTheStoreException>(delegate { Bridge.RemoveStoreManager(DEF_SID, "store1", "notManager"); });
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.RemoveStoreManager(DEF_SID, "store1", "managerFirstOwner"); });
			Bridge.Login(DEF_SID, "user", "1234");
		}

		[Test, Order(26)]
		public void Test_4_9()
		{
			Assert.That(() => Bridge.ViewMessage(DEF_SID, "store1"), Throws.Nothing);
			var m = Bridge.ViewMessage(DEF_SID, "store1").Last();
			Assert.That(() => Bridge.MessageReply(DEF_SID, m, "store1", "thank you"), Throws.Nothing);
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "managerSecondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.MessageReply(DEF_SID, m, "store1", "not thank you"); });
		}

		[Test, Order(28)]
		public void Test_4_10()
		{
			Assert.That(() => Bridge.ManagingPurchaseHistory(DEF_SID, "store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.ManagingPurchaseHistory(DEF_SID, "store3"); });
			Bridge.Logout(DEF_SID);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.ManagingPurchaseHistory(DEF_SID, "store1"); });
			Bridge.Login(DEF_SID, "user", "1234");
			Assert.That(() => Bridge.ManagingPurchaseHistory(DEF_SID, "store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { Bridge.ManagingPurchaseHistory(DEF_SID, "store3"); });
		}

		[Test, Order(27)]
		public void Test_5_1()
		{
			var m = Bridge.ViewMessage(DEF_SID, "store1").Last();
			Assert.That(() => Bridge.AddProduct(DEF_SID, "store1", "chair", "with wheels", "furniture", 350, 1), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.AddStoreManager(DEF_SID, "store1", "notManager"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.MessageReply(DEF_SID, m, "store1", "not thank you"); });
			Assert.That(() => Bridge.ViewMessage(DEF_SID, "store1"), Throws.Nothing);
		}

		[Test, Order(31)]
		public void Test_6_4()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.StorePurchaseHistory(DEF_SID, "store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.UserPurchaseHistory(DEF_SID, "user"); });
			Bridge.Login(DEF_SID, "notManager", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.StorePurchaseHistory(DEF_SID, "store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { Bridge.UserPurchaseHistory(DEF_SID, "user"); });
			Bridge.Logout(DEF_SID);
			Bridge.Login(DEF_SID, "A1", "sadnaTeam");
			Assert.That(() => Bridge.StorePurchaseHistory(DEF_SID, "store1"), Throws.Nothing);
			Assert.That(() => Bridge.UserPurchaseHistory(DEF_SID, "user"), Throws.Nothing);
		}

		[Test, Order(32)]
		public void Test_10()
		{
			const string SECOND_SID = "Test_10_Sid";
			var obsrv = new PurchaseObserverMock();
			Bridge.RegisterPurchaseObserver(obsrv);
			Bridge.Logout(DEF_SID);
			Bridge.Register(DEF_SID, "test_10_usr", "1234");
			Bridge.Login(DEF_SID, "test_10_usr", "1234");
			Bridge.OpenStore(DEF_SID, "test_10_str");
			Bridge.AddProduct(DEF_SID, "test_10_str", "some_prod", "ninini", "some_cat", 999, 50);
			Bridge.Register(SECOND_SID, "test_10_user2", "1234");
			Bridge.Login(SECOND_SID, "test_10_user2", "1234");
			Bridge.AddProductToCart(SECOND_SID, "test_10_str", "some_prod", 1);
			var basket = Bridge.MyCart(SECOND_SID).First(bskt => bskt.Store.Name.Equals("test_10_str"));
			var adrs = new Address("Israel", "Beer Sheva", "Ben Gurion", "14", "1234");
			Bridge.Purchase(SECOND_SID, basket, "1234", ExpirationDate, CVV, adrs, NAME, ID);

			Assert.IsTrue(obsrv.Purchases.Count == 1);
			var prchs = obsrv.Purchases[0];
			Assert.IsTrue(prchs.Address.Equals(adrs));
			Assert.IsTrue(prchs.Basket.Equals(basket));
			Bridge.Logout(DEF_SID);
		}
	}
}