using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class UserPageModel : PageModel
    {
        private IUserManager UserManager { get; }
        public IEnumerable<DataPurchase> purchases { get; private set; }
        public UserPageModel(IUserManager userManager)
        {
            UserManager = userManager;
            
        }
        public void OnGet()
        {
            purchases = UserManager.PurchaseHistory(HttpContext.Session.Id);
        }
    }
}