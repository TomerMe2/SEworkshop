using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class UserPageModel : PageModel
    {
        public IUserManager UserManager { get; }
        public IEnumerable<DataPurchase> purchases { get; private set; }
        public UserPageModel(IUserManager userManager)
        {
            UserManager = userManager;
            purchases = new List<DataPurchase>();
            
        }
        public void OnGet()
        {
            try
            {
                Address adr1 = new Address("rehovot", "hertzel", "206");
                Address adr2 = new Address("beersheva", "hertzel", "206");
                string sid = HttpContext.Session.Id;
                UserManager.Register(sid, "wello", "1234");
                UserManager.Login(sid, "wello", "1234");
                UserManager.OpenStore(sid, "nini");
                UserManager.AddProduct(sid, "nini", "prod1", "some description1", "cat1", 123.7, 7);
                UserManager.AddProduct(sid, "nini", "prod2", "some LONGGGGGGGGGGGGGGGGGGGGGGGGG descriptionnnnnnnnnnnnnnn2", "cat2", 999999, 1);
                UserManager.AddProductToCart(sid, "nini", "prod1", 6);
                UserManager.AddProductToCart(sid, "nini", "prod2", 1);
                UserManager.Purchase(sid, UserManager.MyCart(sid).First(), "12345", adr1);
                UserManager.OpenStore(sid, "bibi");
                UserManager.AddProduct(sid, "bibi", "brod1", "bsome description1", "cat1", 150, 7);
                UserManager.AddProduct(sid, "bibi", "brod2", "bsome LONGGGGGGGGGGGGGGGGGGGGGGGGG descriptionnnnnnnnnnnnnnn2", "bcat2", 873, 1);
                UserManager.AddProductToCart(sid, "bibi", "brod1", 3);
                UserManager.Purchase(sid, UserManager.MyCart(sid).First(), "123456", adr2);
            }
            catch { }
            purchases = UserManager.PurchaseHistory(HttpContext.Session.Id);
        }
        public void OnPost(string content, string storeName, string productName)
        {
            string sid = HttpContext.Session.Id;
            UserManager.WriteReview(sid, storeName, productName, content);
        }

    }
}