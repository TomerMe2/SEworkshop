using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.Enums;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class UsersStatisticsModel : PageModel
    {
        public IUserManager UserManager { get; }
        public DateTime DateTimeNow;
        public IDictionary<KindOfUser, int> TodaysStatistics;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Error;
        public bool Selected;
        public IDictionary<DateTime, IDictionary<KindOfUser, int>> SelectedStatistics;

        public UsersStatisticsModel(IUserManager userManager)
        {
            UserManager = userManager;
            TodaysStatistics = new Dictionary<KindOfUser, int>();
            SelectedStatistics = new Dictionary<DateTime, IDictionary<KindOfUser, int>>();
            Selected = false;
            Error = "";
        }

        public void OnGet()
        {
            Selected = false;
            Error = "";
            string sid = HttpContext.Session.Id;
            DateTimeNow = DateTime.Now;
            TodaysStatistics = UserManager.GetUsersByCategory(sid, DateTimeNow);
        }

        public void OnPost(DateTime StartDate, DateTime EndDate)
        {
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            try
            {
                string sid = HttpContext.Session.Id;
                DateTimeNow = DateTime.Now;
                TodaysStatistics = UserManager.GetUsersByCategory(sid, DateTimeNow);
                SelectedStatistics = UserManager.GetUseRecord(sid, StartDate, EndDate);
                Selected = true;
            }
            catch (Exception e)
            {
                Error = e.ToString();
            }
        }
    }
}