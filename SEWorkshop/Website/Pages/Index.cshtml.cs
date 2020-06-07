using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }
        public IUserManager UserManager { get; }

        public IndexModel(IUserManager userManager)
        {
            UserManager = userManager;
            Username = "";
        }

        public void OnGet()
        {
        }
    }
}
