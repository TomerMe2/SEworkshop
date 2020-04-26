using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SEWorkshop.Facades;
using SEWorkshop.Models;
using SEWorkshop.Adapters;
using System.Linq;
using SEWorkshop.Exceptions;

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
    }
}
