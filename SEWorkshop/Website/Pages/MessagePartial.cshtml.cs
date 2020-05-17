using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;

namespace Website.Pages
{
    public class MessagePartialModel : PageModel
    {
        public DataMessage Message { get; }
        public bool IsClient { get; }

        public MessagePartialModel(DataMessage msg, bool isClient)
        {
            Message = msg;
            IsClient = isClient;
        }

        public void OnGet()
        {

        }
    }
}