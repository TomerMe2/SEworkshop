using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
using System.Linq;

namespace SEWorkshop.Tests
{
    [TestFixture]
    public class LoggedInUserTests
    {
        SecurityAdapter _securityAdapter = new SecurityAdapter();

        [Test]
        public void WriteReview()
        {

        }

        [Test]
        public void WriteMessage()
        {

        }
        
        [Test]
        public void AddProductTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product= usr.AddProduct(store, "BestApp", "Authentic One", "App", 4.00, 10);

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
        public void RemoveProduct()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
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
        public void EditProductDescription()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductDescription(store, product, "Awesome App");
            Assert.IsTrue(product.Description.Equals("Awesome App"));

        }

        [Test]
        public void EditProductCategory()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductCategory(store, product, "General Communication");
            Assert.IsTrue(product.Category.Equals("General Communication"));

        }

        [Test]
        public void EditProductName()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product1 = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            Product product2 = new Product(store, "Instagram", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product1);
            store.Products.Add(product2);
            usr.EditProductName( store, product1, "Messenger");
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
        public void EditProductPrice()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductPrice(store, product, 0.00);
            Assert.IsTrue(product.Price == 0);


        }

        [Test]
        public void EditProductQuantity()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            Product product = new Product(store, "Whatsapp", "Great app!", "Personal Communication", 4.00, 100);
            store.Products.Add(product);
            usr.EditProductQuantity(store, product, 200);
            Assert.IsTrue(product.Quantity == 200);

        }
        [Test]
        public void SetPermissionsOfManagerTestOwner()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager,store);
            newManager.Manage.Add(management);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Products);
            LoggedInUser ownerToTest1 = new LoggedInUser("appdevloper2", _securityAdapter.Encrypt("1234"));
            bool success = true;
            try
            {
                newManager.AddStoreOwner(store, ownerToTest1);
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && store.Owners[ownerToTest1] == newManager);

            LoggedInUser ownerToTest2 = new LoggedInUser("appdevloper3", _securityAdapter.Encrypt("1234"));
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Products);
            try
            {
                newManager.AddStoreOwner(store, ownerToTest2);
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
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Manager);
            LoggedInUser managerToTest1 = new LoggedInUser("appdevloper2", _securityAdapter.Encrypt("1234"));
            bool success = true;
            try
            {
                newManager.AddStoreManager(store, managerToTest1);
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && store.Managers[managerToTest1] == newManager);

            LoggedInUser managerToTest2 = new LoggedInUser("appdevloper3", _securityAdapter.Encrypt("1234"));
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Manager);
            try
            {
                newManager.AddStoreManager(store, managerToTest2);
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
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Authorizing);
            LoggedInUser managerToTest1 = new LoggedInUser("appmanager2", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(managerToTest1, newManager);
            Manages management1 = new Manages(managerToTest1, store);
            managerToTest1.Manage.Add(management1);
            bool success = true;
            try
            {
                newManager.SetPermissionsOfManager(store, managerToTest1, Authorizations.Products);
            }
            catch
            {
                success = false;
            }
            var manage = managerToTest1.Manage.FirstOrDefault(man => man.Store.Equals(store));
            Assert.IsTrue(success);

            usr.SetPermissionsOfManager(store, newManager, Authorizations.Authorizing);
            try
            {
                newManager.SetPermissionsOfManager(store, managerToTest1, Authorizations.Products);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);
            LoggedInUser managerToTest2 = new LoggedInUser("appmanager2", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(managerToTest2, usr);
            Manages management2 = new Manages(managerToTest2, store);
            managerToTest2.Manage.Add(management2);
            try
            {
                newManager.SetPermissionsOfManager(store, managerToTest1, Authorizations.Authorizing);
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
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Replying);
            LoggedInUser client = new LoggedInUser("client1", _securityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, "Great store!");
            store.Messages.Add(message1);
            bool success = true;
            Message reply1 = new Message(usr, "");
            try
            {
                reply1 = newManager.MessageReply(message1, store, "Thank you!");
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && message1.Next == reply1 && reply1.Prev == message1
                            && reply1.Description.Equals("Thank you!"));
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Replying);

            Message message2 = new Message(client, "Piece of garbage...");
            store.Messages.Add(message2);
            Message reply2 = new Message(usr, "");
            try
            {
                reply2 = newManager.MessageReply(message2, store, "goto L");
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
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Watching);
            LoggedInUser client = new LoggedInUser("client1", _securityAdapter.Encrypt("1234"));
            Message message1 = new Message(client, "Great store!");
            store.Messages.Add(message1);
            IEnumerable<Message> output1 = new List<Message>();
            bool success = true;
            try
            {
                output1 = newManager.getMessage(store);
            }
            catch
            {
                success = false;
            }
            Assert.IsTrue(success && output1.Contains(message1));

            usr.SetPermissionsOfManager(store, newManager, Authorizations.Watching);
            Purchase purchase = new Purchase(client, new Basket(store));
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> output2 = new List<Purchase>();
            try
            {
                output2 = newManager.ViewPurchaseHistory(store);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success && !output2.Contains(purchase));
        }

        [Test]
        public void SetPermissionsOfManagerTestProducts()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            store.Managers.Add(newManager, usr);
            Manages management = new Manages(newManager, store);
            newManager.Manage.Add(management);
            usr.SetPermissionsOfManager(store, newManager, Authorizations.Products);
            Product product;
            bool success = true;
            try
            {
                product = newManager.AddProduct(store, "ManagerProduct1", "Facebook", "Social", 4.00, 100);
            }
            catch
            {
                success = false;
                product = null;
            }
            Assert.IsTrue(success && store.Products.Contains(product));

            usr.SetPermissionsOfManager(store, newManager, Authorizations.Products);
            try
            {
                newManager.AddProduct(store, "ManagerProduct2", "Facebook", "Social", 4.00, 100);
                success = false;
            }
            catch
            {
                success = true;
            }
            Assert.IsTrue(success);


        }

        [Test]
        public void AddStoreOwnerTest()
        {
          
        }



        [Test]
        public void AddStoreManagerTest()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            Assert.IsTrue(store.Managers.Contains(new KeyValuePair<LoggedInUser, LoggedInUser>(newManager, usr)));
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
        public void RemoveStoreManager()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser newManager = new LoggedInUser("appmanager1", _securityAdapter.Encrypt("1234"));
            usr.AddStoreManager(store, newManager);
            usr.RemoveStoreManager(store, newManager);
            Assert.IsTrue(!store.Managers.Contains(new KeyValuePair<LoggedInUser, LoggedInUser>(newManager, usr)));
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
        public void MessageReply()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser client = new LoggedInUser("client", _securityAdapter.Encrypt("1324"));
            Message message = new Message(client, "Great app");
            store.Messages.Add(message);
            Message reply = usr.MessageReply( message, store, "Thank you!");
            Assert.IsTrue(reply.Prev == message && message.Next == reply && reply.Description.Equals("Thank you!"));

        }

        [Test]
        public void getMassage()
        {
            const string STORE_NAME = "Google Play";
          
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));

              Store store = new Store(usr, STORE_NAME);
              LoggedInUser client = new LoggedInUser("client", _securityAdapter.Encrypt("1324"));
            
              Message message = new Message(client, "Great app");
              store.Messages.Add(message);
            
            IEnumerable<Message> messages = usr.getMessage(store);
            /*
              Assert.IsTrue(messages.Count() == 1 && messages.First() == message);*/

        }

        [Test]
        public void ViewPurchaseHistory()
        {
            const string STORE_NAME = "Google Play";
            LoggedInUser usr = new LoggedInUser("appdevloper1", _securityAdapter.Encrypt("1234"));
            Store store = new Store(usr, STORE_NAME);
            LoggedInUser client = new LoggedInUser("client", _securityAdapter.Encrypt("1324"));
            Purchase purchase = new Purchase(client, new Basket(store));
            store.Purchases.Add(purchase);
            IEnumerable<Purchase> purchases = usr.ViewPurchaseHistory(store);
            Assert.IsTrue(purchases.Count() == 1 && purchases.First() == purchase);

        }

    }
}
