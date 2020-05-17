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
    public class UserPageModel : PageModel
    {
        public IUserManager UserManager { get; }
        public IEnumerable<DataPurchase> Purchases { get; private set; }
        public IEnumerable<string> Users;
        public bool IsAdmin;
        public string Username;

        public UserPageModel(IUserManager userManager)
        {
            UserManager = userManager;
            Purchases = new List<DataPurchase>();
            Users = new List<string>();
            IsAdmin = false;
            Username = "";
        }

        public void OnGet(string username)
        {
            string sid = HttpContext.Session.Id;
            Username = (UserManager.GetDataLoggedInUser(sid)).Username;
            IsAdmin = UserManager.IsAdministrator(sid);
            Users = UserManager.GetAllUsers(sid).ToList();
            if (username == null || username.Length == 0)
            {
                Purchases = UserManager.PurchaseHistory(sid);
            }
            else
            {
                Purchases = UserManager.UserPurchaseHistory(sid, username);
                Username = username;
            }
        }
        
        public void OnPost(string content, string storeName, string productName)
        {
            string sid = HttpContext.Session.Id;
            UserManager.WriteReview(sid, storeName, productName, content);
            Purchases = UserManager.PurchaseHistory(sid);
        }
        
    }
}