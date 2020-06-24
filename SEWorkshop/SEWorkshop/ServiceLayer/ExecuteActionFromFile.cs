using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SEWorkshop.ServiceLayer
{
    class ExecuteActionFromFile
    {
        private readonly IUserManager userManager;
        private const string DEF_SID = "1";

        public ExecuteActionFromFile(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        public void ReadAndExecute()
        {
            try
            {
                string jsonFile = System.IO.File.ReadAllText("ActionsFile.json");
                List<JObject> jobjectsList = JsonConvert.DeserializeObject<List<JObject>>(jsonFile);
                List<Dictionary<string, object>> dictionariesList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonFile);
                if (jobjectsList == null || dictionariesList == null)
                    return;
                ReadActionsFile(jobjectsList, dictionariesList);
            }
            catch (System.IO.FileNotFoundException)
            {
                Console.WriteLine("Error! File not found.\nFile name should be 'ActionsFile.json' and it should be located in SEWorkshop\\Website");
            }
            catch (TradingSystemException e)
            {
                Console.WriteLine(e.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        void ReadActionsFile(List<JObject> jobjectsList, List<Dictionary<string, object>> dictionariesList)
        {
            int index = -1;
            foreach (JObject action in jobjectsList)
            {
                index++;
                Dictionary<string, object> valuesDictionary = dictionariesList.ElementAt(index);
                if (!valuesDictionary.ElementAt(0).Key.Equals("command"))
                {
                    Console.WriteLine("Error! \"command\" property should be first");
                    continue;
                }
                switch (valuesDictionary.ElementAt(0).Value)
                {
                    case "Register":
                        HandleRegister(action, valuesDictionary);
                        break;
                    case "Login":
                        HandleLogin(action, valuesDictionary);
                        break;
                    case "Logout":
                        HandleLogout(action);
                        break;
                    case "AddProductToCart":
                        HandleAddProductToCart(action, valuesDictionary);
                        break;
                    case "RemoveProductFromCart":
                        HandleRemoveProductFromCart(action, valuesDictionary);
                        break;
                    case "Purchase":
                        HandlePurchase(action, valuesDictionary);
                        break;
                    case "OpenStore":
                        HandleOpenStore(action, valuesDictionary);
                        break;
                    case "WriteReview":
                        HandleWriteReview(action, valuesDictionary);
                        break;
                    case "WriteMessage":
                        HandleWriteMessage(action, valuesDictionary);
                        break;
                    case "AddProduct":
                        HandleAddProduct(action, valuesDictionary);
                        break;
                    case "EditProductName":
                        HandleEditProductName(action, valuesDictionary);
                        break;
                    case "EditProductCategory":
                        HandleEditProductCategory(action, valuesDictionary);
                        break;
                    case "EditProductDescription":
                        HandleEditProductDescription(action, valuesDictionary);
                        break;
                    case "EditProductPrice":
                        HandleEditProductPrice(action, valuesDictionary);
                        break;
                    case "EditProductQuantity":
                        HandleEditProductQuantity(action, valuesDictionary);
                        break;
                    case "RemoveProduct":
                        HandleRemoveProduct(action, valuesDictionary);
                        break;
                    case "AddAlwaysTruePolicy":
                        HandleAddAlwaysTruePolicy(action, valuesDictionary);
                        break;
                    case "AddSingleProductQuantityPolicy":
                        HandleAddSingleProductQuantityPolicy(action, valuesDictionary);
                        break;
                    case "AddSystemDayPolicy":
                        HandleAddSystemDayPolicy(action, valuesDictionary);
                        break;
                    case "AddUserCityPolicy":
                        HandleAddUserCityPolicy(action, valuesDictionary);
                        break;
                    case "AddUserCountryPolicy":
                        HandleAddUserCountryPolicy(action, valuesDictionary);
                        break;
                    case "AddWholeStoreQuantityPolicy":
                        HandleAddWholeStoreQuantityPolicy(action, valuesDictionary);
                        break;
                    case "RemovePolicy":
                        HandleRemovePolicy(action, valuesDictionary);
                        break;
                    case "AddProductCategoryDiscount":
                        HandleAddProductCategoryDiscount(action, valuesDictionary);
                        break;
                    case "AddSpecificProductDiscount":
                        HandleAddSpecificProductDiscount(action, valuesDictionary);
                        break;
                    case "AddBuySomeGetSomeDiscount":
                        HandleAddBuySomeGetSomeDiscount(action, valuesDictionary);
                        break;
                    case "AddBuyOverDiscount":
                        HandleAddBuyOverDiscount(action, valuesDictionary);
                        break;
                    case "RemoveDiscount":
                        HandleRemoveDiscount(action, valuesDictionary);
                        break;
                    case "AddStoreOwner":
                        HandleAddStoreOwner(action, valuesDictionary);
                        break;
                    case "AddStoreManager":
                        HandleAddStoreManager(action, valuesDictionary);
                        break;
                    case "SetPermissionsOfManager":
                        HandleSetPermissionsOfManager(action, valuesDictionary);
                        break;
                    case "RemovePermissionsOfManager":
                        HandleRemovePermissionsOfManager(action, valuesDictionary);
                        break;
                    case "RemoveStoreOwner":
                        HandleRemoveStoreOwner(action, valuesDictionary);
                        break;
                    case "RemoveStoreManager":
                        HandleRemoveStoreManager(action, valuesDictionary);
                        break;
                    default:
                        Console.WriteLine("Error! command name is illegal");
                        continue;
                }
            }
        }

        void HandleRegister(JObject action, Dictionary<string,object> properties)
        {
            string schemaJson = @"{
                'description': 'Register',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'username': { 'type': 'string'},
                    'password': { 'type': 'string'}
                },
                'required': [ 'command', 'username', 'password'],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! Register invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "username     type: string" +
                  "password     type: string" +
                  "Additional properties are not allowed.}");
                return;
            }
            string username = "", password = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("username"))
                    username = (string)property.Value;
                if (property.Key.Equals("password"))
                    password = (string)property.Value;
            }
            userManager.Register(DEF_SID, username, password);
        }

        void HandleLogin(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'Login',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'username': {'type':'string'},
                    'password': {'type':'string'}
                },
                'required': [ 'command', 'username', 'password'],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! Login invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "username     type: string" +
                  "password     type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string username = "", password = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("username"))
                    username = (string)property.Value;
                if (property.Key.Equals("password"))
                    password = (string)property.Value;
            }
            userManager.Login(DEF_SID, username, password);
        }

        void HandleLogout(JObject action)
        {
            string schemaJson = @"{
                'description': 'Logout',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'}
                },
                'required': [ 'command' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! Logout invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            userManager.Logout(DEF_SID);
        }

        void HandleAddProductToCart(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddProductToCart',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'},
                    'quantity': {'type':'integer'}
                },
                'required': [ 'command', 'storeName', 'productName', 'quantity'],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddProductToCart invalid." +
                 "Required properties:" +
                 "command      type: string" +
                 "storeName    type: string" +
                 "productName  type: string" +
                 "quantity     type: integer" +
                 "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            int quantity = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("quantity"))
                    quantity = (int)(long)property.Value;
            }
            userManager.AddProductToCart(DEF_SID, storeName, productName, quantity);
        }

        void HandleRemoveProductFromCart(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveProductFromCart',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string' },
                    'storeName': { 'type': 'string' },
                    'productName': {'type':'string' },
                    'quantity': {'type':'integer' }
                },
                'required': [ 'command', 'storeName', 'productName', 'quantity' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveProductFromCart invalid." +
                 "Required properties:" +
                 "command      type: string" +
                 "storeName    type: string" +
                 "productName  type: string" +
                 "quantity     type: integer" +
                 "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            int quantity = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("quantity"))
                    quantity = (int)(long)property.Value;
            }
            userManager.RemoveProductFromCart(DEF_SID, storeName, productName, quantity);
        }

        void HandlePurchase(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'Purchase',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'firstName': { 'type': 'string'},
                    'lastName' : { 'type': 'string'},
                    'id' : { 'type': 'string'},
                    'creditCardNumber': {'type':'string'},
                    'expirationMonth': {'type':'integer'},
                    'expirationYear': {'type':'integer'},
                    'cvv': {'type':'string'},
                    'city': {'type':'string'},
                    'street': {'type':'string'},
                    'houseNumber': {'type':'string'},
                    'country': {'type':'string'},
                    'zip': {'type':'string'},
                },
                'required': [ 'command', 'storeName', 'firstName', 'lastName', 'id', 'creditCardNumber', 'expirationMonth'
                              'expirationYear', 'cvv', 'city', 'street', 'houseNumber', 'country', 'zip' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! Purchase invalid." +
                 "Required properties:" +
                 "command           type: string" +
                 "storeName         type: string" +
                 "firstName         type: string" +
                 "lastName          type: string" +
                 "id                type: string" +
                 "creditCardNumber  type: string" +
                 "expirationMonth   type: integer" +
                 "expirationYear    type: integer" +
                 "cvv               type: string" +
                 "city              type: string" +
                 "street            type: string" +
                 "houseNumber       type: string" +
                 "country           type: string" +
                 "zip               type: string" +
                 "Additional properties are not allowed.");
                return;
            }
            string storeName = "", firstName = "", lastName = "", id = "", creditCardNumber = "", cvv = "", city = "", street = "", houseNumber = "", country = "", zip = "";
            int expirationMonth = -1, expirationYear = -1;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "firstName":
                        firstName = (string)property.Value;
                        break;
                    case "lastName":
                        lastName = (string)property.Value;
                        break;
                    case "id":
                        id = (string)property.Value;
                        break;
                    case "creditCardNumber":
                        creditCardNumber = (string)property.Value;
                        break;
                    case "expirationMonth":
                        expirationMonth = (int)property.Value;
                        break;
                    case "expirationYear":
                        expirationYear = (int)property.Value;
                        break;
                    case "cvv":
                        cvv = (string)property.Value;
                        break;
                    case "city":
                        city = (string)property.Value;
                        break;
                    case "street":
                        street = (string)property.Value;
                        break;
                    case "houseNumber":
                        houseNumber = (string)property.Value;
                        break;
                    case "country":
                        country = (string)property.Value;
                        break;
                    case "zip":
                        zip = (string)property.Value;
                        break;
                }
            }
            IEnumerable<DataBasket> baskets = userManager.MyCart(DEF_SID);
            Address address = new Address(country, city, street, houseNumber, zip);
            DateTime expirationDate = new DateTime(expirationYear, expirationMonth, 1);
            foreach (DataBasket basket in baskets)
                if (basket.Store.Name.Equals(storeName))
                {
                    userManager.Purchase(DEF_SID, basket, creditCardNumber, expirationDate, cvv, address, firstName+lastName, id);
                    return;
                }
            Console.WriteLine("Error! Purchase invalid." +
                "couldn't find a basket in the Store: " + storeName);
        }

        void HandleOpenStore(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'OpenStore',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! OpenStore invalid." +
                 "Required properties:" +
                 "command      type: string" +
                 "storeName    type: string" +
                 "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
            }
            userManager.OpenStore(DEF_SID, storeName);
        }

        void HandleWriteReview(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'WriteReview',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': { 'type':'string'},
                    'review': { 'type':'string'}
                },
                'required': [ 'command', 'storeName', 'productName', 'review' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! WriteReview invalid." +
                 "Required properties:" +
                 "command      type: string" +
                 "storeName    type: string" +
                 "productName  type: string" +
                 "review       type: string" +
                 "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "", review = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("review"))
                    review = (string)property.Value;
            }
            userManager.WriteReview(DEF_SID, storeName, productName, review);
        }

        void HandleWriteMessage(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'WriteMessage',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'message': { 'type':'string'}
                },
                'required': [ 'command', 'storeName', 'message' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! WriteReview invalid." +
                 "Required properties:" +
                 "command      type: string" +
                 "storeName    type: string" +
                 "productName  type: string" +
                 "message      type: string" +
                 "Additional properties are not allowed.");
                return;
            }
            string storeName = "", message = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("message"))
                    message = (string)property.Value;
            }
            userManager.WriteMessage(DEF_SID, storeName, message);
        }

        void HandleAddProduct(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddProduct',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'},
                    'description': {'type':'string'},
                    'category': {'type':'string'},
                    'price': {'type':'number'},
                    'quantity': {'type':'integer'}
                },
                'required': [ 'command', 'storeName', 'productName', 'description', 'category', 'price', 'quantity' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddProduct invalid." +
                "Required properties:" +
                "command      type: string" +
                "storeName    type: string" +
                "productName  type: string" +
                "description  type: string" +
                "category     type: string" +
                "price        type: number" +
                "quantity     type: integer" +
                "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "", description = "", category = "";
            double price = 0;
            int quantity = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("description"))
                    description = (string)property.Value;
                if (property.Key.Equals("category"))
                    category = (string)property.Value;
                if (property.Key.Equals("price"))
                    price = (double)property.Value;
                if (property.Key.Equals("quantity"))
                    quantity = (int)(long)property.Value;
            }
            userManager.AddProduct(DEF_SID, storeName, productName, description, category, price, quantity);
        }

        void HandleEditProductName(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductName',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'},
                    'newName': {'type':'string'}
                },
                'required': [ 'command', 'storeName', 'productName', 'newName' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductName invalid." +
               "Required properties:" +
               "command      type: string" +
               "storeName    type: string" +
               "productName  type: string" +
               "newName      type: string" +
               "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "", newName = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("newName"))
                    newName = (string)property.Value;
            }
            userManager.EditProductName(DEF_SID, storeName, productName, newName);
        }

        void HandleEditProductCategory(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductCategory',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'},
                    'newCategory': {'type':'string'}
                },
                'required': [ 'command', 'storeName', 'productName', 'newCategory' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductCategory invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "productName  type: string" +
                  "newCategory  type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "", newCategory = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("newCategory"))
                    newCategory = (string)property.Value;
            }
            userManager.EditProductCategory(DEF_SID, storeName, productName, newCategory);
        }

        void HandleEditProductDescription(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductDescription',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': { 'type': 'string'},
                    'newDescription': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'productName', 'newDescription' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductDescription invalid." +
                  "Required properties:" +
                  "command         type: string" +
                  "storeName       type: string" +
                  "productName     type: string" +
                  "newDescription  type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "", newDescription = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("newDescription"))
                    newDescription = (string)property.Value;
            }
            userManager.EditProductDescription(DEF_SID, storeName, productName, newDescription);
        }

        void HandleEditProductPrice(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductPrice',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'},
                    'newPrice': {'type':'number'}
                },
                'required': [ 'command', 'storeName', 'productName', 'newDescription' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductPrice invalid." +
                  "Required properties:" +
                  "command         type: string" +
                  "storeName       type: string" +
                  "productName     type: string" +
                  "newPrice        type: number" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            double newPrice = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("newPrice"))
                    newPrice = (double)property.Value;
            }
            userManager.EditProductPrice(DEF_SID, storeName, productName, newPrice);
        }

        void HandleEditProductQuantity(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductQuantity',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'},
                    'newQuantity': {'type':'integer'}
                },
                'required': [ 'command', 'storeName', 'productName', 'newQuantity' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductQuantity invalid." +
                  "Required properties:" +
                  "command         type: string" +
                  "storeName       type: string" +
                  "productName     type: string" +
                  "newQuantity     type: integer" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            int newQuantity = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("newQuantity"))
                    newQuantity = (int)(long)property.Value;
            }
            userManager.EditProductQuantity(DEF_SID, storeName, productName, newQuantity);
        }

        void HandleRemoveProduct(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveProduct',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': {'type':'string'}
                },
                'required': [ 'command', 'storeName', 'productName' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {

                Console.WriteLine("Error! RemoveProduct invalid." +
                  "Required properties:" +
                  "command         type: string" +
                  "storeName       type: string" +
                  "productName     type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
            }
            userManager.RemoveProduct(DEF_SID, storeName, productName);
        }

        void HandleAddAlwaysTruePolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddAlwaysTruePolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']}
                },
                'required': [ 'command', 'storeName', 'operator' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddAlwaysTruePolicy invalid." +
                   "Required properties:" +
                   "command      type: string" +
                   "storeName    type: string" +
                   "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                   "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            Operator op = Operator.And;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("operator"))
                    op = StringToOperator((string)property.Value);
            }
            userManager.AddAlwaysTruePolicy(DEF_SID, storeName, op);
        }

        void HandleAddSingleProductQuantityPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddSingleProductQuantityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': { 'type': 'string'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'minQuantity': { 'type': 'integer'},
                    'maxQuantity': { 'type': 'integer'}
                },
                'required': [ 'command', 'storeName', 'productName', 'operator', 'minQuantity', 'maxQuantity' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddSingleProductQuantityPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "productName  type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "minQuantity  type: integer" +
                  "maxQuantity  type: integer" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            string productName = "";
            Operator op = Operator.And;
            int minQuantity = 0;
            int maxQuantity = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
                if (property.Key.Equals("operator"))
                    op = StringToOperator((string)property.Value);
                if (property.Key.Equals("minQuantity"))
                    minQuantity = (int)(long)property.Value;
                if (property.Key.Equals("maxQuantity"))
                    maxQuantity = (int)(long)property.Value;
            }
            userManager.AddSingleProductQuantityPolicy(DEF_SID, storeName, op, productName, minQuantity, maxQuantity);
        }

        void HandleAddSystemDayPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddSystemDayPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'dayOfWeek': {'type': 'string', 'enum': ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']}
                },
                'required': [ 'command', 'storeName', 'operator', 'dayOfWeek' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddSystemDayPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "dayOfWeek    type: enum ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            Operator op = Operator.And;
            Enums.Weekday dayOfWeek = Enums.Weekday.Sunday;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "operator":
                        op = StringToOperator((string)property.Value);
                        break;
                    case "dayOfWeek":
                        dayOfWeek = StringToDayOfWeek((string)property.Value);
                        break;
                }
            }
            userManager.AddSystemDayPolicy(DEF_SID, storeName, op, dayOfWeek);
        }

        void HandleAddUserCityPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddUserCityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string' },
                    'storeName': { 'type': 'string'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'city': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'operator', 'city' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddUserCityPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "city         type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", city = "";
            Operator op = Operator.And;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("city"))
                    city = (string)property.Value;
                if (property.Key.Equals("operator"))
                    op = StringToOperator((string)property.Value);
            }
            userManager.AddUserCityPolicy(DEF_SID, storeName, op, city);
        }

        void HandleAddUserCountryPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddUserCountryPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'country': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'operator', 'country' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddUserCountryPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "country      type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", country = "";
            Operator op = Operator.And;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("country"))
                    country = (string)property.Value;
                if (property.Key.Equals("operator"))
                    op = StringToOperator((string)property.Value);
            }
            userManager.AddUserCountryPolicy(DEF_SID, storeName, op, country);
        }

        void HandleAddWholeStoreQuantityPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddWholeStoreQuantityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'minQuantity': { 'type': 'integer'},
                    'maxQuantity': { 'type': 'integer'}
                },
                'required': [ 'command', 'storeName', 'operator', 'minQuantity', 'maxQuantity' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddWholeStoreQuantityPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "minQuantity  type: integer" +
                  "maxQuantity  type: integer" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            Operator op = Operator.And;
            int minQuantity = 0, maxQuantity = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("operator"))
                    op = StringToOperator((string)property.Value);
                if (property.Key.Equals("minQuantity"))
                    minQuantity = (int)(long)property.Value;
                if (property.Key.Equals("maxQuantity"))
                    maxQuantity = (int)(long)property.Value;
            }
            userManager.AddWholeStoreQuantityPolicy(DEF_SID, storeName, op, minQuantity, maxQuantity);
        }

        void HandleRemovePolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemovePolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'indexInChain': { 'type': 'integer'}
                },
                'required': [ 'command', 'storeName', 'indexInChain' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemovePolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "indexInChain type: integer" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            int indexInChain = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("indexInChain"))
                    indexInChain = (int)(long)property.Value;
            }
            userManager.RemovePolicy(DEF_SID, storeName, indexInChain);
        }

        void HandleAddProductCategoryDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddProductCategoryDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'categoryName': { 'type': 'string' },
                    'deadline': { 'type': 'string', 'format': 'date-time'},
                    'percentage': { 'type': 'number' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'] },
                    'indexInChain': { 'type': 'integer'},
                    'discountId': { 'type': 'integer'},
                    'toLeft': {'type': 'string', 'enum': ['True', 'False']}
                },
                'required': [ 'command', 'storeName', 'categoryName', 'deadline', 'percentage', 'operator', 'indexInChain',
                'discountId', 'toLeft'],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddProductCategoryDiscount invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "categoryName type: string" +
                  "deadline     type: string    format:'date-time'  example: 2020-03-19T07:22Z" +
                  "percentage   type: number" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "indexInChain type: integer" +
                  "discountId   type: integer" +
                  "toLeft       type: enum ['True', 'False']" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", categoryName = "";
            DateTime deadline = new DateTime();
            double percentage = 0;
            Operator op = Operator.And;
            int indexInChain = 0, discountId = 0;
            bool toLeft = false;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "categoryName":
                        categoryName = (string)property.Value;
                        break;
                    case "deadline":
                        deadline = DateTime.Parse((string)property.Value);
                        break;
                    case "percentage":
                        percentage = (double)property.Value;
                        break;
                    case "operator":
                        op = StringToOperator((string)property.Value);
                        break;
                    case "indexInChain":
                        indexInChain = (int)(long)property.Value;
                        break;
                    case "discountId":
                        discountId = (int)(long)property.Value;
                        break;
                    case "toLeft":
                        toLeft = bool.Parse((string)property.Value);
                        break;
                }
            }
            userManager.AddProductCategoryDiscount(DEF_SID, storeName, categoryName, deadline, percentage, op, indexInChain, discountId, toLeft);
        }

        void HandleAddSpecificProductDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddSpecificProductDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string' },
                    'storeName': { 'type': 'string' },
                    'productName': { 'type': 'string' },
                    'deadline': { 'type': 'string', 'format': 'date-time' },
                    'percentage': { 'type': 'number' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'] },
                    'indexInChain': { 'type': 'integer' },
                    'discountId': { 'type': 'integer' },
                    'toLeft': {'type': 'string', 'enum': ['True', 'False'] }
                },
                'required': [ 'command', 'storeName', 'productName', 'deadline', 'percentage', 'operator', 'indexInChain',
                              'discountId', 'toLeft'],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddSpecificProductDiscount invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "productName  type: string" +
                  "deadline     type: string    format:'date-time'  example: 2020-03-19T07:22Z" +
                  "percentage   type: number" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "indexInChain type: integer" +
                  "discountId   type: integer" +
                  "toLeft       type: enum ['True', 'False']" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            DateTime deadline = new DateTime();
            double percentage = 0;
            Operator op = Operator.And;
            int indexInChain = 0, discountId = 0;
            bool toLeft = false;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "productName":
                        productName = (string)property.Value;
                        break;
                    case "deadline":
                        deadline = DateTime.Parse((string)property.Value);
                        break;
                    case "percentage":
                        percentage = (double)property.Value;
                        break;
                    case "operator":
                        op = StringToOperator((string)property.Value);
                        break;
                    case "indexInChain":
                        indexInChain = (int)(long)property.Value;
                        break;
                    case "discountId":
                        discountId = (int)(long)property.Value;
                        break;
                    case "toLeft":
                        toLeft = bool.Parse((string)property.Value);
                        break;
                }
            }
            userManager.AddSpecificProductDiscount(DEF_SID, storeName, productName, deadline, percentage, op, indexInChain, discountId, toLeft);
        }

        void HandleAddBuySomeGetSomeDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddBuySomeGetSomeDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'deadline': { 'type': 'string', 'format': 'date-time'},
                    'percentage': { 'type': 'number'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'indexInChain': { 'type': 'integer'},
                    'discountId': { 'type': 'integer'},
                    'toLeft': {'type': 'string', 'enum': ['True', 'False']},
                    'conditionProductName': { 'type': 'string'},
                    'underDiscountProductName': { 'type': 'string'},
                    'buySome': { 'type': 'integer'},
                    'getSome': { 'type': 'integer'}
                },
                'required': [ 'command', 'storeName', 'deadline', 'percentage', 'operator', 'indexInChain', 'discountId', 'toLeft', 'conditionProductName', 'underDiscountProductName', 'buySome', 'getSome' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddBuySomeGetSomeDiscount invalid." +
                  "Required properties:" +
                  "command                      type: string" +
                  "storeName                    type: string" +
                  "deadline                     type: string    format:'date-time'  example: 2020-03-19T07:22Z" +
                  "percentage                   type: number" +
                  "operator                     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "indexInChain                 type: integer" +
                  "discountId                   type: integer" +
                  "toLeft                       type: enum ['True', 'False']" +
                  "conditionProductName         type: string" +
                  "underDiscountProductName     type: string" +
                  "buySome                      type: integer" +
                  "getSome                      type: integer" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", conditionProductName = "", underDiscountProductName = "";
            DateTime deadline = new DateTime();
            double percentage = 0;
            Operator op = Operator.And;
            int indexInChain = 0, discountId = 0, buySome = 0, getSome = 0;
            bool toLeft = false;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "deadline":
                        deadline = DateTime.Parse((string)property.Value);
                        break;
                    case "percentage":
                        percentage = (double)property.Value;
                        break;
                    case "operator":
                        op = StringToOperator((string)property.Value);
                        break;
                    case "indexInChain":
                        indexInChain = (int)(long)property.Value;
                        break;
                    case "discountId":
                        discountId = (int)(long)property.Value;
                        break;
                    case "toLeft":
                        toLeft = bool.Parse((string)property.Value);
                        break;
                    case "conditionProductName":
                        conditionProductName = (string)property.Value;
                        break;
                    case "underDiscountProductName":
                        conditionProductName = (string)property.Value;
                        break;
                    case "buySome":
                        buySome = (int)(long)property.Value;
                        break;
                    case "getSome":
                        getSome = (int)(long)property.Value;
                        break;
                }
            }
            userManager.AddBuySomeGetSomeDiscount(buySome, getSome, DEF_SID, conditionProductName, underDiscountProductName,
                storeName, deadline, percentage, op, indexInChain, discountId, toLeft);
        }

        void HandleAddBuyOverDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddBuyOverDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'productName': { 'type': 'string' },
                    'deadline': { 'type': 'string', 'format': 'date-time'},
                    'percentage': { 'type': 'number' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies']},
                    'indexInChain': { 'type': 'integer' },
                    'discountId': { 'type': 'integer' },
                    'toLeft': {'type': 'string', 'enum': ['True', 'False']},
                    'minSum' : { 'type': 'number' }
                },
                'required': [ 'command', 'storeName', 'productName', 'deadline', 'percentage', 'operator', 'discountId', 'indexInChain', 'discountId', 'toLeft', 'minSum' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddBuyOverDiscount invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "productName  type: string" +
                  "deadline     type: string    format:'date-time'  example: 2020-03-19T07:22Z" +
                  "percentage   type: number" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "indexInChain type: integer" +
                  "discountId   type: integer" +
                  "toLeft       type: enum ['True', 'False']" +
                  "minSum       type: number" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", productName = "";
            DateTime deadline = new DateTime();
            double percentage = 0;
            Operator op = Operator.And;
            int indexInChain = 0, discountId = 0, minSum = 0;
            bool toLeft = false;
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "productName":
                        productName = (string)property.Value;
                        break;
                    case "deadline":
                        deadline = DateTime.Parse((string)property.Value);
                        break;
                    case "percentage":
                        percentage = (double)property.Value;
                        break;
                    case "operator":
                        op = StringToOperator((string)property.Value);
                        break;
                    case "indexInChain":
                        indexInChain = (int)(long)property.Value;
                        break;
                    case "discountId":
                        discountId = (int)(long)property.Value;
                        break;
                    case "toLeft":
                        toLeft = bool.Parse((string)property.Value);
                        break;
                    case "minSum":
                        minSum = (int)(long)property.Value;
                        break;
                }
            }
            userManager.AddBuyOverDiscount(minSum, DEF_SID, storeName, productName, deadline, percentage, op, indexInChain, discountId, toLeft);
        }

        void HandleRemoveDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'indexInChain': { 'type': 'integer'}
                },
                'required': [ 'command', 'storeName', 'indexInChain' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveDiscount invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "indexInChain type: integer" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "";
            int indexInChain = 0;
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("indexInChain"))
                    indexInChain = (int)(long)property.Value;
            }
            userManager.RemoveDiscount(DEF_SID, storeName, indexInChain);
        }

        void HandleAddStoreOwner(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddStoreOwner',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'username': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'username' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddStoreOwner invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "username     type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", username = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "username":
                        username = (string)property.Value;
                        break;
                }
            }
            userManager.AddStoreOwner(DEF_SID, storeName, username);
        }

        void HandleAddStoreManager(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddStoreManager',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'username': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'username' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddStoreManager invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "username     type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", username = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "username":
                        username = (string)property.Value;
                        break;
                }
            }
            userManager.AddStoreManager(DEF_SID, storeName, username);
        }

        void HandleSetPermissionsOfManager(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'SetPermissionsOfManager',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'username': { 'type': 'string'},
                    'authorization': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'username', 'authorization' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! SetPermissionsOfManager invalid." +
                  "Required properties:" +
                  "command       type: string" +
                  "storeName     type: string" +
                  "username      type: string" +
                  "authorization type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", username = "", authorization = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "username":
                        username = (string)property.Value;
                        break;
                    case "authorization":
                        authorization = (string)property.Value;
                        break;
                }
            }
            userManager.SetPermissionsOfManager(DEF_SID, storeName, username, authorization);
        }

        void HandleRemovePermissionsOfManager(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemovePermissionsOfManager',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'username': { 'type': 'string'},
                    'authorization': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'username', 'authorization' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemovePermissionsOfManager invalid." +
                  "Required properties:" +
                  "command       type: string" +
                  "storeName     type: string" +
                  "username      type: string" +
                  "authorization type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", username = "", authorization = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "username":
                        username = (string)property.Value;
                        break;
                    case "authorization":
                        authorization = (string)property.Value;
                        break;
                }
            }
            userManager.RemovePermissionsOfManager(DEF_SID, storeName, username, authorization);
        }

        void HandleRemoveStoreOwner(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveStoreOwner',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'username': { 'type': 'string'}
                },
                'required': [ 'command', 'storeName', 'username' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveStoreOwner invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "username     type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", username = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "username":
                        username = (string)property.Value;
                        break;
                }
            }
            userManager.RemoveStoreOwner(DEF_SID, storeName, username);
        }

        void HandleRemoveStoreManager(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveStoreManager',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string'},
                    'storeName': { 'type': 'string'},
                    'username': { 'type': 'string' }
                },
                'required': [ 'command', 'storeName', 'username' ],
                'additionalProperties': false
            }";
            JSchema schema = JSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveStoreManager invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "username     type: string" +
                  "Additional properties are not allowed.");
                return;
            }
            string storeName = "", username = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "username":
                        username = (string)property.Value;
                        break;
                }
            }
            userManager.RemoveStoreManager(DEF_SID, storeName, username);
        }

        Operator StringToOperator(string op)
        {
            return op switch
            {
                "And" => Operator.And,
                "Or" => Operator.Or,
                "Xor" => Operator.Xor,
                "Implies" => Operator.Implies,
                _ => Operator.And,
            };
        }

        Enums.Weekday StringToDayOfWeek(string day)
        {
            return day switch
            {
                "Sunday" => Enums.Weekday.Sunday,
                "Monday" => Enums.Weekday.Monday,
                "Tuesday" => Enums.Weekday.Tuesday,
                "Wednesday" => Enums.Weekday.Wednesday,
                "Thursday" => Enums.Weekday.Thursday,
                "Friday" => Enums.Weekday.Friday,
                "Saturday" => Enums.Weekday.Saturday,
                _ => Enums.Weekday.Sunday,
            };
        }
    }
}
