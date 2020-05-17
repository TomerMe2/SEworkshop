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
        public IEnumerable<string> Users { get; set; }
        public bool IsAdmin { get; set; }
        public string Username { get; set; }  //current user
        [BindProperty(SupportsGet = true)]
        public string SearchUsername { get; set; }
        public string ErrorMsg { get; set; }

        public UserPageModel(IUserManager userManager)
        {
            UserManager = userManager;
            Purchases = new List<DataPurchase>();
            Users = new List<string>();
            IsAdmin = false;
            Username = "";
            SearchUsername = "";
            ErrorMsg = "";
        }

        public void OnGet(string username)
        {
            string sid = HttpContext.Session.Id;
            Username = (UserManager.GetDataLoggedInUser(sid)).Username;
            SearchUsername = "";
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

        /*
        public void OnPost(string content, string storeName, string productName)
        {
            string sid = HttpContext.Session.Id;
            UserManager.WriteReview(sid, storeName, productName, content);
            Purchases = UserManager.PurchaseHistory(sid);
        }
        */
        public IActionResult OnPost()
        {
            try
            {
                Purchases = UserManager.UserPurchaseHistory(HttpContext.Session.Id, SearchUsername);
            }
            catch (Exception e)
            {
                ErrorMsg = e.ToString();
                return new PageResult();
            }
            return RedirectToPage("./UserPage", new { Username = SearchUsername });
        }
    }
}