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
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        public IUserManager UserManager { get; }

        public PrivacyModel(ILogger<PrivacyModel> logger, IUserManager userManager)
        {
            _logger = logger;
            UserManager = userManager;
        }

        public void OnGet()
        {
        }
    }
}
