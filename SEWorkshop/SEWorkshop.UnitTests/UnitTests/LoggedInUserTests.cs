using NUnit.Framework;
using System;
using System.Collections.Generic;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using System.Linq;
using SEWorkshop.Enums;
using SEWorkshop.Models.Discounts;
using SEWorkshop.Models.Policies;
using SEWorkshop.DAL;

namespace SEWorkshop.Tests.UnitTests
{
    [TestFixture]
    public class LoggedInUserTests
    {
        SecurityAdapter _securityAdapter = new SecurityAdapter();
        private Address DEF_ADRS = new Address("1", "1", "1", "1");

        [OneTimeSetUp]
        public void Init()
        {
            DatabaseProxy.MoveToTestDb();
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WriteReview()
        {
            const string STORE_NAME = "writeReviewStore";
            const string USER_NAME = "WriteReviewUser";
            const string USER_PASSWORD = "1111";
            const string PROD_NAME = "writeReviewProd";
            LoggedInUser usr = new LoggedInUser(USER_NAME, _securityAdapter.Encrypt(USER_PASSWORD));
            Store str = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, str, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            str.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product prod = usr.AddProduct(str, PROD_NAME, "ninini", "cat1", 11.11, 1);
            //Review is empty - Throw Exception
            Assert.Throws<ReviewIsEmptyException>(delegate { usr.WriteReview(prod, ""); });
            //Review is not empty and user is logged in - success
            Assert.That(() => usr.WriteReview(prod, "baddd"), Throws.Nothing);
        }

        [Test]
        public void WriteMessage()
        {
            const string STORE_NAME = "writeMessageStore";
            const string USER_NAME = "WriteMessageUser";
            const string USER_PASSWORD = "1111";
            LoggedInUser usr = new LoggedInUser(USER_NAME, _securityAdapter.Encrypt(USER_PASSWORD));
            Store str = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, str, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            str.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            //Message is empty - Throw Exception
            Assert.Throws<MessageIsEmptyException>(delegate { usr.WriteMessage(str, "", true); });
            //Message is not empty and user is logged in - success
            Assert.That(() => usr.WriteMessage(str, "baddd", true), Throws.Nothing);
        }

        [Test]
        public void AddProductTest_storeManagerAddsAProductToStore_productIsAddedToStore()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = usr.AddProduct(store, "BestApp", "Authentic One", "App", 4.00, 10);

            Assert.IsTrue(store.Products.Contains(product));
            bool catched = false;
            try
            {
                usr.AddProduct(store, "BestApp", "Fake One", "App", 4.00, 10);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);

        }

        [Test]
        public void RemoveProduct_StoreOwnerRemovesAnExistingProduct_ProductIsRemovedFromStore()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = usr.AddProduct(store, "BestApp", "Authentic One", "App", 4.00, 10);
            usr.RemoveProduct(store, product);
            Assert.IsTrue(!store.Products.Contains(product));
            bool catched = false;
            try
            {
                usr.RemoveProduct(store, product);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }

        [Test]
        public void EditProductDescription_StoreOwnerEditsDescription_success()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductDescription(store, product, "Awesome App");
            Assert.IsTrue(product.Description.Equals("Awesome App"));

        }

