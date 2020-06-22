using NUnit.Framework;
using NUnit.Framework.Constraints;
using SEWorkshop.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Tests.IntegrationTests
{
    class DALTests
    {
        private IUserManager Manager { get; set; }
        private const string DEF_ID = "DALTests";

        //You could run each test separately by its order, or run everything at once

        [SetUp]
        public void SetUp()
        {
            Manager = new UserManager();
        }
        [Test, Order(1)]
        public void DALTest1()
        {
            Manager.Register(DEF_ID, "user1", "1234");
            Manager.Register(DEF_ID, "user2", "1234");
            Manager.Register(DEF_ID, "user3", "1234");
            Manager.Register(DEF_ID, "user4", "1234");
            Manager.Login(DEF_ID, "user1", "1234");
            Manager.OpenStore(DEF_ID, "store1");
            Manager.OpenStore(DEF_ID, "store2");
            Manager.AddStoreOwner(DEF_ID, "store1", "user2");
            Manager.AddStoreManager(DEF_ID, "store2", "user3");
            Manager.AddProduct(DEF_ID, "store1", "Wakanda", "forever", "lol", 10, 10);
            Manager.AddProduct(DEF_ID, "store2", "Wakanda2", "forever", "lol", 10, 10);
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user4", "1234");
            Manager.AddProductToCart(DEF_ID, "store1", "Wakanda", 5);
            Manager.AddProductToCart(DEF_ID, "store2", "Wakanda2", 5);
            Assert.IsTrue(Manager.MyCart(DEF_ID).Count() == 2);
            Manager.Purchase(DEF_ID, Manager.MyCart(DEF_ID).ElementAt(0), "5555", new Address("Israel", "Beersheba", "Rager blv.", "123"));
            Assert.IsTrue(Manager.MyCart(DEF_ID).Count() == 1);
            Manager.Logout(DEF_ID);
        }

        [Test, Order(2)]
        public void DALTest2()
        {
            Manager.Login(DEF_ID, "user4", "1234");
            Assert.IsTrue(Manager.PurchaseHistory(DEF_ID).Count() == 1);
            Assert.IsTrue(Manager.MyCart(DEF_ID).Count() == 1);
            Manager.Purchase(DEF_ID, Manager.MyCart(DEF_ID).ElementAt(0), "5555", new Address("Israel", "Beersheba", "Rager blv.", "124"));
            Manager.WriteMessage(DEF_ID, "store2", "Wakanda Forever!");
            Manager.WriteReview(DEF_ID, "store2", "Wakanda2", "Forever!");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user1", "1234");
            Manager.MessageReply(DEF_ID, Manager.ViewMessage(DEF_ID, "store2").ElementAt(0), "store2", "Mr. Stark, I don't feek so well");
            Assert.IsTrue(Manager.ManagingPurchaseHistory(DEF_ID, "store1").Count() == 1);
            Manager.SetPermissionsOfManager(DEF_ID, "store2", "user3", "Products");
            Manager.Logout(DEF_ID);
            Manager.AddProductToCart(DEF_ID, "store1", "Wakanda", 3);
            Manager.Purchase(DEF_ID, Manager.MyCart(DEF_ID).ElementAt(0), "5555", new Address("Israel", "Beersheba", "Rager blv.", "125"));
        }

        [Test, Order(3)]
        public void DALTest3()
        {
            Manager.Login(DEF_ID, "user3", "1234");
            Assert.IsTrue(Manager.ManagingPurchaseHistory(DEF_ID, "store2").Count() == 1);
            Assert.That(() => Manager.AddProduct(DEF_ID, "store2", "best", "product", "ever", 10, 10), Throws.Nothing);
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user1", "1234");
            Assert.IsTrue(Manager.ManagingPurchaseHistory(DEF_ID, "store1").Count() == 2);
            string input = "Wakanda2";
            Assert.IsTrue(Manager.SearchProductsByName(ref input).ElementAt(0).Reviews.Count() == 1);
            Assert.IsTrue(Manager.ViewMessage(DEF_ID, "store2").Count() == 2);
            Assert.IsTrue(Manager.ViewMessage(DEF_ID, "store2").ElementAt(0).Next.Description
                    .Equals(Manager.ViewMessage(DEF_ID, "store2").ElementAt(1).Description));
            Assert.IsTrue(Manager.ViewMessage(DEF_ID, "store2").ElementAt(1).Prev.Description
                    .Equals(Manager.ViewMessage(DEF_ID, "store2").ElementAt(0).Description));
            Manager.AddSpecificProductDiscount(DEF_ID, "store1", "Wakanda", DateTime.Today, 10, Enums.Operator.And, 0, 0, true);
            Manager.AddBuyOverDiscount(3, DEF_ID, "store1", "Wakanda", DateTime.Today, 5, Enums.Operator.And, 0, 1, true);
            Manager.AddSystemDayPolicy(DEF_ID, "store1", Enums.Operator.Or, Enums.Weekday.Sunday);
            Manager.AddSystemDayPolicy(DEF_ID, "store1", Enums.Operator.Or, Enums.Weekday.Monday);
            Manager.AddSystemDayPolicy(DEF_ID, "store1", Enums.Operator.Or, Enums.Weekday.Tuesday);
            Manager.Logout(DEF_ID);
        }

        [Test, Order(4)]
        public void DALTest4()
        {
            Manager.Login(DEF_ID, "user1", "1234");
            Assert.IsTrue(Manager.SearchStore("store1").Discounts.Count() == 3);
            Assert.IsTrue(Manager.SearchStore("store1").Discounts.ElementAt(0).rightChild.DiscountId ==
                Manager.SearchStore("store1").Discounts.ElementAt(1).DiscountId);
            Assert.IsTrue(Manager.SearchStore("store1").Discounts.ElementAt(0).leftChild.DiscountId ==
                Manager.SearchStore("store1").Discounts.ElementAt(2).DiscountId);
            Assert.IsTrue(Manager.SearchStore("store1").Discounts.ElementAt(0).DiscountId ==
                Manager.SearchStore("store1").Discounts.ElementAt(1).father.DiscountId);
            Assert.IsTrue(Manager.SearchStore("store1").Discounts.ElementAt(0).DiscountId ==
                Manager.SearchStore("store1").Discounts.ElementAt(2).father.DiscountId);
            Manager.RemoveDiscount(DEF_ID, "store1", 0);
            Assert.IsTrue(Manager.SearchStore("store1").Discounts.Count() == 0);
            Assert.IsTrue(Manager.SearchStore("store1").Policies.Count() == 3);
            Manager.RemovePolicy(DEF_ID, "store1", 1);
            Manager.Logout(DEF_ID);
        }

        [Test, Order(5)]
        public void DALTest5()
        {
            Assert.That(Manager.SearchStore("store1").Policies.Count(), Is.EqualTo(2));
            Manager.Login(DEF_ID, "user1", "1234");
            Manager.AddStoreOwner(DEF_ID, "store1", "user4");
            Manager.AnswerOwnershipRequest(DEF_ID, "store1", "user4", Enums.RequestState.Approved);
            Manager.AddStoreOwner(DEF_ID, "store1", "user3");
            Manager.AddStoreOwner(DEF_ID, "store2", "user4");
            Manager.AddStoreOwner(DEF_ID, "store2", "user2");
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user2", "1234");
            Manager.AnswerOwnershipRequest(DEF_ID, "store1", "user4", Enums.RequestState.Approved);
            Manager.Logout(DEF_ID);
        }

        [Test, Order(6)]
        public void DALTest6()
        {
            Assert.IsTrue(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("store1").Policies.ElementAt(0)).CantBuyIn == Enums.Weekday.Sunday);
            Assert.IsTrue(((DataModels.Policies.DataSystemDayPolicy)Manager.SearchStore("store1").Policies.ElementAt(1)).CantBuyIn == Enums.Weekday.Tuesday);
            Manager.Login(DEF_ID, "user2", "1234");
            Assert.IsTrue(Manager.SearchStore("store1").Ownership.Count() == 3);
            Manager.AnswerOwnershipRequest(DEF_ID, "store1", "user3", Enums.RequestState.Approved);
            Assert.IsTrue(Manager.SearchStore("store1").Ownership.Count() == 4);
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user4", "1234");
            Assert.IsTrue(Manager.SearchStore("store2").Ownership.Count() == 2);
            Manager.AnswerOwnershipRequest(DEF_ID, "store2", "user2", Enums.RequestState.Denied);
            Assert.IsTrue(Manager.SearchStore("store2").Ownership.Count() == 2);
            Manager.Logout(DEF_ID);
        }
    }
}
