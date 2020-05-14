using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.ServiceLayer;
using SEWorkshop.DataModels;
using SEWorkshop.Exceptions;

namespace Website.Pages.Shared
{

    public class CRUDproductModel : PageModel
    {
        private IUserManager UserManager { get; }
        public DataProduct? Product { get; private set; }
        public string ProductName { get; private set; }
        public string ErrorMsg { get; private set; }

        public CRUDproductModel(IUserManager userManager) {
            UserManager = userManager;
            ProductName = "";

            ErrorMsg = "";

        }
        public void OnGet()
        {

        }
    }
}