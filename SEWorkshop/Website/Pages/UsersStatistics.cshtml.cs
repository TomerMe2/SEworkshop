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

        public UsersStatisticsModel(IUserManager userManager)
        {
            UserManager = userManager;
            TodaysStatistics = new Dictionary<KindOfUser, int>();
            KindsOfUsers = new List<KindOfUser>();
            KindsOfUsers.Add(KindOfUser.Guest);
            KindsOfUsers.Add(KindOfUser.LoggedInNotOwnNotManage);
            KindsOfUsers.Add(KindOfUser.LoggedInNoOwnYesManage);
            KindsOfUsers.Add(KindOfUser.LoggedInYesOwn);
            KindsOfUsers.Add(KindOfUser.Admin);
        }

        public void OnGet()
        {
            string sid = HttpContext.Session.Id;
            DateTimeNow = DateTime.Now;
            TodaysStatistics = UserManager.GetUsersByCategory(sid, DateTimeNow);
        }
    }
}