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
        public int Min { get; private set; }
        public int Max { get; private set; }
        public DataStore Store { get; private set; }
        public DataLoggedInUser LoggedUser { get; private set; }
        public IEnumerable<string> countries { get; private set; }

        public ManageModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            countries = System.IO.File.ReadAllLines("./wwwroot/texts/countries.txt");
            Error = "";
            Policy = "";
        }
        public void OnGet(string storeName, string error) 
        {
            Store = UserManager.SearchStore(storeName);
            LoggedUser = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
            DataPolicy policy = Store.Policy;
            PolicyNumber = 0;
            Error = error;
            if (policy is DataAlwaysTruePolicy)
                Policy = "None";
            else
                Policy = StringPolicy(policy, 0);

        }

        public IActionResult OnPostOwnerManagerHandler(string storeName, string request, string username)
        {
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            switch (request)
            {
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
                        UserManager.RemovePolicy(sid, StoreName, int.Parse(value));
                    }
                    catch (Exception e)
                    {
                        Error = e.ToString();
                    }
                    break;
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