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

        public int MsgId { get; private set; }
        public DataMessage? MsgToShow { get; private set; }
        public DataStore? StoreWritingTo { get; private set; }
        
        public MessageViewModel(IUserManager userManager)
        {
            UserManager = userManager;
            Error = "";
            CompleteMsg = "";
        }

        private void FindMessage()
        {
            //find message to display
            var usr = UserManager.GetDataLoggedInUser(HttpContext.Session.Id);
            //find his messages as client
            MsgToShow = usr.Messages.FirstOrDefault(currMsg => currMsg.Id == MsgId);
            if (MsgToShow == null)
            {
                foreach (var owns in usr.Owns)
                {
                    MsgToShow = owns.Messages.FirstOrDefault(currMsg => currMsg.Id == MsgId);
                }
                if (MsgToShow == null)
                {
                    foreach (var manages in usr.Manages)
                    {
                        if (manages.Value.Contains(SEWorkshop.Authorizations.Replying))
                        {
                            MsgToShow = manages.Key.Messages.FirstOrDefault(currMsg => currMsg.Id == MsgId);
                        }
                    }
                }
            }
        }

        public IActionResult OnGet(string storeWritingTo, string msgId)
        {
            if (!UserManager.IsLoggedIn(HttpContext.Session.Id))
            {
                return StatusCode(500);
            }
            try
            {
                MsgId = int.Parse(msgId);
            }
            catch
            {
                Error = "answering on msg id is not a number";
                return StatusCode(500);
            }
            if (MsgId < -1)
            {
                Error = "invalid answering on msg id";
                return StatusCode(500);
            }
            if (MsgId != -1)
            {

                FindMessage();
                if (MsgToShow == null)
                {
                    Error = "can't find message to show";
                    return StatusCode(500);
                }
            }
            try
            {
                StoreWritingTo = UserManager.SearchStore(storeWritingTo);
            }
            catch
            {
                Error = "No such store";
                return StatusCode(500);
            }
            if (MsgToShow != null && StoreWritingTo != null)
            {
                UserManager.MarkAllDiscussionAsRead(HttpContext.Session.Id, StoreWritingTo.Name, MsgToShow);
            }
            return Page();
        }

        public IActionResult OnPost(string content, string storeWritingTo, string answeringOnMsgId)
        {
            //answeringOnMsgId should always be the first message in the talk
            if (!UserManager.IsLoggedIn(HttpContext.Session.Id))
            {
                return StatusCode(500);
            }
            try
            {
                MsgId = int.Parse(answeringOnMsgId);
            }
            catch
            {
                Error = "answering on msg id is not a number";
                return StatusCode(500);
            }
            try
            {
                StoreWritingTo = UserManager.SearchStore(storeWritingTo);
            }
            catch
            {
                Error = "No such store";
                return StatusCode(500);
            }
            if (MsgId < -1)
            {
                Error = "invalid answering on msg id";
                return StatusCode(500);
            }
            if (MsgId == -1)
            {
                int msgId = UserManager.WriteMessage(HttpContext.Session.Id, storeWritingTo, content);
                return RedirectToPage("./MessageView", new { storeWritingTo, msgId});
            }
            else
            {
                FindMessage();
                if (MsgToShow == null)
                {
                    Error = "can't find message to answer on";
                    return StatusCode(500);
                }
                UserManager.MessageReply(HttpContext.Session.Id, MsgToShow, storeWritingTo, content);
                return Page();
            }
        }
    }
}