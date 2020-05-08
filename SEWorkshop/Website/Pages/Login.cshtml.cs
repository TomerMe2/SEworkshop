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
        public UserManager UserManager = UserManager.GetInstance();

        [BindProperty (SupportsGet = true)]
        public string Username { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string Password { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Error { get; set; }

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
            catch (UserDoesNotExistException)
            {
                Error = "Username or password are incorrect";
                return new PageResult();
            }
            catch(ArgumentException e)
            {
                Error = e.Message;
                return new PageResult();
            }
            catch(UserAlreadyLoggedInException)
            {
                Error = "You are already logged in";
                return new PageResult();
            }
            return RedirectToPage("./Index", new { Username = Username });
        }
    }
}