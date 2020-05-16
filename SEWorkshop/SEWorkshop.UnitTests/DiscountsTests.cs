using System;
using NUnit.Framework;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using SEWorkshop.Models.Discounts;

namespace SEWorkshop.Tests
{
    public class DiscountsTests
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

        private Address DefAdrs = new Address("Israel", "Beer Sheva", "Ben Gurion", "44");
        
        private DateTime Deadline { get; set; }
        
        [SetUp]
        public void Setup()
        {
            Buyer = new LoggedInUser("buyer", new byte[1] { 0 });
            StoreOwner = new LoggedInUser("owner", new byte[1] { 0 });
            Str = new Store(StoreOwner, "storenm");
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

            Bskt = new Basket(Str);
            Buyer.Cart.Baskets.Add(Bskt);

            Deadline = Convert.ToDateTime("20/5/2022");
        }
        
        [Test]
        public void ProductCategoryDiscountTest()
        {
            Bskt.Products.Add((Prod1, 3));
            Bskt.Products.Add((Prod2, 3));
            Discount dis = new ProductCategoryDiscount(50, Deadline, Prod1, Str, "cat1");
            double discount = dis.ComposeDiscounts(Bskt.Products);
            Assert.That(discount, Is.EqualTo(Prod1.Price * 0.5 * 3)); //prod2 is not under discount
        }
        
        [Test]
        public void SpecificProductDiscountTest()
        {
            Bskt.Products.Add((Prod1, 3));
            Bskt.Products.Add((Prod2, 3));
            Discount dis = new SpecificProducDiscount(75, Deadline, Prod2, Str);
            double discount = dis.ComposeDiscounts(Bskt.Products);
            Assert.That(discount, Is.EqualTo(Prod2.Price * 0.75 * 3)); //prod2 is not under discount
        }

        [Test]
        public void XorDiscountsTest()
        {
            Bskt.Products.Add((Prod1, 3));
            Bskt.Products.Add((Prod2, 3));
            Discount dis = new SpecificProducDiscount(50, Deadline, Prod2, Str);
            dis.InnerDiscount = (new SpecificProducDiscount(50, Deadline, Prod1, Str), Operator.Xor);
            double discount = dis.ComposeDiscounts(Bskt.Products);
            Assert.That(discount, Is.EqualTo(Prod1.Price * 0.5 * 3)); //prod1 is cheaper after discount
        }

        [Test]
        public void AndDiscountTest()
        {
            Bskt.Products.Add((Prod1, 3));
            Bskt.Products.Add((Prod2, 3));
            Discount dis = new SpecificProducDiscount(50, Deadline, Prod2, Str);
            dis.InnerDiscount = (new SpecificProducDiscount(50, Deadline, Prod1, Str), Operator.And);
            double discount = dis.ComposeDiscounts(Bskt.Products);
            Assert.That(discount, Is.EqualTo(Prod1.Price* 0.5 * 3 + Prod2.Price * 0.5 * 3));
        }

        [Test]
        public void ImpliesDiscountTest()
        {
            Bskt.Products.Add((Prod1, 3));
            Bskt.Products.Add((Prod2, 3));
            Discount dis = new SpecificProducDiscount(50, Deadline, Prod2, Str);
            dis.InnerDiscount = (new SpecificProducDiscount(50, Deadline, Prod1, Str), Operator.Implies);
            double discount = dis.ComposeDiscounts(Bskt.Products);
            Assert.That(discount, Is.EqualTo(Prod1.Price* 0.5 * 3 + Prod2.Price * 0.5 * 3));
        }

        [Test]
        public void ExpiredDiscountException()
        {
            Bskt.Products.Add((Prod1, 3));
            Bskt.Products.Add((Prod2, 3));
            Discount dis = new SpecificProducDiscount(50, Convert.ToDateTime("15/5/2020"), Prod2, Str);
            Assert.Throws<DiscountExpiredException>(delegate { dis.ComposeDiscounts(Bskt.Products); });
        }
    }
}