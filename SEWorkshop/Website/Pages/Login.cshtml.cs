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
        public IUserManager UserManager { get; }

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
                string sid = HttpContext.Session.Id;
                UserManager.Register(sid, "owner", "1234");
                UserManager.Register(sid, "manager", "1234");
                UserManager.Register(sid, "user", "1234");
                UserManager.Register(sid, "owner2", "1234");
                UserManager.Login(sid, "owner", "1234");
                UserManager.OpenStore(sid, "nini");
                UserManager.AddProduct(sid, "nini", "prod1", "some description1", "cat1", 123.7, 7);
                UserManager.AddProduct(sid, "nini", "prod2", "some LONGGGGGGGGGGGGGGGGGGGGGGGGG descriptionnnnnnnnnnnnnnn2", "cat2", 999999, 1);
                UserManager.AddStoreManager(sid, "nini", "manager");
                //UserManager.AddStoreOwner(sid, "nini", "owner2");
                UserManager.SetPermissionsOfManager(sid, "nini", "manager", "Manager");
                UserManager.Logout(sid);
            }
            catch { }
            
            try
            {
                UserManager.Login(HttpContext.Session.Id, Username, Password);
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