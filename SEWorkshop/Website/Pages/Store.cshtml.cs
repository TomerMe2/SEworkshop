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
        public IUserManager UserManager { get; }
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
            catch(Exception e)
            {
                ErrorMsg = e.ToString();
            }
        }
        
        public IActionResult OnPostEditDescriptionAsync(string StoreName, string ProductName, string Description)
        {
            try
            {
                UserManager.EditProductDescription(HttpContext.Session.Id, StoreName, ProductName, Description);
            }
            catch
            {
                ErrorMsg = "Invalid Data";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }

        public IActionResult OnPostEditCategoryAsync(string StoreName, string ProductName, string Category)
        {
            try
            {
                UserManager.EditProductCategory(HttpContext.Session.Id, StoreName, ProductName, Category);
            }
            catch
            {
                ErrorMsg = "Invalid Data";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }

        public IActionResult OnPostEditPriceAsync(string StoreName, string ProductName, string Price)
        {
            try
            {
                UserManager.EditProductPrice(HttpContext.Session.Id, StoreName, ProductName, int.Parse(Price));
            }
            catch
            {
                ErrorMsg = "Invalid Data";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }

        public IActionResult OnPostEditQuantityAsync(string StoreName, string ProductName, string Quantity)
        {
            try
            {
                UserManager.EditProductQuantity(HttpContext.Session.Id, StoreName, ProductName, int.Parse(Quantity));
            }
            catch
            {
                ErrorMsg = "Invalid Data";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }

        public IActionResult OnPostEditNameAsync(string StoreName, string ProductName, string Name)
        {
            try
            {
                UserManager.EditProductName(HttpContext.Session.Id, StoreName, ProductName, Name);
            }
            catch
            {
                ErrorMsg = "Invalid Data";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
        }
        public IActionResult OnPostRemoveProductAsync(string StoreName, string ProductName, string Quantity)
        {
            try
            {
                UserManager.RemoveProduct(HttpContext.Session.Id, StoreName, ProductName);
            }
            catch
            {
                ErrorMsg = "An Error Has Occured";
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
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