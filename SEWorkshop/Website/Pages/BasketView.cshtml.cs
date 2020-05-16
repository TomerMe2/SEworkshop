using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using SEWorkshop.Exceptions;
using System;

namespace Website.Pages
{
    public class BasketViewModel : PageModel
    {
        public IUserManager UserManager { get; }

        [BindProperty (SupportsGet = true)]
        public string ErrorMsg { get; set; }
        public DataBasket Basket { get; set; }
        public bool Paying { get; set; }

        public BasketViewModel(IUserManager userManager, DataBasket basket)
        {
            UserManager = userManager;
            Basket = basket;
            Paying = false;
            ErrorMsg = "";
        }

        public void OnGet()
        {
        }
    }
}