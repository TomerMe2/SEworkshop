using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using System.Linq;
using SEWorkshop.Exceptions;
using SEWorkshop.Enums;
using SEWorkshop.DAL;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace SEWorkshop.Tests.IntegrationTests
{
    [TestFixture]
    class ManageFacadeTests
    {
        private IManageFacade Facade { get; set; }
        private ISecurityAdapter SecurityAdapter { get; set; }
        private Address DEF_ADRS = new Address("1", "1", "1", "1");


        [OneTimeSetUp]
        public void Init()
        {
            Facade = new ManageFacade();
            SecurityAdapter = new SecurityAdapter();
            DatabaseProxy.MoveToTestDb();
        }

        [Test]
        public void AddProductTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product = Facade.AddProduct(usr, store, "BestApp", "Authentic One", "App", 4.00, 10);
            Assert.IsTrue(store.Products.Contains(product));
            bool catched = false;
            try
            {
                Facade.AddProduct(usr, store, "BestApp", "Fake One", "App", 4.00, 10);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }
        
        [Test]
        public void RemoveProductTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product = Facade.AddProduct(usr, store, "BestApp", "Authentic One", "App", 4.00, 10);
            Facade.RemoveProduct(usr, store, product);
            Assert.IsTrue(!store.Products.Contains(product));
            bool catched = false;
            try
            {
                Facade.RemoveProduct(usr, store, product);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }

        [Test]
        public void AddStoreOwnerTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newOwner = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(usr, store, newOwner);
            Assert.IsTrue(store.GetOwnership(newOwner).Appointer == usr);
            bool success = false;
            try
            {
                Facade.AddStoreOwner(usr, store, newOwner);
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void AddStoreManagerTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreManager(usr, store, newManager);
            Assert.IsTrue(store.GetManagement(newManager).Appointer == usr);
            bool catched = false;
            try
            {
                Facade.AddStoreManager(usr, store, newManager);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }

        [Test]
        public void RemoveStoreManagerTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreManager(usr, store, newManager);
            Facade.RemoveStoreManager(usr, store, newManager);
            Assert.IsTrue(store.GetManagement(newManager) == null);
            bool catched = false;
            try
            {
                Facade.RemoveStoreManager(usr, store, newManager);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }

        [Test]
        public void ViewMessageTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser client = new LoggedInUser("client", SecurityAdapter.Encrypt("1324"));
            Message message = new Message(client, store, "Great app", true);
            store.Messages.Add(message);
            IEnumerable<Message> messages = Facade.ViewMessage(usr, store);
            Assert.IsTrue(messages.Count() == 1 && messages.First() == message);
        }

        [Test]
        public void ViewPurchaseHistoryTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser client = new LoggedInUser("client", SecurityAdapter.Encrypt("1324"));
            Purchase purchase = new Purchase(client, new Basket(store, usr.Cart), DEF_ADRS);
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> purchases = Facade.ViewPurchaseHistory(usr, store);
            Assert.IsTrue(purchases.Count() == 1 && purchases.First() == purchase);
        }

        [Test]
        public void MessageReplyTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser client = new LoggedInUser("client", SecurityAdapter.Encrypt("1324"));
            Message message = new Message(client, store, "Great app", true);
            store.Messages.Add(message);
            Message reply = Facade.MessageReply(usr, message, store, "Thank you!");
            Assert.IsTrue(reply.Prev == message && message.Next == reply && reply.Description.Equals("Thank you!"));
        }

        [Test]
        public void EditProductDescriptionTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product = Facade.AddProduct(usr, store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //store.Products.Add(product);
            Facade.EditProductDescription(usr, store, product, "Awesome App");
            Assert.IsTrue(product.Description.Equals("Awesome App"));
        }

        [Test]
        public void EditProductNameTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product1 = Facade.AddProduct(usr, store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //Product product1 = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //store.Products.Add(product1);
            //Product product2 = new Product(store, "Instagram", "Great app!", "Personal Communication", 4.00, 100);
            //store.Products.Add(product2);
            Product product2 = Facade.AddProduct(usr, store, "Instagram", "Great app!", "Personal Communication", 4.00, 100);
            Facade.EditProductName(usr, store, product1, "Messenger");
            Assert.IsTrue(product1.Name.Equals("Messenger"));
            bool catched = false;
            try
            {
                Facade.EditProductName(usr, store, product1, "Instagram");
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched && product1.Name.Equals("Messenger"));
        }
        [Test]
        public void EditProductPriceTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product = Facade.AddProduct(usr, store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //store.Products.Add(product);
            Facade.EditProductPrice(usr, store, product, 1.00);
            Assert.IsTrue(product.Price == 1);
        }

        [Test]
        public void EditProductCategoryTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product = Facade.AddProduct(usr, store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //store.Products.Add(product);
            Facade.EditProductCategory(usr, store, product, "General Communication");
            Assert.IsTrue(product.Category.Equals("General Communication"));
        }

        [Test]
        public void EditProductQuantityTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            Product product = Facade.AddProduct(usr, store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            //store.Products.Add(product);
            Facade.EditProductQuantity(usr, store, product, 200);
            Assert.IsTrue(product.Quantity == 200);
        }

        [Test]
        public void SetPermissionsOfManagerTestProducts()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            Product product;
            bool success = true;
            try
            {
                Facade.AddProduct(newManager, store, "ManagerProduct2", "Facebook", "Social", 4.00, 100);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Products);
            try
            {
                product = Facade.AddProduct(newManager, store, "ManagerProduct1", "Facebook", "Social", 4.00, 100);
            }
            catch
            {
                success = false;
                product = null;
            }
            Assert.IsTrue(success && store.Products.Contains(product));
        }

        [Test]
        public void SetPermissionsOfManagerTestOwner()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Owner);
            LoggedInUser ownerToTest1 = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            bool success = true;
            try
            {
                Facade.AddStoreOwner(newManager, store, ownerToTest1);
                success = true;
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success );

         
        }

        [Test]
        public void SetPermissionsOfManagerTestManager()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store,newManager);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Manager);
            LoggedInUser managerToTest1 = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            bool success = true;
            try
            {
                Facade.AddStoreManager(newManager, store, managerToTest1);
            }
            catch
            {
                success = false;
            }
            Manages management = store.GetManagement(managerToTest1);
            Assert.IsTrue(success && management.Appointer.Username == newManager.Username);
            
            LoggedInUser managerToTest2 = new LoggedInUser("appdevloper3", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreManager(newManager,store,managerToTest2);
            try
            {
                Facade.AddStoreManager(newManager, store, managerToTest2);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void SetPermissionsOfManagerTestAuthorizing()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Authorizing);
            bool success = true;
            try
            {
                Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Authorizing);
            }
            catch
            {
                success = false;
            }
            //var manage = managerToTest1.Manage.FirstOrDefault(man => man.Store.Equals(store));
            Assert.IsTrue(success);

          
            var auto= newManager.Manage.FirstOrDefault(man => man.Store==(store));

            Assert.IsTrue(auto.HasAuthorization(Authorizations.Authorizing));
            LoggedInUser managerToTest = new LoggedInUser("appmanager2", SecurityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, managerToTest);
            try
            {
                Facade.SetPermissionsOfManager(newManager, store, managerToTest, Authorizations.Products);
                success = true;
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void SetPermissionsOfManagerTestReplying()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Replying);
            LoggedInUser client = new LoggedInUser("client1", SecurityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, store, "Great store!", true);
            store.Messages.Add(message1);
            bool success = true;
            Message reply1 = new Message(usr, store, "", false);
            try
            {
                reply1 = Facade.MessageReply(newManager, message1, store, "Thank you!");
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && message1.Next == reply1 && reply1.Prev == message1
                            && reply1.Description.Equals("Thank you!"));
           
            Message message2 = new Message(client, store, "Piece of garbage...", true);
            store.Messages.Add(message2);
            Message reply2 = new Message(usr, store, "", false);
            try
            {
                reply2 = Facade.MessageReply(newManager, message2, store, "goto L");
                success = true;
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && message2.Next == reply2
                            && reply2.Description.Equals("goto L"));
        }

        [Test]
        public void SetPermissionsOfManagerTestWatching()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            Manages management = new Manages(newManager, store, usr);
            store.Management.Add(management);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Watching);
            LoggedInUser client = new LoggedInUser("client1", SecurityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, store, "Great store!", false);
            store.Messages.Add(message1);
            IEnumerable<Message> output1 = new List<Message>();
            bool success = true;
            try
            {
                output1 = Facade.ViewMessage(newManager, store);
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && output1.Contains(message1));

            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Watching);
            Purchase purchase = new Purchase(client, new Basket(store, usr.Cart), DEF_ADRS);
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> output2 = new List<Purchase>();
            try
            {
                output2 = Facade.ViewPurchaseHistory(newManager, store);
                success = true;
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && output2.Contains(purchase));
        }

        [Test]
        public void AddingNewStoreOwnerIsDenied()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newOwner = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(usr, store, newOwner);
            LoggedInUser secondOwner = new LoggedInUser("appdevloper3", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(usr, store, secondOwner);
            Assert.IsTrue(store.RequestExists(secondOwner));
            Assert.IsFalse(store.GetOwnership(secondOwner) != null);
            Facade.AnswerOwnershipRequest(newOwner, store, secondOwner, RequestState.Denied);
            Assert.IsFalse(store.GetOwnership(secondOwner) != null);
        }

        [Test]
        public void AddingAndApprovingNewStoreOwner()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newOwner = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(usr, store, newOwner);
            LoggedInUser secondOwner = new LoggedInUser("appdevloper3", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(usr, store, secondOwner);
            Assert.IsFalse(store.GetOwnership(secondOwner) != null);
            Assert.IsTrue(store.RequestExists(secondOwner));
            Facade.AnswerOwnershipRequest(newOwner,store,secondOwner,RequestState.Approved);
            Assert.IsTrue(store.GetOwnership(secondOwner) != null);
            //Assert.IsFalse(store.RequestExists(secondOwner));
        }

        [Test]
        public void AddingAndApprovingNewStoreOwner3Owners()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser firstOwner = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(firstOwner, STORE_NAME);
            LoggedInUser secondOwner = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(firstOwner, store, secondOwner);
            LoggedInUser thirdOwner = new LoggedInUser("appdevloper3", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(firstOwner, store, thirdOwner);
            Facade.AnswerOwnershipRequest(secondOwner, store, thirdOwner, RequestState.Approved);

            LoggedInUser forthOwner = new LoggedInUser("appdevloper4", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(firstOwner, store, forthOwner);

            Assert.IsTrue(store.GetOwnership(thirdOwner) != null);
            //Assert.IsTrue(store.RequestExists(thirdOwner));

            Facade.AnswerOwnershipRequest(secondOwner, store, forthOwner, RequestState.Approved);
            //Facade.AnswerOwnershipRequest(thirdOwner, store, forthOwner, RequestState.Approved);

            Assert.IsTrue(store.GetOwnership(forthOwner) != null);
            //Assert.IsFalse(store.RequestExists(forthOwner));
        }


        [Test]
        public void RemovePermissionsOfManager()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = Store.StoreBuilder(usr, STORE_NAME);
            //Owns ownership = new Owns(usr, store, new LoggedInUser("DEMO", SecurityAdapter.Encrypt("1234")));
            //store.Ownership.Add(ownership);
            //usr.Owns.Add(ownership);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            Manages management = new Manages(newManager, store, usr);
            store.Management.Add(management);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Products);
            Assert.That(() => newManager.AddProduct(store, "proddi", "ninini", "cat1", 11.11, 1), Throws.Nothing);
            Facade.RemovePermissionsOfManager(usr, store, newManager, Authorizations.Products);
            Assert.Throws<UserHasNoPermissionException>(delegate
            {
                newManager.AddProduct(store, "failedProd", "ninini", "cat1", 11.11, 1);
            });
        }

        [Test]
        public void AddBuySomeGetSomeDiscount()
        {
            const string STORE_NAME = "bsgs_store1";
            const string PROD1_NAME = "bsgs_prod1";
            const string PROD2_NAME = "bsgs_prod2";
            LoggedInUser bsgs_user1 = new LoggedInUser("bsgs_user1", SecurityAdapter.Encrypt("1234"));
            Store bsgs_store1 = Store.StoreBuilder(bsgs_user1, STORE_NAME);
            Product bsgs_prod1 = Facade.AddProduct(bsgs_user1, bsgs_store1, PROD1_NAME, "nini", "cat1", 10, 10);
            Product bsgs_prod2 = Facade.AddProduct(bsgs_user1, bsgs_store1, PROD2_NAME, "nini", "cat1", 10, 10);
            Facade.AddBuySomeGetSomeDiscount(bsgs_user1, bsgs_store1, bsgs_prod1, bsgs_prod2, 2, 1, DateTime.Now.AddMonths(1),  100, Operator.And, 0, 1, true);
            bsgs_user1.AddProductToCart(bsgs_prod1, 2);
            bsgs_user1.AddProductToCart(bsgs_prod2, 1);
            //how to get discountes price?
        }

        [Test]
        public void AddProductCategoryDiscount()
        {
            const string STORE_NAME = "pc_store1";
            const string PROD1_NAME = "pc_prod1";
            const string PROD2_NAME = "pc_prod2";
            LoggedInUser pc_user1 = new LoggedInUser("pc_user1", SecurityAdapter.Encrypt("1234"));
            Store pc_store1 = Store.StoreBuilder(pc_user1, STORE_NAME);
            Product pc_prod1 = Facade.AddProduct(pc_user1, pc_store1, PROD1_NAME, "nini", "cat1", 10, 10);
            Product pc_prod2 = Facade.AddProduct(pc_user1, pc_store1, PROD2_NAME, "nini", "cat1", 10, 10);
            Facade.AddProductCategoryDiscount(pc_user1, pc_store1, "cat1", DateTime.Now.AddMonths(1), 50, Operator.And, 0, 1, true);
            pc_user1.AddProductToCart(pc_prod1, 2);
            pc_user1.AddProductToCart(pc_prod2, 1);
            //how to get discountes price?
        }

        [Test]
        public void AddSpecificProductDiscount()
        {
            const string STORE_NAME = "sp_store1";
            const string PROD1_NAME = "sp_prod1";
            const string PROD2_NAME = "sp_prod2";
            LoggedInUser sp_user1 = new LoggedInUser("sp_user1", SecurityAdapter.Encrypt("1234"));
            Store sp_store1 = Store.StoreBuilder(sp_user1, STORE_NAME);
            Product sp_prod1 = Facade.AddProduct(sp_user1, sp_store1, PROD1_NAME, "nini", "cat1", 10, 10);
            Product sp_prod2 = Facade.AddProduct(sp_user1, sp_store1, PROD2_NAME, "nini", "cat1", 10, 10);
            Facade.AddSpecificProductDiscount(sp_user1, sp_store1, sp_prod1, DateTime.Now.AddMonths(1), 10, Operator.And, 0, 1, true);
            sp_user1.AddProductToCart(sp_prod1, 2);
            sp_user1.AddProductToCart(sp_prod2, 1);
            //how to get discountes price?
        }

        [Test]
        public void AddBuyOverDiscount()
        {
            const string STORE_NAME = "bo_store1";
            const string PROD1_NAME = "bo_prod1";
            const string PROD2_NAME = "bo_prod2";
            LoggedInUser bo_user1 = new LoggedInUser("bo_user1", SecurityAdapter.Encrypt("1234"));
            Store bo_store1 = Store.StoreBuilder(bo_user1, STORE_NAME);
            Product bo_prod1 = Facade.AddProduct(bo_user1, bo_store1, PROD1_NAME, "nini", "cat1", 10, 10);
            Product bo_prod2 = Facade.AddProduct(bo_user1, bo_store1, PROD2_NAME, "nini", "cat1", 10, 10);
            Facade.AddBuyOverDiscount(bo_user1, bo_store1, bo_prod1, 10, DateTime.Now.AddMonths(1), 2, Operator.And, 0, 1, true);
            bo_user1.AddProductToCart(bo_prod1, 2);
            bo_user1.AddProductToCart(bo_prod2, 1);
            //how to get discountes price?
        }

        [Test]
        public void RemoveDiscount()
        {
            const string STORE_NAME = "rd_store1";
            LoggedInUser rd_user1 = new LoggedInUser("rd_user1", SecurityAdapter.Encrypt("1234"));
            Store rd_store1 = Store.StoreBuilder(rd_user1, STORE_NAME);
            Facade.AddProductCategoryDiscount(rd_user1, rd_store1, "cat1", DateTime.Now.AddMonths(1), 10, Operator.And, 0, 1, true);
            Facade.AddProductCategoryDiscount(rd_user1, rd_store1, "cat2", DateTime.Now.AddMonths(1), 10, Operator.Or, 0, 1, true);
            Facade.AddProductCategoryDiscount(rd_user1, rd_store1, "cat3", DateTime.Now.AddMonths(1), 10, Operator.Xor, 0, 1, true);
            Assert.That(rd_store1.Discounts.Count, Is.EqualTo(5));
            Facade.RemoveDiscount(rd_user1, rd_store1, 0);
            Assert.That(rd_store1.Discounts, Is.Empty);
        }
    }
}