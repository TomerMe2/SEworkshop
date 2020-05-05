using System;
using NUnit.Framework;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace SEWorkshop.UnitTests
{
	[TestFixture]
	public class AcceptanceTests
	{
		private UserManager um = new UserManager();

		[Test, Order(1)]
		public void Test_2_2()
        {
			Assert.That(() => um.Register("user", "1234"), Throws.Nothing);
			Assert.Throws<UserAlreadyExistsException>(delegate { um.Register("user", "1234"); });
			Assert.That(() => um.Register("secondOwner", "1234"), Throws.Nothing);
			Assert.That(() => um.Register("notManager", "1234"), Throws.Nothing);
			Assert.That(() => um.Register("managerFirstOwner", "1234"), Throws.Nothing);
			Assert.That(() => um.Register("managerSecondOwner", "1234"), Throws.Nothing);
		}

		[Test, Order(2)]
		public void Test_2_3()
		{
			Assert.Throws<UserDoesNotExistException>(delegate { um.Login("user3", "1234"); });
			Assert.That(() => um.Login("user", "1234"), Throws.Nothing);
		}

		[Test, Order(13)]
		public void Test_2_4()
		{
			Assert.That(() => um.BrowseStores(), Throws.Nothing);
		}


		[Test, Order(7)]
		public void Test_2_5_1()
		{
			um.AddProduct("store1", "bisli", "snack", "food", 5, 1);
			um.AddProduct("store1", "doritos", "snack", "food", 6, 1);
			um.AddProduct("store1", "tv", "television", "electronics", 850, 1);
			um.AddProduct("store1", "flower", "smells nice", "plants", 25, 1);
			string tv = "tv";
			Assert.That(() => um.SearchProductsByName(ref tv), Throws.Nothing);
			Assert.AreEqual(um.SearchProductsByName(ref tv).Count(), 1);
		}

		[Test, Order(8)]
		public void Test_2_5_2()
		{
			string food = "food";
			Assert.That(() => um.SearchProductsByCategory(ref food), Throws.Nothing);
			Assert.AreEqual(um.SearchProductsByCategory(ref food).Count(), 2);
		}

		[Test, Order(9)]
		public void Test_2_5_3()
		{
			string bsli = "bsli";
			Assert.That(() => um.SearchProductsByKeywords(ref bsli), Throws.Nothing);
			Assert.AreEqual(um.SearchProductsByKeywords(ref bsli).Count(), 1);
		}

		[Test, Order(10)]
		public void Test_2_5_4()
		{
			string food = "food";
			var products = um.SearchProductsByCategory(ref food).ToList();
			Assert.That(() => um.FilterProducts(products, (x) => x.Name.Equals("bisli")), Throws.Nothing);
			Assert.AreEqual((um.FilterProducts(products, (x) => x.Name.Equals("bisli"))).Count(), 1);
		}

		[Test, Order(11)]
		public void Test_2_6()
		{
			um.OpenStore("store2");
			Assert.Throws<ProductNotInTheStoreException>(delegate { um.AddProductToCart("store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { um.AddProductToCart("store1", "tv", -1); });
			um.Logout();
			Assert.Throws<ProductNotInTheStoreException>(delegate { um.AddProductToCart("store2", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { um.AddProductToCart("store1", "tv", -1); });
			
			Assert.That(() => um.AddProductToCart("store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(um.MyCart().First().Products.Count() == 1);
			um.Login("user", "1234");
			Assert.That(() => um.AddProductToCart("store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(um.MyCart().First().Products.Count() == 2);

		}

		[Test, Order(12)]
		public void Test_2_7()
		{
			Assert.That(() => um.MyCart(), Throws.Nothing);
			Assert.IsTrue(um.MyCart().First().Products.Count() == 2);
			Assert.Throws<ProductIsNotInCartException>(delegate { um.RemoveProductFromCart("store1", "flower", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { um.RemoveProductFromCart("store2", "bisli", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.RemoveProductFromCart("store3", "bisli", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { um.RemoveProductFromCart("store1", "bisli", -1); });
			Assert.IsTrue(um.MyCart().First().Products.Count() == 2);
			Assert.That(() => um.RemoveProductFromCart("store1", "bisli", 1), Throws.Nothing);
			Assert.IsTrue(um.MyCart().First().Products.Count() == 1);
			um.Logout();
			Assert.That(() => um.MyCart(), Throws.Nothing);
			Assert.IsTrue(um.MyCart().Count() == 0);
			um.AddProductToCart("store1", "tv", 1);
			Assert.Throws<ProductIsNotInCartException>(delegate { um.RemoveProductFromCart("store1", "doritos", 1); });
			Assert.Throws<ProductNotInTheStoreException>(delegate { um.RemoveProductFromCart("store2", "tv", 1); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.RemoveProductFromCart("store3", "tv", 1); });
			Assert.Throws<NegativeQuantityException>(delegate { um.RemoveProductFromCart("store1", "tv", -1); });
			Assert.IsTrue(um.MyCart().First().Products.Count() == 1);
			Assert.That(() => um.RemoveProductFromCart("store1", "tv", 1), Throws.Nothing);
			Assert.IsTrue(um.MyCart().First().Products.Count() == 0);
			um.Login("user", "1234");
		}

		[Test, Order(14)]
		public void Test_2_8()
		{
			um.AddProductToCart("store1", "bisli", 1);
            string creditCardNumber = "5555";
			Address address = new Address("Pardes Hanna-Karkur", "Hadarim", "1");
            var basket = um.MyCart().First();
			Assert.That(() => um.Purchase(basket, creditCardNumber, address), Throws.Nothing);
			Assert.Throws<BasketNotInSystemException>(delegate { um.Purchase(basket); });
		}

		[Test, Order(30)]
		public void Test_3_1()
		{	
			Assert.That(() => um.Logout(), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { um.Logout(); });
		}

		[Test, Order(3)]
		public void Test_3_2()
		{
			Assert.That(() => um.OpenStore("store1"), Throws.Nothing);
			Assert.Throws<StoreWithThisNameAlreadyExistsException>(delegate { um.OpenStore("store1"); });
		}

		[Test, Order(20)]
		public void Test_3_3()
		{
			Assert.Throws<ReviewIsEmptyException>(delegate { um.WriteReview("store1", "bisli", ""); });
			Assert.That(() => um.WriteReview("store1", "bisli", "product is good"), Throws.Nothing);	
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.WriteReview("store1", "bisli", "product is bad"); });
		}

		[Test, Order(21)]
		public void Test_3_5()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { um.WriteMessage("store1", "store is bad"); });
			um.Login("user", "1234");
			Assert.Throws<MessageIsEmptyException>(delegate { um.WriteMessage("store1", ""); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.WriteMessage("store3", "store is ok"); });
			Assert.That(() => um.WriteMessage("store1", "store is good"), Throws.Nothing);
		}

		[Test, Order(22)]
		public void Test_3_7()
		{
			Assert.That(() => um.PurchaseHistory(), Throws.Nothing);
			Assert.NotNull(um.PurchaseHistory());
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.PurchaseHistory(); });
		}

		[Test, Order(4)]
		public void Test_4_1_1()
		{
			Assert.That(() => um.AddProduct("store1", "bamba", "peanut snack", "food", 4.5, 1), Throws.Nothing);
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.AddProduct("store1", "bamba2", "peanut snack", "food", 4.5, 1); });
			um.Login("secondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { um.AddProduct("store1", "bamba3", "peanut snack", "food", 4.5, 1); });
			um.Logout();
			um.Login("user", "1234");

		}

		[Test, Order(6)]
		public void Test_4_1_2()
		{
			string bamba2 = "bamba2";
			Assert.That(() => um.RemoveProduct("store1", "bamba2"), Throws.Nothing);
			Assert.IsEmpty(um.SearchProductsByName(ref bamba2));
		}

		[Test, Order(5)]
		public void Test_4_1_3()
		{
			string bamba = "bamba";
			string bamba2 = "bamba2";
			var bamb = um.SearchProductsByName(ref bamba).First();
			Assert.That(() => um.EditProductName("store1", "bamba", "bamba2"), Throws.Nothing);
			Assert.That(() => um.EditProductCategory("store1", "bamba2", "electronics"), Throws.Nothing);
			Assert.That(() => um.EditProductPrice("store1", "bamba2", 215), Throws.Nothing);
			Assert.That(() => um.EditProductDescription("store1", "bamba2", "electric bamba"), Throws.Nothing);
			Assert.AreEqual(um.SearchProductsByName(ref bamba2).Count(), 1);
			Assert.AreEqual(um.SearchProductsByName(ref bamba).Count(), 0);
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.EditProductName("store1", "bamba2", "bamba3"); });
			um.Login("user", "1234");
		}

		[Test, Order(23)]
		public void Test_4_3()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { um.AddStoreOwner("store1", "secondOwner"); });
			um.Login("user", "1234");
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.AddStoreOwner("store3", "secondOwner"); });
			Assert.That(() => um.AddStoreOwner("store1", "secondOwner"), Throws.Nothing);
			um.AddStoreManager("store1", "managerFirstOwner");
			um.Logout();
			um.Login("secondOwner", "1234");
			Assert.Throws<UserIsAlreadyStoreOwnerException>(delegate { um.AddStoreOwner("store1", "user"); });
		}

		[Test, Order(24)]
		public void Test_4_5()
		{
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.AddStoreManager("store3", "managerSecondOwner"); });
			Assert.That(() => um.AddStoreManager("store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserIsAlreadyStoreManagerException>(delegate { um.AddStoreManager("store1", "managerFirstOwner"); });
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.AddStoreManager("store1", "notManager"); });
			um.Login("user", "1234");
		}

		[Test, Order(25)]
		public void Test_4_6()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { um.SetPermissionsOfManager("store1", "managerSecondOwner", "Products"); });
			Assert.That(() => um.SetPermissionsOfManager("store1", "managerFirstOwner", "Watching"), Throws.Nothing);
			um.Logout();
			um.Login("secondOwner", "1234");
			Assert.That(() => um.SetPermissionsOfManager("store1", "managerSecondOwner", "Products"), Throws.Nothing);
		}

		[Test, Order(29)]
		public void Test_4_7()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { um.RemoveStoreManager("store1", "managerSecondOwner"); });
			um.Logout();
			um.Login("secondOwner", "1234");
			//Assert.Throws<UserHasNoPermissionException>(delegate { um.RemoveStoreManager("store2", "user"); });
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.RemoveStoreManager("store3", "managerFirstOwner"); });
			Assert.That(() => um.RemoveStoreManager("store1", "managerSecondOwner"), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { um.RemoveStoreManager("store1", "managerFirstOwner"); });
			Assert.Throws<UserIsNotMangerOfTheStoreException>(delegate { um.RemoveStoreManager("store1", "notManager"); });
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.RemoveStoreManager("store1", "managerFirstOwner"); });
			um.Login("user", "1234");
		}

		[Test, Order(26)]
		public void Test_4_9()
		{
			Assert.That(() => um.ViewMessage("store1"), Throws.Nothing);
			var m = um.ViewMessage("store1").Last();
			Assert.That(() => um.MessageReply(m, "store1", "thank you"), Throws.Nothing);
			um.Logout();
			um.Login("managerSecondOwner", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { um.MessageReply(m, "store1", "not thank you"); });
		}

		[Test, Order(28)]
		public void Test_4_10()
		{
			Assert.That(() => um.ManagingPurchaseHistory("store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.ManagingPurchaseHistory("store3"); });
			um.Logout();
			Assert.Throws<UserHasNoPermissionException>(delegate { um.ManagingPurchaseHistory("store1"); });
			um.Login("user", "1234");
			Assert.That(() => um.ManagingPurchaseHistory("store1"), Throws.Nothing);
			Assert.Throws<StoreNotInTradingSystemException>(delegate { um.ManagingPurchaseHistory("store3"); });
		}

		[Test, Order(27)]
		public void Test_5_1()
		{
			var m = um.ViewMessage("store1").Last();
			Assert.That(() => um.AddProduct("store1", "chair", "with wheels", "furniture", 350, 1), Throws.Nothing);
			Assert.Throws<UserHasNoPermissionException>(delegate { um.AddStoreManager("store1", "notManager"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { um.MessageReply(m, "store1", "not thank you"); });
			Assert.That(() => um.ViewMessage("store1"), Throws.Nothing);
		}

		[Test, Order(31)]
		public void Test_6_4()
		{
			Assert.Throws<UserHasNoPermissionException>(delegate { um.StorePurchaseHistory("store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { um.UserPurchaseHistory("user"); });
			um.Login("notManager", "1234");
			Assert.Throws<UserHasNoPermissionException>(delegate { um.StorePurchaseHistory("store1"); });
			Assert.Throws<UserHasNoPermissionException>(delegate { um.UserPurchaseHistory("user"); });
			um.Logout();
			um.Login("admin", "sadnaTeam");
			Assert.That(() => um.StorePurchaseHistory("store1"), Throws.Nothing);
			Assert.That(() => um.UserPurchaseHistory("user"), Throws.Nothing);
		}
	}
}