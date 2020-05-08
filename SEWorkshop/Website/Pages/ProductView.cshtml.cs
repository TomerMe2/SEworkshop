using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;
using SEWorkshop.Models;
using SEWorkshop.ServiceLayer;

namespace Website.Pages
{
    public class ProductViewModel : PageModel
    {
        public DataProduct Product { get; private set; }

        public ProductViewModel(DataProduct product)
        {
            Product = product;
        }

        public void OnGet()
        {

        }
    }
}