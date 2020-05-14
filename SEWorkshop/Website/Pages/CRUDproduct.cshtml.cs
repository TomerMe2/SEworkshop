using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using SEWorkshop.Exceptions;
using System.Security.Cryptography;

namespace Website.Pages.Shared
{

    public class CRUDproductModel : PageModel
    {
        private IUserManager UserManager { get; }
        public DataProduct? Product { get; private set; }
        public string ProductName { get; private set; }
        public string ProductCategory { get; private set; }
        public string ProductDescription { get; private set; }
        public double ProductPrice { get; private set; }
        public int ProductQuantity { get; private set; }
        public string StoreName { get; private set; }
        public string ErrorMsg { get; private set; }

        public CRUDproductModel(IUserManager userManager) {
            UserManager = userManager;
            ProductName = "";
            ProductCategory = "";
            ProductDescription = "";
            ProductPrice = 0.00;
            ProductQuantity = 0;
            ErrorMsg = "";

        }
        public void OnGet()
        {
            ErrorMsg = "";
        }
        public IActionResult OnPost(string ProductName, string StoreName,  string ProductDescription, string ProductCategory, string ProductPrice, string ProductQuantity)
        {
            try
            {
                UserManager.AddProduct(HttpContext.Session.Id, StoreName, ProductName, ProductDescription, ProductCategory, double.Parse(ProductPrice), int.Parse(ProductQuantity));
            }
            catch (UserHasNoPermissionException e)
            {
                ErrorMsg = e.ToString();
            }
            catch (ProductAlreadyExistException e)
            {
                ErrorMsg = e.ToString();
            }
            catch (NegativeQuantityException e)
            {
                ErrorMsg = e.ToString();
            }
            catch (StoreNotInTradingSystemException e)
            {
                ErrorMsg = e.ToString();
            }
         
            this.ProductName = ProductName;
            this.StoreName = StoreName;
            this.ProductCategory = ProductCategory;
            this.ProductDescription = ProductDescription;
            this.ProductPrice =double.Parse(ProductPrice);
            this.ProductQuantity =int.Parse(ProductQuantity);

            return RedirectToPage("./Store", new {storeName= StoreName});

        }

    }
}