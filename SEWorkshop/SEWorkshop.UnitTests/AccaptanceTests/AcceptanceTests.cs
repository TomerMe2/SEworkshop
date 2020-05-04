using System;
using NUnit.Framework;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using System.Collections.Generic;
using System.Linq;
using SEWorkshop.Tests.AccaptanceTests;

namespace SEWorkshop.UnitTests
{
	[TestFixture]
	public class AcceptanceTests
	{
		private UserManager um = new UserManager();
		private Bridge bridge = new ProxyServiceLayer();

		[Test, Order(1)]
		public void Test_2_2()
        {
			Assert.That(() => bridge.Register("user", "1234"), Throws.Nothing);
			Assert.Throws<UserAlreadyExistsException>(delegate { bridge.Register("user", "1234"); });
			Assert.That(() => bridge.Register("secondOwner", "1234"), Throws.Nothing);
			Assert.That(() => bridge.Register("notManager", "1234"), Throws.Nothing);
			Assert.That(() => bridge.Register("managerFirstOwner", "1234"), Throws.Nothing);
			Assert.That(() => bridge.Register("managerSecondOwner", "1234"), Throws.Nothing);
		}

		[Test, Order(2)]
		public void Test_2_3()
		{
			Assert.Throws<UserDoesNotExistException>(delegate { bridge.Login("user3", "1234"); });
			Assert.That(() => bridge.Login("user", "1234"), Throws.Nothing);
		}

		[Test, Order(13)]
		public void Test_2_4()
		{
			Assert.That(() => bridge.BrowseStores(), Throws.Nothing);
		}


		[Test, Order(7)]
		public void Test_2_5_1()
		{
			bridge.AddProduct("store1", "bisli", "snack", "food", 5, 1);
			bridge.AddProduct("store1", "doritos", "snack", "food", 6, 1);
			bridge.AddProduct("store1", "tv", "television", "electronics", 850, 1);
			bridge.AddProduct("store1", "flower", "smells nice", "plants", 25, 1);
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
			ICollection<Product> products = (ICollection<Product>)bridge.SearchProductsByCategory(ref food);
			Assert.That(() => bridge.FilterProducts(products, (x) => x.Name.Equals("bisli")), Throws.Nothing);
			Assert.AreEqual((bridge.FilterProducts(products, (x) => x.Name.Equals("bisli"))).Count(), 1);
		}

		[Test, Order(11)]
		public void Test_2_6()
		{
			bridge.OpenStore("store2");
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.AddProductToCart("store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.AddProductToCart("store1", "tv", -1); });
			bridge.Logout();
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.AddProductToCart("store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.AddProductToCart("store1", "tv", -1); });
			
			Assert.That(() => bridge.AddProductToCart("store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 1);
			bridge.Login("user", "1234");
			Assert.That(() => bridge.AddProductToCart("store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 2);

		}

		[Test, Order(12)]
		public void Test_2_7()
		{
			Assert.That(() => bridge.MyCart(), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 2);
			Assert.Throws<ProductIsNotInCartException>(delegate { bridge.RemoveProductFromCart("store1", "flower", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.RemoveProductFromCart("store2", "bisli", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.RemoveProductFromCart("store3", "bisli", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.RemoveProductFromCart("store1", "bisli", -1); });
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 2);
			Assert.That(() => bridge.RemoveProductFromCart("store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 1);
			bridge.Logout();
			Assert.That(() => bridge.MyCart(), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart().Count() == 0);
			bridge.AddProductToCart("store1", "tv", 1);
			Assert.Throws<ProductIsNotInCartException>(delegate { bridge.RemoveProductFromCart("store1", "doritos", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { bridge.RemoveProductFromCart("store2", "tv", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.RemoveProductFromCart("store3", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { bridge.RemoveProductFromCart("store1", "tv", -1); });
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 1);
			Assert.That(() => bridge.RemoveProductFromCart("store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(bridge.MyCart().First().Products.Count() == 0);
			bridge.Login("user", "1234");
		}

		[Test, Order(14)]
		public void Test_2_8()
		{
			bridge.AddProductToCart("store1", "bisli", 1);
			Basket basket = bridge.MyCart().First();
			Assert.That(() => bridge.Purchase(basket), Throws.Nothing);
			basket.Products.Clear();
			Assert.Throws<BasketIsEmptyException>(delegate { bridge.Purchase(basket); });
		}

		[Test, Order(30)]
		public void Test_3_1()
		{	
			Assert.That(() => bridge.Logout(), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.Logout(); });
		}

		[Test, Order(3)]
		public void Test_3_2()
		{
			Assert.That(() => bridge.OpenStore("store1"), Throws.Nothing);
			Assert.Throws<StoreWithThisNameAlreadyExistsException>(delegate { bridge.OpenStore("store1"); });
		}

		[Test, Order(20)]
		public void Test_3_3()
		{
			Assert.Throws<ReviewIsEmptyException>(delegate { bridge.WriteReview("store1", "bisli", ""); });
			Assert.That(() => bridge.WriteReview("store1", "bisli", "product is good"), Throws.Nothing);	
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.WriteReview("store1", "bisli", "product is bad"); });
		}

		[Test, Order(21)]
		public void Test_3_5()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.WriteMessage("store1", "store is bad"); });
			bridge.Login("user", "1234");
			Assert.Throws<MessageIsEmptyException>(delegate { bridge.WriteMessage("store1", ""); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.WriteMessage("store3", "store is ok"); });
			Assert.That(() => bridge.WriteMessage("store1", "store is good"), Throws.Nothing);
		}

		[Test, Order(22)]
		public void Test_3_7()
		{
			Assert.That(() => bridge.PurcahseHistory(), Throws.Nothing);
			Assert.NotNull(bridge.PurcahseHistory());
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.PurcahseHistory(); });
		}

		[Test, Order(4)]
		public void Test_4_1_1()
		{
			Assert.That(() => bridge.AddProduct("store1", "bamba", "peanut snack", "food", 4.5, 1), Throws.Nothing);
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddProduct("store1", "bamba2", "peanut snack", "food", 4.5, 1); });
			bridge.Login("secondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddProduct("store1", "bamba3", "peanut snack", "food", 4.5, 1); });
			bridge.Logout();
			bridge.Login("user", "1234");

		}

		[Test, Order(6)]
		public void Test_4_1_2()
		{
			string bamba2 = "bamba2";
			Assert.That(() => bridge.RemoveProduct("store1", "bamba2"), Throws.Nothing);
			Assert.IsEmpty(bridge.SearchProductsByName(ref bamba2));
		}

		[Test, Order(5)]
		public void Test_4_1_3()
		{
			string bamba = "bamba";
			string bamba2 = "bamba2";
			Product bamb = bridge.SearchProductsByName(ref bamba).First();
			Assert.That(() => bridge.EditProductName("store1", "bamba", "bamba2"), Throws.Nothing);
			Assert.That(() => bridge.EditProductCategory("store1", "bamba2", "electronics"), Throws.Nothing);
			Assert.That(() => bridge.EditProductPrice("store1", "bamba2", 215), Throws.Nothing);
			Assert.That(() => bridge.EditProductDescription("store1", "bamba2", "electric bamba"), Throws.Nothing);
			Assert.AreEqual(bridge.SearchProductsByName(ref bamba2).Count(), 1);
			Assert.AreEqual(bridge.SearchProductsByName(ref bamba).Count(), 0);
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.EditProductName("store1", "bamba2", "bamba3"); });
			bridge.Login("user", "1234");
		}

		[Test, Order(23)]
		public void Test_4_3()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddStoreOwner("store1", "secondOwner"); });
			bridge.Login("user", "1234");
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.AddStoreOwner("store3", "secondOwner"); });
			Assert.That(() => bridge.AddStoreOwner("store1", "secondOwner"), Throws.Nothing);
			bridge.AddStoreManager("store1", "managerFirstOwner");
			bridge.Logout();
			bridge.Login("secondOwner", "1234");
			Assert.Throws<UserIsAlreadyStoreOwnerException>(delegate { bridge.AddStoreOwner("store1", "user"); });
		}

