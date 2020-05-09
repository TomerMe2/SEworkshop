using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using SEWorkshop.Exceptions;

namespace Website.Pages
{
    public class StoreModel : PageModel
    {
        private IUserManager UserManager { get; }
        public DataStore? Store { get; private set; }
        public string ErrorMsg { get; private set; }

        public StoreModel(IUserManager userManager)
        {
            UserManager = userManager;
            ErrorMsg = "";
        }

        public void OnGet(string storeName)
        {
            try
            {
                Store = UserManager.SearchStore(storeName);
            }
            catch(StoreNotInTradingSystemException e)
            {
                ErrorMsg = e.ToString();
            }
        }
    }
}