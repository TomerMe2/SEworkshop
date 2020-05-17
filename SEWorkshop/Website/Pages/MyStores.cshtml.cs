using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class MyStoresModel : PageModel
    {
        public IUserManager UserManager { get; }

        public MyStoresModel(IUserManager userManager)
        {
            UserManager = userManager;
        }

        public void OnGet()
        {

        }
    }
}