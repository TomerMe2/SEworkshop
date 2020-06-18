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
        private IUserManager userManager;
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
                    /*
                    case "RemoveProduct":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.RemoveProduct(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        break;
                    case "AddAlwaysTruePolicy":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[2]);
                            if (op != null)
                                userManager.AddAlwaysTruePolicy(DEF_SID, actionLineSplited[1], (Operator)op);
                        }
                        break;
                    case "AddSingleProductQuantityPolicy":
                        if (actionLineSplited.Length == 6 && CheckArgs(actionLineSplited))
                        {
                            Operator? op1 = StringToOperator(actionLineSplited[2]);
                            if (op1 != null && int.TryParse(actionLineSplited[4], out int minQuantity1) && int.TryParse(actionLineSplited[5], out int maxQuantity1))
                                userManager.AddSingleProductQuantityPolicy(DEF_SID, actionLineSplited[1], (Operator)op1, actionLineSplited[3], minQuantity1, maxQuantity1);
                        }
                        break;
                    case "AddSystemDayPolicy":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[2]);
                            DayOfWeek? day = StringToDayOfWeek(actionLineSplited[3]);
                            if (op != null && day != null)
                                userManager.AddSystemDayPolicy(DEF_SID, actionLineSplited[1], (Operator)op, (DayOfWeek)day);
                        }
                        break;
                    case "AddUserCityPolicy":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[2]);
                            if (op != null)
                                userManager.AddUserCityPolicy(DEF_SID, actionLineSplited[1], (Operator)op, actionLineSplited[3]);
                        }
                        break;
                    case "AddUserCountryPolicy":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[2]);
                            if (op != null)
                                userManager.AddUserCountryPolicy(DEF_SID, actionLineSplited[1], (Operator)op, actionLineSplited[3]);
                        }
                        break;
                    case "AddWholeStoreQuantityPolicy":
                        if (actionLineSplited.Length == 5 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[2]);
                            if (op != null && int.TryParse(actionLineSplited[4], out int minQuantity)
                                && int.TryParse(actionLineSplited[5], out int maxQuantity))
                                userManager.AddWholeStoreQuantityPolicy(DEF_SID, actionLineSplited[1], (Operator)op, minQuantity, maxQuantity);
                        }
                        break;
                    case "RemovePolicy":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                        {
                            if (int.TryParse(actionLineSplited[2], out int indexInChain1))
                                userManager.RemovePolicy(DEF_SID, actionLineSplited[1], indexInChain1);
                        }
                        break;
                    case "AddProductCategoryDiscount":
                        if (actionLineSplited.Length == 9 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[5]);
                            DateTime deadline = DateTime.Parse(actionLineSplited[3]);
                            bool toLeft = bool.Parse(actionLineSplited[8]);
                            if (op != null
                                && double.TryParse(actionLineSplited[4], out double percentage)
                                && int.TryParse(actionLineSplited[6], out int indexInChain2)
                                && int.TryParse(actionLineSplited[7], out int disId))
                                userManager.AddProductCategoryDiscount(DEF_SID, actionLineSplited[1], actionLineSplited[2],
                                    deadline, percentage, (Operator)op, indexInChain2, disId, toLeft);
                        }
                        break;
                    case "AddSpecificProductDiscount":
                        if (actionLineSplited.Length == 9 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[5]);
                            DateTime deadline = DateTime.Parse(actionLineSplited[3]);
                            bool toLeft = bool.Parse(actionLineSplited[8]);
                            if (op != null
                                && double.TryParse(actionLineSplited[4], out double percentage)
                                && int.TryParse(actionLineSplited[6], out int indexInChain3)
                                && int.TryParse(actionLineSplited[7], out int disId))
                                userManager.AddSpecificProductDiscount(DEF_SID, actionLineSplited[1], actionLineSplited[2],
                                    deadline, percentage, (Operator)op, indexInChain3, disId, toLeft);
                        }
                        break;
                    case "AddBuySomeGetSomeDiscount":
                        // <actionName>,<storeName>,<deadline>,<percentae>,<op>,<indexInChain>
                        // ,<disId>,<toLeft>,<conditionProdName>,<underDiscountProdName>,<buySome>,<getSome>
                        if (actionLineSplited.Length == 12 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[5]);
                            DateTime deadline = DateTime.Parse(actionLineSplited[2]);
                            bool toLeft = bool.Parse(actionLineSplited[8]);
                            if (op != null
                                && double.TryParse(actionLineSplited[3], out double percentage)
                                && int.TryParse(actionLineSplited[5], out int indexInChain4)
                                && int.TryParse(actionLineSplited[6], out int disId)
                                && int.TryParse(actionLineSplited[10], out int buySome)
                                && int.TryParse(actionLineSplited[11], out int getSome))
                                userManager.AddBuySomeGetSomeDiscount(buySome, getSome, DEF_SID, actionLineSplited[8], actionLineSplited[9],
                                    actionLineSplited[1], deadline, percentage, (Operator)op, indexInChain4, disId, toLeft);
                        }
                        break;
                    case "AddBuyOverDiscount":
                        // <actionName>,<storeName>,<productName>,<deadline>,<percentage>,<op>
                        // ,<indexInChain>,<disId>,<toLeft>,<minSum>
                        if (actionLineSplited.Length == 10 && CheckArgs(actionLineSplited))
                        {
                            Operator? op = StringToOperator(actionLineSplited[5]);
                            DateTime deadline = DateTime.Parse(actionLineSplited[2]);
                            bool toLeft = bool.Parse(actionLineSplited[8]);
                            if (op != null
                                && double.TryParse(actionLineSplited[3], out double percentage)
                                && int.TryParse(actionLineSplited[5], out int indexInChain5)
                                && int.TryParse(actionLineSplited[6], out int disId)
                                && int.TryParse(actionLineSplited[9], out int minSum))
                                userManager.AddBuyOverDiscount(minSum, DEF_SID, actionLineSplited[1], actionLineSplited[2],
                                    deadline, percentage, (Operator)op, indexInChain5, disId, toLeft);
                        }
                        break;
                    case "RemoveDiscount":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited)
                            && int.TryParse(actionLineSplited[2], out int indexInChain6))
                            userManager.RemoveDiscount(DEF_SID, actionLineSplited[1], indexInChain6);
                        break;
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
            string storeName = "";
            string creditCardNumber = "";
            string city = "";
            string street = "";
            string houseNumber = "";
            string country = "";
            foreach (var property in properties)
            {
                if (property.Key.Equals("storeName"))
                    storeName = (string)property.Value;
                if (property.Key.Equals("creditCardNumber"))
                    creditCardNumber = (string)property.Value;
                if (property.Key.Equals("city"))
                    city = (string)property.Value;
                if (property.Key.Equals("street"))
                    street = (string)property.Value;
                if (property.Key.Equals("houseNumber"))
                    houseNumber = (string)property.Value;
                if (property.Key.Equals("country"))
                    country = (string)property.Value;
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
                    'newQuantity': {'type':'number', 'required': 'true'}
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

        bool CheckArgs(string[] args)
        {
            foreach (string arg in args)
                if (arg.Length <= 0)
                    return false;
            return true;
        }

        Operator? StringToOperator(string op)
        {
            return op switch
            {
                "And" => Operator.And,
                "Or" => Operator.Or,
                "Xor" => Operator.Xor,
                "Implies" => Operator.Implies,
                _ => null,
            };
        }

        DayOfWeek? StringToDayOfWeek(string day)
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
                _ => null,
            };
        }
    }
}
