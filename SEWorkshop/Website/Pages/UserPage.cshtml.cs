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
        public IUserManager UserManager { get; }
        public IEnumerable<DataPurchase> purchases { get; private set; }
        public UserPageModel(IUserManager userManager)
        {
            UserManager = userManager;
            purchases = new List<DataPurchase>();
            
        }
        public void OnGet()
        {
            purchases = UserManager.PurchaseHistory(HttpContext.Session.Id);
        }
        public void OnPost(string content, string storeName, string productName)
        {
            string sid = HttpContext.Session.Id;
            UserManager.WriteReview(sid, storeName, productName, content);
            purchases = UserManager.PurchaseHistory(sid);
        }

    }
}