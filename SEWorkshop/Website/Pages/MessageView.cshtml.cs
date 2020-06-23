using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;

namespace Website.Pages
{
    public class MessageViewModel : PageModel
    {
        public IUserManager UserManager { get; }

        public string Error { get; private set; }
        public string CompleteMsg { get; private set; }

        public DataMessage? MsgToShow { get; private set; }
        public DataStore? StoreWritingTo { get; private set; }
        public string? UserNameWritingTo { get; private set; }
        public bool CanRespond { get; private set; }
        
        public MessageViewModel(IUserManager userManager)
        {
            UserManager = userManager;
            Error = "";
            CompleteMsg = "";
            CanRespond = false;
        }

        private void FindMessage(DataLoggedInUser loggedIn, bool isClient, bool isMan, bool isOwner, string writerUserName)
        {
            //find message to display
            //find his messages as client
            if (isClient)
            {
                MsgToShow = loggedIn.Messages.FirstOrDefault(currMsg => currMsg.ToStore.Equals(StoreWritingTo));
            }
            else if (isMan)
            {
                foreach (var manages in loggedIn.Manages)
                {
                    if (manages.Value.Where(auth => auth.Authorization == SEWorkshop.Enums.Authorizations.Watching).Any())
                    {
                        if (manages.Value.Where(auth => auth.Authorization == SEWorkshop.Enums.Authorizations.Replying).Any())
                        {
                            CanRespond = true;
                        }
                        MsgToShow = manages.Key.Messages.FirstOrDefault(currMsg => currMsg.WrittenBy.Equals(writerUserName));
                    }
                }
            }
            else if (isOwner)
            {
                foreach (var owns in loggedIn.Owns)
                {
                    if (owns.Equals(StoreWritingTo))
                    {
                        MsgToShow = owns.Messages.FirstOrDefault(currMsg => currMsg.WrittenBy.Username.Equals(writerUserName));
                    }
                }
            }
            if (!CanRespond)
            {
                CanRespond = isClient || isOwner;
            }
        }

        private void RelationsWithStore(DataLoggedInUser usr, string writerUserName, out bool isClient, out bool isMan, out bool isOwner)
        {
            isClient = usr.Username.Equals(writerUserName);
            isMan = false;
            foreach (var keyVal in usr.Manages)
            {
                if (keyVal.Key.Equals(StoreWritingTo) && keyVal.Value.Where(auth => auth.Authorization == SEWorkshop.Enums.Authorizations.Watching).Any())
                {
                    isMan = true;
                    break;
                }
            }
            isOwner = usr.Owns.Contains(StoreWritingTo);
        }

        public IActionResult OnGet(string storeName, string writerUserName)
        {
            if (!UserManager.IsLoggedIn(HttpContext.Session.Id))
            {
                return StatusCode(500);
            }
            UserNameWritingTo = writerUserName;
            var usr = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
            try
            {
                StoreWritingTo = UserManager.SearchStore(storeName);
            }
            catch
            {
                Error = "No such store";
                return StatusCode(500);
            }
            RelationsWithStore(usr, writerUserName, out bool isClient, out bool isMan, out bool isOwner);
            if(!(isClient || isMan || isOwner))
            {
                Error = "Can't watch this";
                return StatusCode(500);
            }
            FindMessage(usr, isClient, isMan, isOwner, writerUserName);
            if (MsgToShow != null && StoreWritingTo != null)
            {
                UserManager.MarkAllDiscussionAsRead(HttpContext.Session.Id, StoreWritingTo.Name, MsgToShow);
            }
            return Page();
        }

        public IActionResult OnPost(string content, string storeWritingTo, string userNameWritingTo)
        {
            if (!UserManager.IsLoggedIn(HttpContext.Session.Id))
            {
                return StatusCode(500);
            }
            var usr = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
            try
            {
                StoreWritingTo = UserManager.SearchStore(storeWritingTo);
            }
            catch
            {
                Error = "No such store";
                return StatusCode(500);
            }
            RelationsWithStore(usr, userNameWritingTo, out bool isClient, out bool isMan, out bool isOwner);
            if (!(isClient || isMan || isOwner))
            {
                Error = "Can't write this";
                return StatusCode(500);
            }
            FindMessage(usr, isClient, isMan, isOwner, userNameWritingTo);
            if (MsgToShow == null)
            {
                UserManager.WriteMessage(HttpContext.Session.Id, storeWritingTo, content);
                return RedirectToPage("./MessageView", new { storeWritingTo, userNameWritingTo});
            }
            else
            {
                UserManager.MessageReply(HttpContext.Session.Id, MsgToShow, storeWritingTo, content);
                return Page();
            }
        }
    }
}