using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class ProductViewModel : PageModel
    {
        public DataProduct Product { get; private set; }
        public IUserManager UserManager { get; private set; }
        public string destPath { get; set; }
        public bool Authorized { get; private set; }

        public ProductViewModel(DataProduct product, IUserManager userManager, bool authorized)
        {
            Product = product;
            UserManager = userManager;
            destPath = "./Store/"+Product.Store.Name;
            Authorized = authorized;
        }

        public void OnGet()
        {

        }
    }
}