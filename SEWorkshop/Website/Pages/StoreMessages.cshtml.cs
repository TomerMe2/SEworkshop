using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class StoreMessagesModel : PageModel
    {
        public IUserManager UserManager { get; }
        public string StoreName { get; private set; }

        public StoreMessagesModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
        }

        public void OnGet(string storeName)
        {
            StoreName = storeName;
        }
    }
}