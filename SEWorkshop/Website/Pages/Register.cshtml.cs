using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class RegisterModel : PageModel
    {
        private IUserManager UserManager;

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Error { get; set; }

        public RegisterModel(IUserManager userManager)
        {
            UserManager = userManager;
            Username = "";
            Password = "";
            Error = "";
        }
        public void OnGet()
        {
            Error = "";
        }

        public IActionResult OnPost()
        {
            try
            {
                string deb = HttpContext.Session.Id;
                UserManager.Register(HttpContext.Session.Id, Username, Password);
            }
            catch (Exception e)
            {
                Error = e.ToString();
                return new PageResult();
            }
            return RedirectToPage("./Login");
        }
    }
}
