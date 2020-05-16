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
        public DataStore Store { get; private set; }
        public DataLoggedInUser LoggedUser { get; private set; }
        public IEnumerable<string> countries { get; private set; }

        public ManageModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            countries = System.IO.File.ReadAllLines("./wwwroot/js/countries.txt");
            Error = "";
            Policy = "";
            //Store = store;

        }
        public void OnGet(string storeName, string error) 
        {
            Store = UserManager.SearchStore(storeName);
            LoggedUser = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
            DataPolicy policy = Store.Policy;
            PolicyNumber = 0;
            Error = error;
            if (policy.InnerPolicy.HasValue)
                Policy = stringPolicy(policy.InnerPolicy.Value.Item1, 1);
            else
                Policy = "None";
        }

        public IActionResult OnPost(string storeName, string request, string value, string value2, string value3, string value4)
        {
            string sid = HttpContext.Session.Id;
            StoreName = storeName;
            switch (request)
            {
                case "1":
                    //UserManager.Removeow(sid, StoreName, value);
                    break;
                case "2":
                    UserManager.RemoveStoreManager(sid, StoreName, value);
                    break;
                case "3":
                    UserManager.SetPermissionsOfManager(sid, StoreName, value, value2);
                    break;
                case "4":
                    UserManager.RemovePermissionsOfManager(sid, StoreName, value, value2);
                    break;
                case "5":
                    UserManager.AddStoreOwner(sid, StoreName, value);
                    if(value == null)
                    {
                        Error = "Illegal Value";
                        break;
                    }
                    break;
                case "6":
                    UserManager.AddStoreManager(sid, StoreName, value);
                    if (value == null)
                    {
                        Error = "Illegal Value";
                        break;
                    }
                    break;
                case "7":
                    UserManager.AddSystemDayPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), (DayOfWeek)Enum.Parse(typeof(DayOfWeek), value2));
                    break;
                case "8":
                    UserManager.AddUserCountryPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), value2);
                    break;
                case "9":
                    UserManager.AddUserCityPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), value2);
                    if (value2 == null)
                    {
                        Error = "Illegal Value";
                        break;
                    }
                    break;
                case "10":
                    try
                    {
                        int min = -1, max = -1;
                        if (value2 == null)
                            min = -1;
                        else {
                            min = Int32.Parse(value2);
                            if (min < 0)
                                throw new Exception();
                        }
                        if (value3 == null)
                            max = -1;
                        else {
                            max = Int32.Parse(value3);
                            if (max < 0)
                                throw new Exception();
                        }
                        if ((min > max && max != -1) || (min == -1 && max == -1))
                            throw new Exception();
                        UserManager.AddWholeStoreQuantityPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), min, max);
                    }
                    catch (Exception)
                    {
                        Error = "Invald Number Range";
                    }
                    break;
                case "11":
                    try
                    {
                        int min = -1, max = -1;
                        if (value2 == null)
                            min = -1;
                        else
                        {
                            min = Int32.Parse(value2);
                            if (min < 0)
                                throw new Exception();
                        }
                        if (value3 == null)
                            max = -1;
                        else
                        {
                            max = Int32.Parse(value3);
                            if (max < 0)
                                throw new Exception();
                        }
                        if ((min > max && max != -1) || (min == -1 && max == -1))
                            throw new Exception();
                        if (value4 == null)
                        {
                            Error = "Illegal Value";
                            break;
                        }
                        UserManager.AddSingleProductQuantityPolicy(sid, StoreName, (Operator)Enum.Parse(typeof(Operator), value), value4, min, max);
                    }
                    catch (Exception)
                    {
                        Error = "Invald Number Range";
                    }
                    break;
                case "12":
                    UserManager.RemovePolicy(sid, StoreName, Int32.Parse(value));
                    break;
            }
            return RedirectToPage("./Manage", new { StoreName = StoreName, Error = Error });
        }

        private string stringPolicy(DataPolicy policy, int index)
        {
            if (!policy.InnerPolicy.HasValue)
            {
                PolicyNumber = index;
                return "[" + index + "] " + policy.ToString();
            }
            return "[" + index + "] " + policy.ToString()+ " "+ policy.InnerPolicy.Value.Item2.ToString() + " (" + stringPolicy(policy.InnerPolicy.Value.Item1, index + 1) + ")";
        }
    }
}