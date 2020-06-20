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
    public class AllPurchasesModel : PageModel
    {
        public IUserManager UserManager { get; }
        public IEnumerable<DataStore> Stores { get; private set; }
        public ICollection<IReadOnlyCollection<DataPurchase>> Purchases { get; private set; }

        public AllPurchasesModel(IUserManager userManager)
        {
            UserManager = userManager;
            Purchases = new List<IReadOnlyCollection<DataPurchase>>();
            Stores = new List<DataStore>();
        }

        public void OnGet()
        {
            Stores = UserManager.BrowseStores();
            foreach(DataStore store in Stores)
            {
                Purchases.Add(store.Purchases);
            }
        }
    }
}