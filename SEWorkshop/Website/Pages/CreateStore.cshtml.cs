using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using SEWorkshop.Exceptions;

namespace Website.Pages
{
    public class CreateStoreModel : PageModel
    {
        public IUserManager UserManager { get; }
        public DataStore? Store { get; private set; }

        [BindProperty(SupportsGet = true)]
        public string StoreName {get; set; }
        [BindProperty(SupportsGet = true)]
        public string ErrorMsg { get; private set; }

        public CreateStoreModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
        }

        public void OnGet()
        {
            ErrorMsg = "";
        }

        public IActionResult OnPost()
        {
            if(StoreName == null)
            {
                ErrorMsg = "Invalid Product Name";
                return new PageResult();
            }
            try
            {
                UserManager.OpenStore(HttpContext.Session.Id, StoreName);
            }
            catch(Exception)
            {
                ErrorMsg = "Invalid Store Name";
                return new PageResult();
            }
            return RedirectToPage("./Index");
        }
    }
}