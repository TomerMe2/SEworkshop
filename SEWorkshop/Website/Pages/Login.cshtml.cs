using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.ServiceLayer;
using SEWorkshop.Exceptions;

namespace Website.Pages
{
    public class LoginModel : PageModel
    {
        public IUserManager UserManager = new UserManager();

        [BindProperty (SupportsGet = true)]
        public string Username { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsValid { get; set; }

        public void OnGet()
        {
            IsValid = true;
        }

        public IActionResult OnPost()
        {
            try
            {
                UserManager.Login(Username, Password);
            }
            catch (UserDoesNotExistException)
            {
                IsValid = false;
                return new PageResult();
            }
            return RedirectToPage("./Index", new { Username = Username });
        }
    }
}