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
    public class SearchResultModel : PageModel
    {
        private IUserManager UserManager { get; }
        public string RequestedSearchString { get; private set; }
        public string SearchCategory { get; private set; }
        public string ActualSearchString { get; private set; }
        public IEnumerable<DataProduct> SearchResult { get; private set; }
        
        public SearchResultModel(IUserManager userManager)
        {
            UserManager = userManager;
            RequestedSearchString = "";
            ActualSearchString = "";
            SearchCategory = "";
            SearchResult = new List<DataProduct>();
        }

        public void OnPost()
        {
            RequestedSearchString = Request.Form["searchText"];
            SearchCategory = Request.Form["searchCategory"];
            string searchString = RequestedSearchString;
            SearchResult = SearchCategory switch
            {
                "product_name" => UserManager.SearchProductsByName(ref searchString),
                "product_category" => UserManager.SearchProductsByCategory(ref searchString),
                "product_keywords" => UserManager.SearchProductsByKeywords(ref searchString),
                _ => new List<DataProduct>()
            };
            ActualSearchString = searchString;
        }
    }
}