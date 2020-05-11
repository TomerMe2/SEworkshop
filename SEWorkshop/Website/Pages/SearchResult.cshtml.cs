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
        public int? PriceMin { get; private set; }
        public int? PriceMax { get; private set; }
        public string? CategoryFilter { get; private set; }
        public string Error { get; private set; }
        public IEnumerable<DataProduct> SearchResult { get; private set; }

        public SearchResultModel(IUserManager userManager)
        {
            UserManager = userManager;
            SearchResult = new List<DataProduct>();
            RequestedSearchString = "";
            ActualSearchString = "";
            SearchCategory = "";
            Error = "";
        }

        public void OnPost(string searchText, string searchCategory, string minPriceText, string maxPriceText, string categoryFilterText)
        {
            bool low = false, high = false;
            if (!string.IsNullOrEmpty(categoryFilterText))
            {
                CategoryFilter = categoryFilterText;
            }
            try
            {
                if (!string.IsNullOrEmpty(minPriceText))
                {
                    if (int.Parse(minPriceText) < 0)
                        throw new ArgumentException();
                    PriceMin = int.Parse(minPriceText);
                    low = true;
                }
                if (!string.IsNullOrEmpty(maxPriceText))
                {
                    if (int.Parse(maxPriceText) < 0)
                        throw new ArgumentException();
                    PriceMax = int.Parse(maxPriceText);
                    high = true;
                }
                if ((low && high) && PriceMin > PriceMax)
                    throw new ArgumentException();
            }
            catch
            {
                Error = "Invalid Price Range";
            }


            RequestedSearchString = searchText;
            SearchCategory = searchCategory;
            string searchString = RequestedSearchString;
            SearchResult = SearchCategory switch
            {
                "product_name" => UserManager.SearchProductsByName(ref searchString),
                "product_category" => UserManager.SearchProductsByCategory(ref searchString),
                "product_keywords" => UserManager.SearchProductsByKeywords(ref searchString),
                _ => new List<DataProduct>()
            };
            ActualSearchString = searchString;
            FilterResults();
        }

        private void FilterResults()
        {
            if(PriceMax != null)
            {
                SearchResult = UserManager.FilterProducts(SearchResult.ToList(), prod => prod.Price <= PriceMax);
            }
            if (PriceMin != null)
            {
                SearchResult = UserManager.FilterProducts(SearchResult.ToList(), prod => prod.Price >= PriceMin);
            }
            if (CategoryFilter != null)
            {
                SearchResult = UserManager.FilterProducts(SearchResult.ToList(), prod => prod.Category.Equals(CategoryFilter));
            }
        }

        /*

        public void OnPostEdit(string minPriceText, string maxPriceText, string categoryFilterText)
        {
            try
            {
                if (minPriceText == null)
                    PriceMin = -1;
                else
                    PriceMin = Int32.Parse(minPriceText);
                if (maxPriceText == null)
                    PriceMax = -1;
                else
                    PriceMax = Int32.Parse(maxPriceText);
                if (categoryFilterText == null)
                    CategoryFilter = "";
                else
                    CategoryFilter = categoryFilterText;
                if (PriceMax < PriceMin && PriceMax != -1)
                    throw new Exception();
                FilteredSearchResult = UserManager.FilterProducts(CurrSearchResult, (x) =>
                {
                    bool low = true, high = true, category = true;
                    if (PriceMin >= 0) low = x.Price >= PriceMin;
                    if (PriceMax >= 0) high = x.Price <= PriceMax;
                    if (!CategoryFilter.Equals("")) category = x.Category.Equals(CategoryFilter);
                    return low && high && category;
                });
                Filter = true;
            }
            catch (Exception)
            {
                Error = "Invalid Price Range";
            }
        }
        */
    }
}