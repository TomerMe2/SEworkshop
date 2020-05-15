using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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
