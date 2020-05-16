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
    public class StorePurchaseHistoryModel : PageModel
    {
        public IUserManager UserManager { get; }
        public DataStore? Store { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StoreName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ErrorMsg { get; set; }

        public StorePurchaseHistoryModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
        }

        public void OnGet(string storeName)
        {
            try
            {
                Store = UserManager.SearchStore(storeName);
                StoreName = storeName;
            }
            catch (Exception e)
            {
                ErrorMsg = e.ToString();
            }
        }
    }
}