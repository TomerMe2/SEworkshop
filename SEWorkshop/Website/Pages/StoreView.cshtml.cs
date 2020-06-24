using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEWorkshop.DataModels;

namespace Website.Pages
{
    public class StoreViewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public DataStore Store { get; set; }

        public StoreViewModel(DataStore store)
        {
            Store = store;
        }

        public void OnGet()
        {
        }
    }
}