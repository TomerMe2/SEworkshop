using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty(SupportsGet = true)]
        public string Username { get; set; }

        private readonly ILogger<IndexModel> _logger;
        public IUserManager UserManager { get; }

        public IndexModel(ILogger<IndexModel> logger, IUserManager userManager)
        {
            _logger = logger;
            UserManager = userManager;
            Username = "";
        }

        public void OnGet()
        {

        }
    }
}
