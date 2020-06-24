using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class StoresModel : PageModel
    {
        public IUserManager UserManager;

        [BindProperty(SupportsGet = true)]
        public IEnumerable<DataStore> Stores { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StoreName { get; set; }

        public DataStore? Store { get; private set; }
        
        [BindProperty(SupportsGet = true)]
        public string ErrorMsg { get; set; }

        public StoresModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
            Stores = UserManager.BrowseStores();
        }
        
        public void OnGet()
        {
            ErrorMsg = "";
            Stores = UserManager.BrowseStores();
        }

        public IActionResult OnPost()
        {
            try
            {
                Store = UserManager.SearchStore(StoreName);
            }
            catch (Exception e)
            {
                ErrorMsg = e.ToString();
                StoreName = "";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }
    }
}