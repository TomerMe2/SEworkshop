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

        public UserPageModel(IUserManager userManager)
        {
            UserManager = userManager;
        }

        public void OnGet()
        {

        }

        public void OnPost(string content, string storeName, string productName)
        {
            string sid = HttpContext.Session.Id;
            UserManager.WriteReview(sid, storeName, productName, content);
        }
    }
}