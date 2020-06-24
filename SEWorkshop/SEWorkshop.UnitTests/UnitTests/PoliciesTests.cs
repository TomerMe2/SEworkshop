using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Models;
using SEWorkshop.Models.Policies;
using SEWorkshop.Enums;
using SEWorkshop.DAL;
using System.Data.Entity;

namespace SEWorkshop.Tests.UnitTests
{
    [TestFixture]
    class PoliciesTests
    {
        private Store Str { get; set; }
        private LoggedInUser StoreOwner { get; set; }
        private LoggedInUser Buyer { get; set; }
        private Basket Bskt { get; set; }
        private Product Prod1 { get; set; }
        private Product Prod2 { get; set; }
        private Product Prod3 { get; set; }
        private Product Prod4 { get; set; }
        private Product Prod5 { get; set; }

        private Address DefAdrs = new Address("Israel", "Beer Sheva", "Ben Gurion", "44", "1234");

        [OneTimeSetUp]
        public void Init()
        {
            DatabaseProxy.MoveToTestDb();
        }


        [SetUp]
        public void Setup()
        {
            Buyer = new LoggedInUser("buyer", new byte[1] { 0 });
            StoreOwner = new LoggedInUser("owner", new byte[1] { 0 });
            Str = Store.StoreBuilder(StoreOwner, "storenm");
            Prod1 = new Product(Str, "prod1", "desc1", "cat1", 1, 999);
            Prod2 = new Product(Str, "prod2", "desc2", "cat2", 2, 999);
            Prod3 = new Product(Str, "prod3", "desc3", "cat3", 3, 999);
            Prod4 = new Product(Str, "prod4", "desc4", "cat4", 4, 999);
            Prod5 = new Product(Str, "prod5", "desc5", "cat5", 5, 999);

            Str.Products.Add(Prod1);
            Str.Products.Add(Prod2);
            Str.Products.Add(Prod3);
            Str.Products.Add(Prod4);
            Str.Products.Add(Prod5);

            Bskt = new Basket(Str, Buyer.Cart);
            Buyer.Cart.Baskets.Add(Bskt);
        }

        [Test]
        public void AlwaysTruePolicyReturnsTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 3));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod4, 9));
            var pol = new AlwaysTruePolicy(Str);
            Assert.IsTrue(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void SingleProductQuantityRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 7));
            var pol = new SingleProductQuantityPolicy(Str, Prod2, 5, 7);
            Assert.IsTrue(pol.CanPurchase(Buyer, DefAdrs));
            pol = new SingleProductQuantityPolicy(Str, Prod2, 7, 8);
            Assert.IsTrue(pol.CanPurchase(Buyer, DefAdrs));
        }


        [Test]
        public void SingleProductQuantityRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new SingleProductQuantityPolicy(Str, Prod2, 5, 7);
            Assert.False(pol.CanPurchase(Buyer, DefAdrs));
            pol = new SingleProductQuantityPolicy(Str, Prod2, 9, 10);
            Assert.False(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void SystemDayRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new SystemDayPolicy(Str, (Weekday)((int)DateTime.Now.AddDays(1).DayOfWeek));  //can't buy tomorrow
            Assert.True(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void SystemDayRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new SystemDayPolicy(Str, (Weekday)((int)DateTime.Now.DayOfWeek));  //can't buy today
            Assert.False(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void UserCityRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new UserCityPolicy(Str, "Beer Sheva");
            Assert.True(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void UserCityRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new UserCityPolicy(Str, "Beer Sheva");
            var addr = new Address("Israel", "Tel Aviv", "Ben Gurion", "44", "1234");
            Assert.False(pol.CanPurchase(Buyer, addr));
        }

        [Test]
        public void UserCountryRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new UserCountryPolicy(Str, "Israel");
            Assert.True(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void UserCityContryFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 8));
            var pol = new UserCountryPolicy(Str, "Israel");
            var addr = new Address("Germany", "Beer Sheva", "Ben Gurion", "44", "1234");
            Assert.False(pol.CanPurchase(Buyer, addr));
        }

        [Test]
        public void WholeStoreQuantityRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod5, 7));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 3));
            var pol = new WholeStoreQuantityPolicy(Str, 10, 13);
            Assert.True(pol.CanPurchase(Buyer, DefAdrs));
            pol = new WholeStoreQuantityPolicy(Str, 9, 10);
            Assert.True(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void WholeStoreQuantityRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod5, 7));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod2, 3));
            var pol = new WholeStoreQuantityPolicy(Str, 11, 13);
            Assert.False(pol.CanPurchase(Buyer, DefAdrs));
            pol = new WholeStoreQuantityPolicy(Str, 8, 9);
            Assert.False(pol.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void OrRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod3, 5));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 2));
            var pol1 = new SingleProductQuantityPolicy(Str, Prod3, 7, -1);
            var pol2 = new SingleProductQuantityPolicy(Str, Prod1, -1, 9);
            pol1.InnerPolicy = pol2;
            pol1.InnerOperator = Operator.Or;
            Assert.True(pol1.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void OrRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod3, 5));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 2));
            var pol1 = new SingleProductQuantityPolicy(Str, Prod3, 7, -1);
            var pol2 = new SingleProductQuantityPolicy(Str, Prod1, 5, 9);
            pol1.InnerPolicy = pol2;
            pol1.InnerOperator = Operator.Or;
            Assert.False(pol1.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void XorRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod3, 5));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 2));
            var pol1 = new SingleProductQuantityPolicy(Str, Prod3, 7, -1);
            var pol2 = new SingleProductQuantityPolicy(Str, Prod1, -1, 9);
            pol1.InnerPolicy = pol2;
            pol1.InnerOperator = Operator.Or;
            Assert.True(pol1.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void XorRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod3, 5));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 2));
            var pol1 = new SingleProductQuantityPolicy(Str, Prod3, 5, -1);
            var pol2 = new SingleProductQuantityPolicy(Str, Prod1, 1, 9);
            pol1.InnerPolicy = pol2;
            pol1.InnerOperator = Operator.Xor;
            Assert.False(pol1.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void AndRetTrue()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod3, 5));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 2));
            var pol1 = new SingleProductQuantityPolicy(Str, Prod3, 5, -1);
            var pol2 = new SingleProductQuantityPolicy(Str, Prod1, 1, 9);
            pol1.InnerPolicy = pol2;
            pol1.InnerOperator = Operator.Xor;
            Assert.False(pol1.CanPurchase(Buyer, DefAdrs));
        }

        [Test]
        public void AndRetFalse()
        {
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod3, 5));
            Bskt.Products.Add(new ProductsInBasket(Bskt, Prod1, 2));
            var pol1 = new SingleProductQuantityPolicy(Str, Prod3, 7, -1);
            var pol2 = new SingleProductQuantityPolicy(Str, Prod1, -1, 9);
            pol1.InnerPolicy = pol2;
            pol1.InnerOperator = Operator.Or;
            Assert.True(pol1.CanPurchase(Buyer, DefAdrs));
        }
    }
}
