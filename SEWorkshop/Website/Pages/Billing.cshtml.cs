using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using SEWorkshop;
using System;

namespace Website.Pages
{
    public class BillingModel : PageModel
    {
        public IUserManager UserManager { get; }

        [BindProperty (SupportsGet = true)]
        public string ErrorMsg { get; set; }
        public string StoreName { get; set; }
        public DataBasket? Basket { get; set; }

        public BillingModel(IUserManager userManager)
        {
            UserManager = userManager;
            StoreName = "";
            ErrorMsg = "";
        }

        public void OnGet(string storeName)
        {
            StoreName = storeName;
            List<DataBasket> cart = UserManager.MyCart(HttpContext.Session.Id).ToList();
            foreach (var basket in cart)
            {
                if(basket.Store.Name.Equals(StoreName))
                {
                    Basket = basket;
                    break;
                }
            }
        }

        public IActionResult OnPostPurchaseAsync(string storeName, string CreditCardNumber, string Country, string City, string Street, string HouseNumber)
        {
            List<DataBasket> cart = UserManager.MyCart(HttpContext.Session.Id).ToList();
            foreach (var basket in cart)
            {
                if(basket.Store.Name.Equals(storeName))
                {
                    Basket = basket;
                    break;
                }
            }
            if(CreditCardNumber == null || Country == null
                || City == null || Street == null || HouseNumber == null)
            {
                ErrorMsg = "Input is Invalid";
                return new PageResult();
            }
            Address address = new Address(Country, City, Street, HouseNumber);
            try
            {
                if(Basket == null)
                {
                    throw new Exception();
                }
                UserManager.Purchase(HttpContext.Session.Id, Basket, CreditCardNumber, address);
            }
            catch
            {
                ErrorMsg = "An Error has Occured";
                return new PageResult();
            }
            return RedirectToPage("./Cart");
        }
        
        public IActionResult OnPostRemoveFromCartAsync(string StoreName, string ProductName, string Quantity)
        {
            try
            {
                UserManager.RemoveProductFromCart(HttpContext.Session.Id, StoreName, ProductName, int.Parse(Quantity));
            }
            catch
            {
                ErrorMsg = "An Error Has Occured";
                return new PageResult();
            }
            return RedirectToPage("./Cart");
        }
    }
}