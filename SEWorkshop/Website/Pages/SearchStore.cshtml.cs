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

        public DataStore? Store { get; private set; }
        
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

        public IActionResult OnPost(string storeName)
        {
            StoreName = storeName;
            try
            {
                Store = UserManager.SearchStore(storeName);
            }
            catch (Exception e)
            {
                Error = "No Results";
                Console.WriteLine(e.ToString());
            }
            return RedirectToPage("./Store", new { storeName = storeName });
        }
    }
}