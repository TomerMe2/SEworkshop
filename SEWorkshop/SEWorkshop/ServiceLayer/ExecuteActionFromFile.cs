using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using SEWorkshop.Models;
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

        [Obsolete]
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
        }

        // action line format: <ActionName>,<Arg1>,<Arg2>...
        [Obsolete]
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
                        HandleLogout(action, valuesDictionary);
                        break;
                    case "AddProductToCart":
                        HandleAddProductToCart(action, valuesDictionary);
                        break;
                    case "RemoveProductFromCart":
                        HandleRemoveProductFromCart(action, valuesDictionary);
                        break;
                    case "Purchase":
                        // action line format: <ActionName>,<StoreName>,<creditCardNumber>,<City>,<Street>,<HouseNumber>,<Country>
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
                    /*
                    case "AddStoreOwner":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.AddStoreOwner(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        break;
                    case "AddStoreManager":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.AddStoreManager(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        break;
                    case "SetPermissionsOfManager":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                            userManager.SetPermissionsOfManager(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3]);
                        break;
                    case "RemovePermissionsOfManager":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                            userManager.RemovePermissionsOfManager(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3]);
                        break;
                    case "RemoveStoreOwner":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.RemoveStoreOwner(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        break;
                    case "RemoveStoreManager":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.RemoveStoreManager(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        break;
                    */
                    default:
                        Console.WriteLine("Error! command name is illegal");
                        continue;
                }
            }
        }

        [Obsolete]
        void HandleRegister(JObject action, Dictionary<string,object> properties)
        {
            string schemaJson = @"{
                'description': 'Register',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'username': {'type':'string', 'required': 'true'},
                    'password': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (action.IsValid(schema))
            {
                string username = "";
                string password = "";
                foreach (var property in properties)
                {
                    if (property.Key.Equals("username"))
                        username = (string)property.Value;
                    if (property.Key.Equals("password"))
                        password = (string)property.Value;
                }
                userManager.Register(DEF_SID, username, password);
            }
            else
            {
                Console.WriteLine("Error! Register invalid. required properties: { \"command\":_, \"username\":_, \"password\":_ }");
            }
        }

        [Obsolete]
        void HandleLogin(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'Login',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'username': {'type':'string', 'required': 'true'},
                    'password': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (action.IsValid(schema))
            {
                string username = "";
                string password = "";
                foreach (var property in properties)
                {
                    if (property.Key.Equals("username"))
                        username = (string)property.Value;
                    if (property.Key.Equals("password"))
                        password = (string)property.Value;
                }
                userManager.Login(DEF_SID, username, password);
            }
            else
            {
                Console.WriteLine("Error! Login invalid. required properties: { \"command\":_, \"username\":_, \"password\":_ }");
            }
        }

        [Obsolete]
        void HandleLogout(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'Logout',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (action.IsValid(schema))
            {
                userManager.Logout(DEF_SID);
            }
            else
            {
                Console.WriteLine("Error! Logout invalid. There are no properties in this command");
            }
        }

        [Obsolete]
        void HandleAddProductToCart(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddProductToCart',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'quantity': {'type':'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddProductToCart invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"quantity\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
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

        [Obsolete]
        void HandleRemoveProductFromCart(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveProductFromCart',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'quantity': {'type':'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveProductFromCart invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"quantity\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
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

        [Obsolete]
        void HandlePurchase(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'Purchase',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'creditCardNumber': {'type':'string', 'required': 'true'},
                    'city': {'type':'string', 'required': 'true'},
                    'street': {'type':'string', 'required': 'true'},
                    'houseNumber': {'type':'string', 'required': 'true'},
                    'country': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! Purchase invalid. required properties: { \"command\":_, \"storeName\":_, \"creditCardNumber\":_, \"city\":_, \"street\":_, \"houseNumber\":_, \"country\":_ }");
                return;
            }
            string storeName = "", creditCardNumber = "", city = "", street = "", houseNumber = "", country = "";
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case "storeName":
                        storeName = (string)property.Value;
                        break;
                    case "creditCardNumber":
                        creditCardNumber = (string)property.Value;
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
                }
            }
            IEnumerable<DataBasket> baskets = userManager.MyCart(DEF_SID);
            Address address = new Address(country, city, street, houseNumber);
            foreach (DataBasket basket in baskets)
                if (basket.Store.Name.Equals(storeName))
                {
                    userManager.Purchase(DEF_SID, basket, creditCardNumber, address);
                    return;
                }
            Console.WriteLine("Purchase invalid, couldn't find a basket in the Store: " + storeName);
        }

        [Obsolete]
        void HandleOpenStore(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'OpenStore',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! OpenStore invalid. required properties: { \"command\":_, \"storeName\":_ }");
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

        [Obsolete]
        void HandleWriteReview(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'WriteReview',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'review': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! WriteReview invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"review\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
            string review = "";
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

        [Obsolete]
        void HandleWriteMessage(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'WriteReview',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'message': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! WriteReview invalid. required properties: { \"command\":_, \"storeName\":_, \"message\":_ }");
                return;
            }
            string storeName = "";
            string message = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("message"))
                    message = (string)property.Value;
            }
            userManager.WriteMessage(DEF_SID, storeName, message);
        }

        [Obsolete]
        void HandleAddProduct(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddProduct',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'description': {'type':'string', 'required': 'true'},
                    'category': {'type':'string', 'required': 'true'},
                    'price': {'type':'number', 'required': 'true'},
                    'quantity': {'type':'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddProduct invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"description\":_, \"category\":_, \"price\":_, \"quantity\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
            string description = "";
            string category = "";
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

        [Obsolete]
        void HandleEditProductName(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductName',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'newName': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductName invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"newName\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
            string newName = "";
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

        [Obsolete]
        void HandleEditProductCategory(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductCategory',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'newCategory': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductCategory invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"newCategory\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
            string newCategory = "";
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

        [Obsolete]
        void HandleEditProductDescription(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductDescription',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'newDescription': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductDescription invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"newDescription\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
            string newDescription = "";
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

        [Obsolete]
        void HandleEditProductPrice(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductPrice',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'newPrice': {'type':'number', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductPrice invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"newPrice\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
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

        [Obsolete]
        void HandleEditProductQuantity(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'EditProductQuantity',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'},
                    'newQuantity': {'type':'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! EditProductQuantity invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_, \"newQuantity\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
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

        [Obsolete]
        void HandleRemoveProduct(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveProduct',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': {'type':'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveProduct invalid. required properties: { \"command\":_, \"storeName\":_, \"productName\":_ }");
                return;
            }
            string storeName = "";
            string productName = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("productName"))
                    productName = (string)property.Value;
            }
            userManager.RemoveProduct(DEF_SID, storeName, productName);
        }

        [Obsolete]
        void HandleAddAlwaysTruePolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddAlwaysTruePolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddAlwaysTruePolicy invalid." +
                   "Required properties:" +
                   "command      type: string" +
                   "storeName    type: string" +
                   "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                   "Additional properties are not allowed.}");
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

        [Obsolete]
        void HandleAddSingleProductQuantityPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddSingleProductQuantityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': { 'type': 'string', 'required': 'true'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'minQuantity': { 'type': 'integer', 'required': 'true'},
                    'minQuantity': { 'type': 'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
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
                  "Additional properties are not allowed.}");
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

        [Obsolete]
        void HandleAddSystemDayPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddSystemDayPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'dayOfWeek': {'type': 'string', 'enum': ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'], 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddSystemDayPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "dayOfWeek    type: enum ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday']" +
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            Operator op = Operator.And;
            DayOfWeek dayOfWeek = DayOfWeek.Sunday;
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

        [Obsolete]
        void HandleAddUserCityPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddUserCityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'city': { 'type': 'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddUserCityPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "city         type: string" +
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            string city = "";
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

        [Obsolete]
        void HandleAddUserCountryPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddUserCountryPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'country': { 'type': 'string', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddUserCountryPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "country      type: string" +
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            string country = "";
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

        [Obsolete]
        void HandleAddWholeStoreQuantityPolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddWholeStoreQuantityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'minQuantity': { 'type': 'integer', 'required': 'true'},
                    'maxQuantity': { 'type': 'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddWholeStoreQuantityPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "operator     type: enum ['And', 'Or', 'Xor', 'Implies']" +
                  "minQuantity  type: integer" +
                  "maxQuantity  type: integer" +
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            Operator op = Operator.And;
            int minQuantity = 0;
            int maxQuantity = 0;
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

        [Obsolete]
        void HandleRemovePolicy(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddWholeStoreQuantityPolicy',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'indexInChain': { 'type': 'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! AddWholeStoreQuantityPolicy invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "indexInChain type: integer" +
                  "Additional properties are not allowed.}");
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

        [Obsolete]
        void HandleAddProductCategoryDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddProductCategoryDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'categoryName': { 'type': 'string', 'required': 'true' },
                    'deadline': { 'type': 'string', 'required': 'true', 'format': 'date-time'},
                    'percentage': { 'type': 'number', 'required': 'true' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'indexInChain': { 'type': 'integer', 'required': 'true' },
                    'discountId': { 'type': 'integer', 'required': 'true' },
                    'toLeft': {'type': 'string', 'enum': ['True', 'False'], 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
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
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            string categoryName = "";
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

        [Obsolete]
        void HandleAddSpecificProductDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddSpecificProductDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': { 'type': 'string', 'required': 'true' },
                    'deadline': { 'type': 'string', 'required': 'true', 'format': 'date-time'},
                    'percentage': { 'type': 'number', 'required': 'true' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'indexInChain': { 'type': 'integer', 'required': 'true' },
                    'discountId': { 'type': 'integer', 'required': 'true' },
                    'toLeft': {'type': 'string', 'enum': ['True', 'False'], 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
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
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            string productName = "";
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

        [Obsolete]
        void HandleAddBuySomeGetSomeDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddBuySomeGetSomeDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'deadline': { 'type': 'string', 'required': 'true', 'format': 'date-time'},
                    'percentage': { 'type': 'number', 'required': 'true' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'indexInChain': { 'type': 'integer', 'required': 'true' },
                    'discountId': { 'type': 'integer', 'required': 'true' },
                    'toLeft': {'type': 'string', 'enum': ['True', 'False'], 'required': 'true'},
                    'conditionProductName': { 'type': 'string', 'required': 'true'},
                    'underDiscountProductName': { 'type': 'string', 'required': 'true'},
                    'buySome': { 'type': 'integer', 'required': 'true' },
                    'getSome': { 'type': 'integer', 'required': 'true' }
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
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
                  "Additional properties are not allowed.}");
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

        [Obsolete]
        void HandleAddBuyOverDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'AddBuyOverDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'productName': { 'type': 'string', 'required': 'true' },
                    'deadline': { 'type': 'string', 'required': 'true', 'format': 'date-time'},
                    'percentage': { 'type': 'number', 'required': 'true' },
                    'operator': {'type': 'string', 'enum': ['And', 'Or', 'Xor', 'Implies'], 'required': 'true'},
                    'indexInChain': { 'type': 'integer', 'required': 'true' },
                    'discountId': { 'type': 'integer', 'required': 'true' },
                    'toLeft': {'type': 'string', 'enum': ['True', 'False'], 'required': 'true'},
                    'minSum' : { 'type': 'number', 'required': 'true' }
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
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
                  "Additional properties are not allowed.}");
                return;
            }
            string storeName = "";
            string productName = "";
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
                        minSum = (double)property.Value;
                        break;
                }
            }
            userManager.AddBuyOverDiscount(minSum, DEF_SID, storeName, productName, deadline, percentage, op, indexInChain, discountId, toLeft);
        }

        [Obsolete]
        void HandleRemoveDiscount(JObject action, Dictionary<string, object> properties)
        {
            string schemaJson = @"{
                'description': 'RemoveDiscount',
                'type': 'object',
                'properties': {
                    'command': { 'type': 'string', 'required': 'true'},
                    'storeName': { 'type': 'string', 'required': 'true'},
                    'indexInChain': { 'type': 'integer', 'required': 'true'}
                },
                'additionalProperties': false
            }";
            JsonSchema schema = JsonSchema.Parse(schemaJson);
            if (!action.IsValid(schema))
            {
                Console.WriteLine("Error! RemoveDiscount invalid." +
                  "Required properties:" +
                  "command      type: string" +
                  "storeName    type: string" +
                  "indexInChain type: integer" +
                  "Additional properties are not allowed.}");
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

        bool CheckArgs(string[] args)
        {
            foreach (string arg in args)
                if (arg.Length <= 0)
                    return false;
            return true;
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

        DayOfWeek StringToDayOfWeek(string day)
        {
            return day switch
            {
                "Sunday" => DayOfWeek.Sunday,
                "Monday" => DayOfWeek.Monday,
                "Tuesday" => DayOfWeek.Tuesday,
                "Wednesday" => DayOfWeek.Wednesday,
                "Thursday" => DayOfWeek.Thursday,
                "Friday" => DayOfWeek.Friday,
                "Saturday" => DayOfWeek.Saturday,
                _ => DayOfWeek.Sunday,
            };
        }
    }
}
