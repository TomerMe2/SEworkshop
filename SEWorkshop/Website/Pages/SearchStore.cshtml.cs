using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class SearchStoreModel : PageModel
    {
        private IUserManager UserManager;
        
        [BindProperty(SupportsGet = true)]
        public string StoreName { get; set; }

        public DataStore Store { get; private set; }
        
        [BindProperty(SupportsGet = true)]
        public string Error { get; set; }

        public SearchStoreModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
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
                Store = UserManager.SearchStore(StoreName);
            }
            catch (Exception)
            {
                Error = "No Results";
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }
    }
}