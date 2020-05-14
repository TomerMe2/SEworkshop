using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class PurchaseViewModel : PageModel
    {
        public IUserManager UserManager { get; }
        public DataPurchase Purchase { get; private set; }
        public bool CanReview { get; private set; }
        public double TotalPrice { get; private set; }
        public string Purchaser { get; private set; }
        public PurchaseViewModel(IUserManager userManager, DataPurchase purchase, bool canReview)
        {
            UserManager = userManager;
            Purchase = purchase;
            CanReview = canReview;
            TotalPrice = 0;
            foreach(var pair in Purchase.Basket.Products)
            {
                TotalPrice += pair.Item1.Price * pair.Item2;
            }
        }
        public void OnGet()
        {

        }
    }
}