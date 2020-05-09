using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;
using System;

namespace Website.Pages
{
    public class LoginModel : PageModel
    {
        private IUserManager UserManager;

        [BindProperty (SupportsGet = true)]
        public string Username { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Error { get; set; }

        public LoginModel(IUserManager userManager)
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
                UserManager.Login(Username, Password);
            }
            catch (Exception e)
            {
                Error = e.ToString();
                return new PageResult();
            }
            return RedirectToPage("./Index", new { Username = Username });
        }
    }
}