        [Test]
        public void EditProductCategory_StoreOwnerEditsCategory_success()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductCategory(store, product, "General Communication");
            Assert.IsTrue(product.Category.Equals("General Communication"));

        }

        [Test]
        public void EditProductName_StoreOwnerEditName_success()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product1 = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            Product product2 = new Product(store, "Instagram", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product1);
            store.Products.Add(product2);
            usr.EditProductName(store, product1, "Messenger");
            Assert.IsTrue(product1.Name.Equals("Messenger"));
            bool catched = false;
            try
            {
                usr.EditProductName(store, product1, "Instagram");
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched && product1.Name.Equals("Messenger"));

        }

        [Test]
        public void EditProductPrice_StoreOwnerEditsPrice_success()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductPrice(store, product, 1.00);
            Assert.IsTrue(product.Price == 1.00);


        }

        [Test]
        public void EditProductQuantity_StoreOwnerEditsQuantity_success()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductQuantity(store, product, 200);
            Assert.IsTrue(product.Quantity == 200);

        }
        [Test]
        public void SetPermissionsOfManager_setPermissionOfStoreManagerToSetPermission_PermissionIsAdded()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Replying);
            var management = newManager.Manage.FirstOrDefault(man => (man.Store.Name == (store.Name)));

            Assert.IsTrue(management.HasAuthorization(Authorizations.Replying));
        }

        [Test]
        public void SetPermissionsOfManager_SetPermissionsOfManagerReplying_repliyngAddedToManger()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);

            usr.SetPermissionsOfManager(store, newManager, Authorizations.Replying);
            LoggedInUser client = new LoggedInUser("client1", _securityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, store, "Great store!", true);
            store.Messages.Add(message1);
            Message reply1 = newManager.MessageReply(message1, store, "Thank you!");

            Assert.IsTrue(message1.Next == reply1 && reply1.Prev == message1
                            && reply1.Description.Equals("Thank you!"));
        }

        [Test]
        public void SetPermissionsOfManager_PermissionsOfManagerWatching_ManagercanWatch()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Watching);
            LoggedInUser client = new LoggedInUser("client1", _securityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, store, "Great store!", true);
            store.Messages.Add(message1);
            IEnumerable<Message> output1 = newManager.GetMessage(store);
            Assert.IsTrue(output1.Contains(message1));

        }

        [Test]
        public void SetPermissionsOfManager_SetPermissionsOfManagerTestProducts_managerHasPermission()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Products);
            Product product;
            product = newManager.AddProduct(store, "ManagerProduct1", "Facebook", "Social", 4.00, 100);
            Assert.IsTrue(store.Products.Contains(product));

        }

        [Test]
        public void AddStoreOwner_storeOwnerAddAnotherLoggeinUserTbeStoreOwner_OwnerIsAdded()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newOwner = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreOwner(store, newOwner);
            Assert.IsTrue(store.GetOwnership(newOwner).Appointer == usr);
        }



        [Test]
        public void AddStoreManagerTest_StoreownerAddsAManagerToStore_ManagerIsAdded()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            Assert.IsTrue(store.GetManagement(newManager).Appointer == usr);
            bool catched = false;
            try
            {
                usr.AddStoreManager(store, newManager);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);

        }


        [Test]
        public void RemoveStoreManager_storeOwnerRemovesStoreManager_MangerIsRemoved()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            usr.RemoveStoreManager(store, newManager);
            Assert.IsTrue(store.GetManagement(newManager) == null);
            bool catched = false;
            try
            {
                usr.RemoveStoreManager(store, newManager);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }

        [Test]
        public void MessageReply_StoreOwnerReplyStoresMessage_replyhasbeenAdded()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser client = new LoggedInUser("client", _securityAdapter.Encrypt("1324"));
            Message message = new Message(client, store, "Great app", true);
            store.Messages.Add(message);
            Message reply = usr.MessageReply(message, store, "Thank you!");
            Assert.IsTrue(reply.Prev == message && message.Next == reply && reply.Description.Equals("Thank you!"));

        }

        [Test]
        public void getMassage_storeOwnerViewsaMessageOfStore_UserGetsMessage()
        {
            const string STORE_NAME = "Google Play";

            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));

            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser client = new LoggedInUser("client", _securityAdapter.Encrypt("1324"));

            Message message = new Message(client, store, "Great app", true);
            store.Messages.Add(message);

            IEnumerable<Message> messages = usr.GetMessage(store);
            /*
              Assert.IsTrue(messages.Count() == 1 && messages.First() == message);*/

        }

        [Test]
        public void ViewPurchaseHistory_storeOwnerviewPurchaseHistory_success()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            store.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            LoggedInUser client = new LoggedInUser("client", _securityAdapter.Encrypt("1324"));
            Purchase purchase = new Purchase(client, new Basket(store, client.Cart), DEF_ADRS);
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> purchases = usr.PurchaseHistory(store);
            Assert.IsTrue(purchases.Count() == 1 && purchases.First() == purchase);

        }
        
        [Test]
        public void PurchasePolicyTest()
        {
            const string STORE_NAME = "Wello";
            LoggedInUser usr = new LoggedInUser("someusr", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            Assert.IsInstanceOf<AlwaysTruePolicy>(store.Policy);
            Owns ownership = new Owns(usr, store, new LoggedInUser("Demo", _securityAdapter.Encrypt("1234")));
            usr.Owns.Add(ownership);
            store.Ownership.Add(ownership);
            usr.AddWholeStoreQuantityPolicy(store, Enums.Operator.And, 2, 5);
            Assert.IsInstanceOf<WholeStoreQuantityPolicy>(store.Policy);
            usr.AddSystemDayPolicy(store, Enums.Operator.Xor, Weekday.Monday);
            Assert.IsInstanceOf<WholeStoreQuantityPolicy>(store.Policy);
            Assert.IsInstanceOf<SystemDayPolicy>(store.Policy.InnerPolicy);
            usr.RemovePolicy(store, 1);
            Assert.IsInstanceOf<WholeStoreQuantityPolicy>(store.Policy);
            Assert.IsNull(store.Policy.InnerPolicy);
            usr.RemovePolicy(store, 0);
            Assert.IsInstanceOf<AlwaysTruePolicy>(store.Policy);
        }

        [Test]
        public void PurchaseDiscountTest()
        {
            const string STORE_NAME = "store1";
            DateTime deadline = DateTime.Now.AddMonths(1);
            LoggedInUser usr = new LoggedInUser("someusr", _securityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("Demo", _securityAdapter.Encrypt("1234")));
            //usr.Owns.Add(ownership);
            //store.Ownership.Add(ownership);
            Product prod1 = usr.AddProduct(store, "prod1", "ninini", "cat1", 11.11, 11);
            usr.AddProductCategoryDiscount(store, "cat1", deadline, 50, Operator.And, 0, 1, true);
            Assert.IsInstanceOf<ProductCategoryDiscount>(store.Discounts.ElementAt(0));
            int id = store.Discounts.ElementAt(0).DiscountId;
            usr.AddSpecificProductDiscount(store, prod1, deadline, 50, Operator.Xor, 0, id, false);
            Assert.IsInstanceOf<SpecificProducDiscount>((store.Discounts.ElementAt(0).Father).RightChild);
            usr.AddSpecificProductDiscount(store, prod1, deadline, 50, Operator.Xor, 5, 1, true);
            usr.RemoveDiscount(store, 0);
            Assert.IsInstanceOf<SpecificProducDiscount>(store.Discounts.ElementAt(0));
        }

        [Test]
        public void BuyOverTest()
        {
            const string STORE_NAME = "writeReviewStore";
            const string USER_NAME = "WriteReviewUser";
            const string USER_PASSWORD = "1111";
            const string PROD_NAME = "writeReviewProd";
            LoggedInUser usr = new LoggedInUser(USER_NAME, _securityAdapter.Encrypt(USER_PASSWORD));
            Store str = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, str, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            str.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product prod = usr.AddProduct(str, PROD_NAME, "ninini", "cat1", 10.00, 5);
            BuyOverDiscount dis = new BuyOverDiscount(str, 9.90, 50, DateTime.Today, prod);
            GuestUser customer = new GuestUser();
            customer.AddProductToCart(prod, 1);
            var basket = customer.Cart.Baskets.ElementAt(0);
            Assert.AreEqual(dis.ComputeDiscount(basket.Products), 5.00);

        }

        [Test]
        public void BuySomeGetSomeTest()
        {
            const string STORE_NAME = "buySomeGetSomeStore";
            const string USER_NAME = "BuySomeGetSomeUser";
            const string USER_PASSWORD = "1111";
            const string PROD1_NAME = "Nails";
            const string PROD2_NAME = "ScrewDriver";
            LoggedInUser usr = new LoggedInUser(USER_NAME, _securityAdapter.Encrypt(USER_PASSWORD));
            Store str = Store.StoreBuilder(usr, STORE_NAME);
            Owns ownership = new Owns(usr, str, new LoggedInUser("DEMO", _securityAdapter.Encrypt("1234")));
            str.Ownership.Add(ownership);
            usr.Owns.Add(ownership);
            Product nails = usr.AddProduct(str, PROD1_NAME, "ninini", "cat1", 10, 5);
            Product screwDriver = usr.AddProduct(str, PROD2_NAME, "ninini", "cat1", 10, 5);
            BuySomeGetSomeDiscount dis = new BuySomeGetSomeDiscount(str, 3, -1, 60, DateTime.Today, screwDriver, nails);
            GuestUser customer = new GuestUser();
            customer.AddProductToCart(nails, 3);
            customer.AddProductToCart(screwDriver, 2);
            var basket1 = customer.Cart.Baskets.ElementAt(0);
            Assert.AreEqual(dis.ComputeDiscount(basket1.Products), 0);
            customer.AddProductToCart(screwDriver, 1);
            var basket2 = customer.Cart.Baskets.ElementAt(0);
            Assert.AreEqual(dis.ComputeDiscount(basket1.Products), 18);
        }

    }
}
