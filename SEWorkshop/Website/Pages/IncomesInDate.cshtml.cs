using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class IncomesInDateModel : PageModel
    {
        public DateTime Date { get; private set; }
        public IUserManager UserManager { get; }
        public string ErrorMsg { get; private set; }

        public IncomesInDateModel(IUserManager userManager)
        {
            UserManager = userManager;
            Date = DateTime.Now;
            ErrorMsg = "";
            try
            {
                UserManager.Register("1", "nini", "1234");
                UserManager.Login("1", "nini", "1234");
                UserManager.OpenStore("1", "suprise");
                UserManager.AddProduct("1", "suprise", "moreSuprise", "nini", "nana", 100, 11);
                UserManager.Logout("1");
                UserManager.Register("1", "buyer", "1234");
                UserManager.Login("1", "buyer", "1234");
                UserManager.AddProductToCart("1", "suprise", "moreSuprise", 3);
                UserManager.Purchase("1", UserManager.MyCart("1").First(bskt => bskt.Store.Name.Equals("suprise")), "555",
                    new SEWorkshop.Address("nini", "nana", "yalla", "aroh"));
            }
            catch { }
        }

        public void OnGet(int? year, int? month, int? day)
        {
            //try
            //{
            //    UserManager.Login(HttpContext.Session.Id, "admin", "sadnaTeam");
            //}
            //catch { }
            if (year != null && month != null && day != null)
            {
                try
                {
                    Date = new DateTime(year.Value, month.Value, day.Value);
                }
                catch
                {
                    ErrorMsg = "Invalid date, shows today's incomes";
                }
            }
        }

        public IActionResult OnPost(int? year, int? month, int? day)
        {
            return RedirectToPage("./IncomesInDate", new { year, month, day});
        }
    }
}