		[Test, Order(24)]
		public void Test_4_5()
		{
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.AddStoreManager("store3", "managerSecondOwner"); });
			Assert.That(() => bridge.AddStoreManager("store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserIsAlreadyStoreManagerException>(delegate { bridge.AddStoreManager("store1", "managerFirstOwner"); });
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddStoreManager("store1", "notManager"); });
			bridge.Login("user", "1234");
		}

		[Test, Order(25)]
		public void Test_4_6()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.SetPermissionsOfManager("store1", "managerSecondOwner", "Products"); });
			Assert.That(() => bridge.SetPermissionsOfManager("store1", "managerFirstOwner", "Watching"), Throws.Nothing);
			bridge.Logout();
			bridge.Login("secondOwner", "1234");
			Assert.That(() => bridge.SetPermissionsOfManager("store1", "managerSecondOwner", "Products"), Throws.Nothing);
		}

		[Test, Order(29)]
		public void Test_4_7()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager("store1", "managerSecondOwner"); });
			bridge.Logout();
			bridge.Login("secondOwner", "1234");
			//Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager("store2", "user"); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.RemoveStoreManager("store3", "managerFirstOwner"); });
			Assert.That(() => bridge.RemoveStoreManager("store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager("store1", "managerFirstOwner"); });
			Assert.Throws<UserIsNotMangerOfTheStoreException>(delegate { bridge.RemoveStoreManager("store1", "notManager"); });
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.RemoveStoreManager("store1", "managerFirstOwner"); });
			bridge.Login("user", "1234");
		}

		[Test, Order(26)]
		public void Test_4_9()
		{
			Assert.That(() => bridge.ViewMessage("store1"), Throws.Nothing);
			Message m = bridge.ViewMessage("store1").Last();
			Assert.That(() => bridge.MessageReply(m, "store1", "thank you"), Throws.Nothing);
			bridge.Logout();
			bridge.Login("managerSecondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.MessageReply(m, "store1", "not thank you"); });
		}

		[Test, Order(28)]
		public void Test_4_10()
		{
			Assert.That(() => bridge.ManagingPurchaseHistory("store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.ManagingPurchaseHistory("store3"); });
			bridge.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.ManagingPurchaseHistory("store1"); });
			bridge.Login("user", "1234");
			Assert.That(() => bridge.ManagingPurchaseHistory("store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { bridge.ManagingPurchaseHistory("store3"); });
		}

		[Test, Order(27)]
		public void Test_5_1()
		{
			Message m = bridge.ViewMessage("store1").Last();
			Assert.That(() => bridge.AddProduct("store1", "chair", "with wheels", "furniture", 350, 1), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.AddStoreManager("store1", "notManager"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.MessageReply(m, "store1", "not thank you"); });
			Assert.That(() => bridge.ViewMessage("store1"), Throws.Nothing);
		}

		[Test, Order(31)]
		public void Test_6_4()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.StorePurchaseHistory("store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.UserPurchaseHistory("user"); });
			bridge.Login("notManager", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.StorePurchaseHistory("store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { bridge.UserPurchaseHistory("user"); });
			bridge.Logout();
			bridge.Login("admin", "sadnaTeam");
			Assert.That(() => bridge.StorePurchaseHistory("store1"), Throws.Nothing);
			Assert.That(() => bridge.UserPurchaseHistory("user"), Throws.Nothing);
		}
	}
}