using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.Exceptions;
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
            Username = UserManager.GetLoggedInUsername(HttpContext.Session.Id);
        }

        public void OnGet()
        {
        }
    }
}
