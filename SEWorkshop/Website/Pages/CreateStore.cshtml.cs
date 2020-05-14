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
    public class CreateStoreModel : PageModel
    {
        public IUserManager UserManager { get; }
        public DataStore? Store { get; private set; }
        public string StoreName {get; private set; }
        public string ErrorMsg { get; private set; }

        public CreateStoreModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
        }

        public void OnGet()
        {
            ErrorMsg = "";
        }

        public IActionResult OnPost(string storeName)
        {
            try
            {
                UserManager.OpenStore(HttpContext.Session.Id, storeName);
            }
            catch(UserHasNoPermissionException e)
            {
                ErrorMsg = e.ToString();
            }
            catch(StoreNotInTradingSystemException e)
            {
                ErrorMsg = e.ToString();
            }
            StoreName = storeName;
            return RedirectToPage("./Store", new { storeName = storeName });
        }
    }
}