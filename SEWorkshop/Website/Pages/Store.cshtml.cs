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
            try
            {
                userManager.Register("wello", "1234");
                userManager.Login("wello", "1234");
                userManager.OpenStore("nini");
                userManager.AddProduct("nini", "prod1", "some description1", "cat1", 123.7, 7);
                userManager.AddProduct("nini", "prod2", "some LONGGGGGGGGGGGGGGGGGGGGGGGGG descriptionnnnnnnnnnnnnnn2", "cat2", 999999, 1);
            }
            catch { }
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