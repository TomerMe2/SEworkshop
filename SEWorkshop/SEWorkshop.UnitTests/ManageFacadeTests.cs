using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using System.Linq;

namespace SEWorkshop.UnitTests
{
    [TestFixture]
    class ManageFacadeTests
    {
        private IManageFacade Facade { get; set; }
        private ISecurityAdapter SecurityAdapter { get; set; } 

        [OneTimeSetUp]
        public void Init()
        {
            Facade = ManageFacade.GetInstance();
            SecurityAdapter = new SecurityAdapter();
        }

        [Test]
        public void AddProductTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
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
            Store store = new Store(usr, STORE_NAME);
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
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newOwner = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreOwner(usr, store, newOwner);
            Assert.IsTrue(store.Owners.Contains(new KeyValuePair<LoggedInUser, LoggedInUser>(newOwner, usr)));
            bool catched = false;
            try
            {
                Facade.AddStoreOwner(usr, store, newOwner);
            }
            catch
            {
                catched = true;
            }
            Assert.IsTrue(catched);
        }

        [Test]
        public void AddStoreManagerTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreManager(usr, store, newManager);
            Assert.IsTrue(store.Managers.Contains(new KeyValuePair<LoggedInUser, LoggedInUser>(newManager, usr)));
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
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            Facade.AddStoreManager(usr, store, newManager);
            Facade.RemoveStoreManager(usr, store, newManager);
            Assert.IsTrue(!store.Managers.Contains(new KeyValuePair<LoggedInUser, LoggedInUser>(newManager, usr)));
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
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser client = new LoggedInUser("client", SecurityAdapter.Encrypt("1324"));
            Message message = new Message(client, "Great app");
            store.Messages.Add(message);
            IEnumerable<Message> messages = Facade.ViewMessage(usr, store);
            Assert.IsTrue(messages.Count() == 1 && messages.First() == message);
        }

        [Test]
        public void ViewPurchaseHistoryTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser client = new LoggedInUser("client", SecurityAdapter.Encrypt("1324"));
            Purchase purchase = new Purchase(client, new Basket(store));
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> purchases = Facade.ViewPurchaseHistory(usr, store);
            Assert.IsTrue(purchases.Count() == 1 && purchases.First() == purchase);
        }

        [Test]
        public void MessageReplyTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser client = new LoggedInUser("client", SecurityAdapter.Encrypt("1324"));
            Message message = new Message(client, "Great app");
            store.Messages.Add(message);
            Message reply = Facade.MessageReply(usr, message, store, "Thank you!");
            Assert.IsTrue(reply.Prev == message && message.Next == reply && reply.Description.Equals("Thank you!"));
        }

        [Test]
        public void EditProductDescriptionTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            Facade.EditProductDescription(usr, store, product, "Awesome App");
            Assert.IsTrue(product.Description.Equals("Awesome App"));
        }

        [Test]
        public void EditProductNameTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product1 = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            Product product2 = new Product(store, "Instagram", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product1);
            store.Products.Add(product2);
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
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            Facade.EditProductPrice(usr, store, product, 0.00);
            Assert.IsTrue(product.Price == 0);
        }

        [Test]
        public void EditProductCategoryTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            Facade.EditProductCategory(usr, store, product, "General Communication");
            Assert.IsTrue(product.Category.Equals("General Communication"));
        }

        [Test]
        public void EditProductQuantityTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            Facade.EditProductQuantity(usr, store, product, 200);
            Assert.IsTrue(product.Quantity == 200);
        }

        [Test]
        public void SetPermissionsOfManagerTestProducts()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Products);
            Product product;
            bool success = true;
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

            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Products);
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
        }

        [Test]
        public void SetPermissionsOfManagerTestOwner()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Owner);
            LoggedInUser ownerToTest1 = new LoggedInUser("appdevloper2", SecurityAdapter.Encrypt("1234"));
            bool success = true;
            try
            {
                Facade.AddStoreOwner(newManager, store, ownerToTest1);
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && store.Owners[ownerToTest1] == newManager);

            LoggedInUser ownerToTest2 = new LoggedInUser("appdevloper3", SecurityAdapter.Encrypt("1234"));
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Owner);
            try
            {
                Facade.AddStoreOwner(newManager, store, ownerToTest2);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void SetPermissionsOfManagerTestManager()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
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
            Assert.IsTrue(success && store.Managers[managerToTest1] == newManager);
            
            LoggedInUser managerToTest2 = new LoggedInUser("appdevloper3", SecurityAdapter.Encrypt("1234"));
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Manager);
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
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Authorizing);
            LoggedInUser managerToTest1 = new LoggedInUser("appmanager2", SecurityAdapter.Encrypt("1234"));
            managerToTest1.AddStoreManager(store, newManager);
            store.Managers.Add(managerToTest1, newManager);
            Manages management1 = new Manages(managerToTest1, store);
            managerToTest1.Manage.Add(management1);
            bool success = true;
            try
            {
                Facade.SetPermissionsOfManager(newManager, store, managerToTest1, Authorizations.Products);
            }
            catch
            {
                success = false;
            }
            var manage = managerToTest1.Manage.FirstOrDefault(man => man.Store.Equals(store));
            Assert.IsTrue(success && manage.AuthoriztionsOfUser.Contains(Authorizations.Manager));
            
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Authorizing);
            try
            {
                Facade.SetPermissionsOfManager(newManager, store, managerToTest1, Authorizations.Products);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
            LoggedInUser managerToTest2 = new LoggedInUser("appmanager2", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(managerToTest2, usr);
            Manages management2 = new Manages(managerToTest2, store);
            managerToTest2.Manage.Add(management2);
            try
            {
                Facade.SetPermissionsOfManager(newManager, store, managerToTest1, Authorizations.Authorizing);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
        }

        [Test]
        public void SetPermissionsOfManagerTestReplying()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Replying);
            LoggedInUser client = new LoggedInUser("client1", SecurityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, "Great store!");
            store.Messages.Add(message1);
            bool success = true;
            Message reply1 = new Message(usr, "");
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
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Replying);

            Message message2 = new Message(client, "Piece of garbage...");
            store.Messages.Add(message2);
            Message reply2 = new Message(usr, "");
            try
            {
                reply2 = Facade.MessageReply(newManager, message2, store, "goto L");
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success && message2.Next != reply2
                            && reply2.Description.Equals(""));
        }

        [Test]
        public void SetPermissionsOfManagerTestWatching()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", SecurityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", SecurityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            Facade.SetPermissionsOfManager(usr, store, newManager, Authorizations.Watching);
            LoggedInUser client = new LoggedInUser("client1", SecurityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, "Great store!");
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
            Purchase purchase = new Purchase(client, new Basket(store));
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> output2 = new List<Purchase>();
            try
            {
                output2 = Facade.ViewPurchaseHistory(newManager, store);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success && !output2.Contains(purchase));
        }
    }
}