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

        [SetUp]
        public void SetUp()
        {
            Manager = new UserManager();
        }
        //You should run this test separately from the other test
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
            Manager.AddProduct(DEF_ID, "store2", "Wakanda", "forever", "lol", 10, 10);
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user4", "1234");
            Manager.AddProductToCart(DEF_ID, "store1", "Wakanda", 5);
            Manager.AddProductToCart(DEF_ID, "store2", "Wakanda", 5);
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
            Manager.Logout(DEF_ID);
            Manager.Login(DEF_ID, "user1", "1234");
            Assert.IsTrue(Manager.ManagingPurchaseHistory(DEF_ID, "store1").Count() == 1);
            Manager.SetPermissionsOfManager(DEF_ID, "store2", "user3", "Products");
            Manager.Logout(DEF_ID);
        }

        [Test, Order(3)]
        public void DALTest3()
        {
            Manager.Login(DEF_ID, "user3", "1234");
            Assert.IsTrue(Manager.ManagingPurchaseHistory(DEF_ID, "store2").Count() == 1);
            Assert.That(() => Manager.AddProduct(DEF_ID, "store2", "best", "product", "ever", 10, 10), Throws.Nothing);
            Manager.Logout(DEF_ID);
        }
    }
}
