using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.ObjectPool;
using SEWorkshop.DataModels;
using SEWorkshop.DataModels.Policies;
using SEWorkshop.Enums;
using SEWorkshop.Models;
using SEWorkshop.Models.Policies;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class ManageModel : PageModel
    {
        public IUserManager UserManager { get; }
        public string StoreName { get; private set; }
        public string Error { get; private set; }
        public string Policy { get; private set; }
        public int PolicyNumber { get; private set; }
        public int DiscountNumber { get; private set; }
        public int Min { get; private set; }
        public int Max { get; private set; }
        public DataStore? Store { get; private set; }
        public DataLoggedInUser? LoggedUser { get; private set; }
        public IEnumerable<string> countries { get; private set; }
        public ICollection<string> discounts { get; private set; }
        public ICollection<string> products { get; private set; }
        public ICollection<string> categories { get; private set; }

        public ManageModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            countries = System.IO.File.ReadAllLines("./wwwroot/texts/countries.txt");
            products = new List<string>();
            categories = new List<string>();
            discounts = new List<string>();
            Error = "";
            Policy = "";
            DiscountNumber = 0;
        }
        public IActionResult OnGet(string storeName, string error) 
        {
            try
            {
                Store = UserManager.SearchStore(storeName);
            }
            catch
            {
                return StatusCode(500);
            }
            LoggedUser = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
            DataPolicy policy = Store.Policy;
            PolicyNumber = 0;
            Error = error;
            foreach (DataProduct dp in Store.Products)
            {
                if (!categories.Contains(dp.Category))
                    categories.Add(dp.Category);
                products.Add(dp.Name);
            }
            foreach(DataDiscount disc in Store.Discounts)
            {
                discounts.Add(StringDiscount(disc));
            }
            DiscountNumber = Store.Discounts.Count();
            if (policy is DataAlwaysTruePolicy)
                Policy = "None";
            else
                Policy = StringPolicy(policy, 1);
            return Page();
            
        }

        public IActionResult OnPostOwnerManagerHandler(string storeName, string request, string username)
        {
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            switch (request)
            {
                case "RemoveOwner":
                    try
                    {
                        UserManager.RemoveStoreOwner(sid, StoreName, username);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "RemoveManager":
                    try
                    {
                        UserManager.RemoveStoreManager(sid, StoreName, username);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "AddOwner":
                    if (username == null)
                    {
                        Error = "Illegal Value";
                        break;
                    }
                    try
                    {
                        UserManager.AddStoreOwner(sid, StoreName, username);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "AddManager":
                    if (username == null)
                    {
                        Error = "Illegal Value";
                        break;
                    }
                    try
                    {
                        UserManager.AddStoreManager(sid, StoreName, username);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        public IActionResult OnPostPermissionHandler(string storeName, string request, string username, string authorization)
        {
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            switch (request)
            {
                case "AddPermission":
                    try
                    {
                        UserManager.SetPermissionsOfManager(sid, StoreName, username, authorization);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "RemovePermission":
                    try
                    {
                        UserManager.RemovePermissionsOfManager(sid, StoreName, username, authorization);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        public IActionResult OnPostPolicyHandler(string storeName, string request, string value, string value2, string value3, string value4)
        {
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            switch (request)
            {
                case "DayPolicy":
                    try
                    {
                        UserManager.AddSystemDayPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), value2));
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "CountryPolicy":
                    try
                    {
                        UserManager.AddUserCountryPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), value2);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "CityPolicy":
                    if (value2 == null)
                    {
                        Error = "Illegal Value";
                        break;
                    }
                    try
                    {
                        UserManager.AddUserCityPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), value2);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "StoreQuantityPolicy":
                    try
                    {
                        HandleMinMax(value2, value3);
                        UserManager.AddWholeStoreQuantityPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), Min, Max);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Error = "Invald Number Range";
                    }
                    catch(Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "ProductQuantityPolicy":
                    try
                    {
                        HandleMinMax(value2, value3);
                        UserManager.AddSingleProductQuantityPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), value4, Min, Max);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Error = "Invald Number Range";
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
                case "RemovePolicy":
                    try
                    {
                        UserManager.RemovePolicy(sid, StoreName, int.Parse(value) - 1);
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        public IActionResult OnPostDiscountHandler(string storeName, string oper, string appliedTo, string chosenProduct, string chosenCategory, string percent, string date, string index)
        {
            int newIndex;
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            if (string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(oper) || string.IsNullOrEmpty(percent) || string.IsNullOrEmpty(date))
            {
                Error = "Missing values";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            DateTime dateTime;
            Operator op;
            try
            {
                dateTime = DateTime.Parse(date);
            }
            catch
            {
                Error = "Date is invalid";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            try
            {
                op = (Operator)Enum.Parse(typeof(Operator), oper);
            }
            catch
            {
                Error = "Operator is invalid";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            StoreName = storeName;
            try
            {
                Store = UserManager.SearchStore(storeName);
                DiscountNumber = Store.Discounts.Count();
                if (index == null)
                    newIndex = DiscountNumber;
                else
                    newIndex = Int32.Parse(index);
                if (appliedTo.Equals("Product"))
                {
                    UserManager.AddSpecificProductDiscount(sid, storeName, chosenProduct, dateTime, Int32.Parse(percent), op, newIndex);
                }
                else
                {
                    UserManager.AddProductCategoryDiscount(sid, storeName, chosenCategory, dateTime, Int32.Parse(percent), op, newIndex);
                }
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        public IActionResult OnPostGetDiscountHandler(string storeName, string buy, string get, string oper, string chosenProduct, string percent, string date, string index)
        {
            int newIndex;
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            if (string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(oper) || string.IsNullOrEmpty(percent) || string.IsNullOrEmpty(date))
            {
                Error = "Missing values";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            DateTime dateTime;
            Operator op;
            try
            {
                dateTime = DateTime.Parse(date);
            }
            catch
            {
                Error = "invalid date";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            try
            {
                op = (Operator)Enum.Parse(typeof(Operator), oper);
            }
            catch
            {
                Error = "invalid operator";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            StoreName = storeName;
            try
            {
                Store = UserManager.SearchStore(storeName);
                DiscountNumber = Store.Discounts.Count();
                if (index == null)
                    newIndex = DiscountNumber;
                else
                    newIndex = Int32.Parse(index);
                UserManager.AddBuySomeGetSomeDiscount(Int32.Parse(buy), Int32.Parse(get), sid, chosenProduct, storeName, dateTime, Int32.Parse(percent), op, newIndex);
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        public IActionResult OnPostOverDiscountHandler(string storeName, string buy, string oper, string chosenProduct, string percent, string date, string index)
        {
            int newIndex;
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            if (string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(oper) || string.IsNullOrEmpty(percent) || string.IsNullOrEmpty(date))
            {
                Error = "Missing values";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            DateTime dateTime;
            Operator op;
            try
            {
                dateTime = DateTime.Parse(date);
            }
            catch
            {
                Error = "invalid date";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            try
            {
                op = (Operator)Enum.Parse(typeof(Operator), oper);
            }
            catch
            {
                Error = "invalid operator";
                return RedirectToPage("./Manage", new { StoreName, Error });
            }
            StoreName = storeName;
            try
            {
                Store = UserManager.SearchStore(storeName);
                DiscountNumber = Store.Discounts.Count();
                if (index == null)
                    newIndex = DiscountNumber;
                else
                    newIndex = Int32.Parse(index);
                UserManager.AddBuyOverDiscount(Int32.Parse(buy), sid, storeName, chosenProduct, dateTime, Int32.Parse(percent), op, newIndex);
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        public IActionResult OnPostRemoveDiscountHandler(string storeName, string buy, string oper, string chosenProduct, string percent, string date, string index)
        {
            int newIndex;
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            try
            {
                Store = UserManager.SearchStore(storeName);
                DiscountNumber = Store.Discounts.Count();
                if (index == null)
                    newIndex = DiscountNumber;
                else
                    newIndex = Int32.Parse(index);
                UserManager.RemoveDiscount(sid, storeName, newIndex);
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
            return RedirectToPage("./Manage", new { StoreName, Error });
        }

        private string StringPolicy(DataPolicy policy, int index)
        {
            if (!policy.InnerPolicy.HasValue)
            {
                PolicyNumber = index;
                return "[" + index + "] " + policy.ToString();
            }
            return "[" + index + "] " + policy.ToString()+ " "+ policy.InnerPolicy.Value.Item2.ToString() + " (" + StringPolicy(policy.InnerPolicy.Value.Item1, index + 1) + ")";
        }

        private string StringDiscount(DataDiscount discount)
        {
            if (!discount.InnerDiscount.HasValue)
            {
                return "" + discount.ToString();
            }
            return discount.ToString() + " " + discount.InnerDiscount.Value.Item2.ToString() + " (" + StringDiscount(discount.InnerDiscount.Value.Item1) + ")";
        }

        private void HandleMinMax(string min, string max)
        {
            if (min == null)
                Min = -1;
            else
            {
                Min = int.Parse(min);
                if (Min < 0)
                    throw new ArgumentOutOfRangeException();
            }
            if (max == null)
                Max = -1;
            else
            {
                Max = int.Parse(max);
                if (Max < 0)
                    throw new ArgumentOutOfRangeException();
            }
            if ((Min > Max && Max != -1) || (Min == -1 && Max == -1))
                throw new ArgumentOutOfRangeException();
        }
    }
}