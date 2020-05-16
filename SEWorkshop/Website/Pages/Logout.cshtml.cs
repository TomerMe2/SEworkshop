using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using System;

namespace Website.Pages
{
    public class LogoutModel : PageModel
    {
        public IUserManager UserManager { get; }

        [BindProperty (SupportsGet = true)]
        public string ErrorMsg { get; set; }

        public LogoutModel(IUserManager userManager)
        {
            UserManager = userManager;
            ErrorMsg = "";
        }
        public IActionResult OnGet()
        {
            if(!UserManager.IsLoggedIn(HttpContext.Session.Id))
            {
                ErrorMsg = "User is not logged in";
                return new PageResult();
            }
            try
            {
                UserManager.Logout(HttpContext.Session.Id);
            }
            catch 
            {
                ErrorMsg = "An Error has Occured";
                return new PageResult();
            }
            return RedirectToPage("./Login");
        }
    }
}