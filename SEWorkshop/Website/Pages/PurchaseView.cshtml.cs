using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;

namespace Website.Pages
{
    public class PurchaseViewModel : PageModel
    {
        public DataPurchase Purchase { get; private set; }
        public double TotalPrice { get; private set; }
        public PurchaseViewModel(DataPurchase purchase)
        {
            Purchase = purchase;
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