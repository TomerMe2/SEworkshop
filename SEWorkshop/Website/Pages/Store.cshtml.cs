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
    public class StoreModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        private IUserManager UserManager { get; }
        public DataStore? Store { get; private set; }
        public string StoreName {get; private set; }
        public string ErrorMsg { get; private set; }

        public StoreModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
        }
        
        public void OnGet(string storeName)
        {
            try
            {
                StoreName = storeName;
                Store = UserManager.SearchStore(StoreName);
            }
            catch(StoreNotInTradingSystemException e)
            {
                ErrorMsg = e.ToString();
            }
        }

        public IActionResult OnPostAddToCartAsync(string StoreName, string ProductName, string Quantity)
        {
            int quantity;
            try
            {
                quantity = int.Parse(Quantity);
            }
            catch(Exception)
            {
                ErrorMsg = "Quantity must be a positive number";
                return new PageResult();
            }
            if(quantity <= 0)
            {
                ErrorMsg = "Quantity must be a positive number";
                return new PageResult();
            }
            try
            {
                UserManager.AddProductToCart(HttpContext.Session.Id, StoreName, ProductName, int.Parse(Quantity));
            }
            catch(Exception e)
            {
                ErrorMsg = e.ToString();
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }
    }
}