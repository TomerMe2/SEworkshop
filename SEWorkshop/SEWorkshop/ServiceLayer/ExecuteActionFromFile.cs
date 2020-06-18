using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using SEWorkshop.DataModels;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                Console.WriteLine("File not found.\nFile name should be 'ActionsFile.json' and it should be located in SEWorkshop\\Website");
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

        // action line format: <ActionName>,<Arg1>,<Arg2>...
        [Obsolete]
        void ReadActionsFile(List<JObject> jobjectsList, List<Dictionary<string, object>> dictionariesList)
        {
            // Read file on startup
            //foreach (string line in lines) { Console.WriteLine("\t" + line); }
            int index = -1;
            foreach (JObject action in jobjectsList)
            {
                index++;
                Dictionary<string, object> valuesDictionary = dictionariesList.ElementAt(index);
                switch (valuesDictionary.ElementAt(0).Value)
                {
                    case "Register":
                        HandleRegister(action, valuesDictionary);
                        break;
                    /*
                    case "Login":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.Login(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        else
                            Console.WriteLine("Login action is invalid, correct structure is: Login,Username,Password");
                        break;

                    case "Logout":
                        userManager.Logout(DEF_SID);
                        break;
                    case "AddProductToCart":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                        {
                            if (int.TryParse(actionLineSplited[3], out int val1))
                                userManager.AddProductToCart(DEF_SID, actionLineSplited[1], actionLineSplited[2], val1);
                            else
                                Console.WriteLine("AddProductToCart action is invalid, Quantity should be integer");
                        }
                        else
                            Console.WriteLine("AddProductToCart action is invalid, correct structure is: AddProductToCart,StoreName,ProductName,Quantity");
                        break;
                    case "RemoveProductFromCart":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                        {
                            if (int.TryParse(actionLineSplited[3], out int val2))
                                userManager.RemoveProductFromCart(DEF_SID, actionLineSplited[1], actionLineSplited[2], val2);
                            else
                                Console.WriteLine("RemoveProductFromCart action is invalid, Quantity should be integer");
                        }
                        else
                            Console.WriteLine("RemoveProductFromCart action is invalid, correct structure is: RemoveProductFromCart,StoreName,ProductName,Quantity");
                        break;
                    case "Purchase":
                        // action line format: <ActionName>,<StoreName>,<creditCardNumber>,<City>,<Street>,<HouseNumber>,<Country>
                        if (actionLineSplited.Length == 7 && int.TryParse(actionLineSplited[2], out int val3))
                        {
                            // find the basket according to store name
                            IEnumerable<DataBasket> baskets = userManager.MyCart(DEF_SID);
                            Address address = new Address(actionLineSplited[6], actionLineSplited[3], actionLineSplited[4], actionLineSplited[5]);
                            foreach (DataBasket basket in baskets)
                                if (basket.Store.Name.Equals(actionLineSplited[1]))
                                {
                                    userManager.Purchase(DEF_SID, basket, actionLineSplited[2], address);
                                }
                            Console.WriteLine("Purchase action is invalid, basket in the StoreName is not found");
                        }
                        else
                            Console.WriteLine("Purchase action is invalid, correct structure is: Purchase,StoreName,CreditCardNumber,City,Street,HouseNumber,Country");
                        break;
                    case "OpenStore":
                        if (actionLineSplited.Length == 2)
                            userManager.OpenStore(DEF_SID, actionLineSplited[1]);
                        else
                            Console.WriteLine("OpenStore action is invalid, correct structure is: OpenStore,StoreName");
                        break;
                    case "WriteReview":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                            userManager.WriteReview(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3]);
                        else
                            Console.WriteLine("WriteReview action is invalid, correct structure is: WriteReview,StoreName,ProductName,Review");
                        break;
                    case "WriteMessage":
                        if (actionLineSplited.Length == 3 && CheckArgs(actionLineSplited))
                            userManager.WriteMessage(DEF_SID, actionLineSplited[1], actionLineSplited[2]);
                        else
                            Console.WriteLine("WriteMessage action is invalid, correct structure is: WriteMessage,StoreName,ProductName,Message");
                        break;
                    case "AddProduct":
                        // action line format: <storeName>,<productName>,<description>,<category>,<price>,<quantity>
                        if (actionLineSplited.Length == 7 && CheckArgs(actionLineSplited))
                        {
                            if (double.TryParse(actionLineSplited[5], out double price1) && int.TryParse(actionLineSplited[6], out int quantity1))
                                userManager.AddProduct(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3], actionLineSplited[4], price1, quantity1);

                        }
                        else
                            Console.WriteLine("AddProduct action is invalid, correct structure is: AddProduct,StoreName,ProductName,Description,Category,Price,Quantity");
                        break;
                    case "EditProductName":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                            userManager.EditProductName(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3]);
                        break;
                    case "EditProductCategory":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                            userManager.EditProductCategory(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3]);
                        break;
                    case "EditProductDescription":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited))
                            userManager.EditProductDescription(DEF_SID, actionLineSplited[1], actionLineSplited[2], actionLineSplited[3]);
                        break;
                    case "EditProductPrice":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited) &&
                            double.TryParse(actionLineSplited[3], out double price2))
                            userManager.EditProductPrice(DEF_SID, actionLineSplited[1], actionLineSplited[2], price2);
                        break;
                    case "EditProductQuantity":
                        if (actionLineSplited.Length == 4 && CheckArgs(actionLineSplited) &&
                            int.TryParse(actionLineSplited[3], out int quantity2))
                            userManager.EditProductQuantity(DEF_SID, actionLineSplited[1], actionLineSplited[2], quantity2);
                        break;
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
                        Console.WriteLine("action name is illegal");
                        continue;
                }
            }
        }

        [Obsolete]
        void HandleRegister(JObject action, Dictionary<string,object> valuesDictionary)
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
                userManager.Register(DEF_SID, (string)valuesDictionary.ElementAt(1).Value, (string)valuesDictionary.ElementAt(2).Value);
            }
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
