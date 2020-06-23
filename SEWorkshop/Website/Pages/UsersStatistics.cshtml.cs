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
        List<KindOfUser> KindsOfUsers;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Error;
        public bool Selected;
        public IDictionary<DateTime, int> SelectedStatistics;

        public UsersStatisticsModel(IUserManager userManager)
        {
            UserManager = userManager;
            TodaysStatistics = new Dictionary<KindOfUser, int>();
            SelectedStatistics = new Dictionary<DateTime, int>();
            KindsOfUsers = new List<KindOfUser>();
            KindsOfUsers.Add(KindOfUser.Guest);
            KindsOfUsers.Add(KindOfUser.LoggedInNotOwnNotManage);
            KindsOfUsers.Add(KindOfUser.LoggedInNoOwnYesManage);
            KindsOfUsers.Add(KindOfUser.LoggedInYesOwn);
            KindsOfUsers.Add(KindOfUser.Admin);
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

        public void OnPost()
        {
            try
            {
                string sid = HttpContext.Session.Id;
                DateTimeNow = DateTime.Now;
                TodaysStatistics = UserManager.GetUsersByCategory(sid, DateTimeNow);
                SelectedStatistics = UserManager.GetUseRecord(sid, StartDate, EndDate, KindsOfUsers);
                Selected = true;
            }
            catch (Exception e)
            {
                Error = e.ToString();
                //return new PageResult();
            }
            //return new PageResult();
        }
    }
}