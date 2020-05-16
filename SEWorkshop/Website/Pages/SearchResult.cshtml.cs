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
        public IUserManager UserManager { get; }
        public string RequestedSearchString { get; private set; }
        public string SearchCategory { get; private set; }
        public string ActualSearchString { get; private set; }
        public int? PriceMin { get; private set; }
        public int? PriceMax { get; private set; }
        public string? CategoryFilter { get; private set; }
        public string ErrorMsg { get; private set; }
        public IEnumerable<DataProduct> SearchResult { get; private set; }

        public SearchResultModel(IUserManager userManager)
        {
            UserManager = userManager;
            SearchResult = new List<DataProduct>();
            RequestedSearchString = "";
            ActualSearchString = "";
            SearchCategory = "";
            ErrorMsg = "";
        }

        public IActionResult OnPostAddToCartAsync(string StoreName, string ProductName, string Quantity)
        {
            int quantity;
            try
            {
                quantity = int.Parse(Quantity);
            }
            catch(Exception)
            {
                ErrorMsg = "Quantity must be a positive number";
                return new PageResult();
            }
            if(quantity <= 0)
            {
                ErrorMsg = "Quantity must be a positive number";
                return new PageResult();
            }
            try
            {
                UserManager.AddProductToCart(HttpContext.Session.Id, StoreName, ProductName, int.Parse(Quantity));
            }
            catch(Exception e)
            {
                ErrorMsg = e.ToString();
                return new PageResult();
            }
            return RedirectToPage("./Store", new { storeName = StoreName });
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
                ErrorMsg = "Invalid Price Range";
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
    }
}