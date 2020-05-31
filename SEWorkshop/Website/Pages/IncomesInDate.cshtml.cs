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
        }

        public void OnGet(int? year, int? month, int? day)
        {
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