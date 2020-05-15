using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class ManageModel : PageModel
    {
        public IUserManager UserManager { get; }
        public string StoreName { get; private set; }
        public DataStore Store { get; private set; }
        public DataLoggedInUser LoggedUser { get; private set; }
        public ManageModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            //Store = store;

        }
        public void OnGet(string storeName) 
        {
            Store = UserManager.SearchStore(storeName);
            LoggedUser = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
        }

        public IActionResult OnPost(string storeName, string request, string value, string value2)
        {
            string sid = HttpContext.Session.Id;
            //Store = UserManager.SearchStore(storeName);
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
            }
            return RedirectToPage("./Manage", new { StoreName = StoreName });
        }
    }
}