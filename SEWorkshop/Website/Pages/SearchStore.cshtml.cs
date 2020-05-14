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
        public string ErrorMsg { get; set; }

        public SearchStoreModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
        }
        
        public void OnGet()
        {
            ErrorMsg = "";
        }

        public IActionResult OnPost(string storeName)
        {
            if(storeName == null)
            {
                ErrorMsg = "Invalid Product Name";
                return new PageResult();
            }
            StoreName = storeName;
            try
            {
                Store = UserManager.SearchStore(storeName);
            }
            catch (Exception e)
            {
                ErrorMsg = "No Results";
                Console.WriteLine(e.ToString());
            }
            return RedirectToPage("./Store", new { storeName = storeName });
        }
    }
}