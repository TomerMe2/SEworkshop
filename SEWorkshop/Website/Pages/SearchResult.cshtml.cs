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
        public int PriceMin { get; private set; }
        public int PriceMax { get; private set; }
        public string CategoryFilter { get; private set; }
        public string Error { get; private set; }
        public bool Filter { get; private set; }
        public IEnumerable<DataProduct> SearchResult { get; private set; }
        public IEnumerable<DataProduct> FilteredSearchResult { get; private set; }
        public ICollection<DataProduct> CurrSearchResult { get; private set; }

        public SearchResultModel(IUserManager userManager)
        {
            Console.WriteLine("here");
            UserManager = userManager;
            RequestedSearchString = "";
            ActualSearchString = "";
            SearchCategory = "";
            PriceMin = -1;
            PriceMax = -1;
            CategoryFilter = "";
            Error = "";
            Filter = false;
            SearchResult = new List<DataProduct>();
            FilteredSearchResult = new List<DataProduct>();
            try
            {
                userManager.Register("wello", "1234");
                userManager.Login("wello", "1234");
                userManager.OpenStore("nini");
                userManager.AddProduct("nini", "prod1", "some description1", "cat1", 123.7, 7);
                userManager.AddProduct("nini", "prod3", "some description1", "cat1", 250, 7);
                userManager.AddProduct("nini", "prod2", "some LONGGGGGGGGGGGGGGGGGGGGGGGGG descriptionnnnnnnnnnnnnnn2", "cat2", 999999, 1);
            }
            catch { }
        }

        public void OnPost(string searchText, string searchCategory)
        { 
            Filter = false;
            Error = "";
            PriceMin = -1;
            PriceMax = -1;
            FilteredSearchResult = new List<DataProduct>();
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
            Deep_Copy();
            ActualSearchString = searchString;
        }

        public void OnPostEdit(string minPriceText, string maxPriceText, string filterCategoryText)
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
                if (filterCategoryText == null)
                    CategoryFilter = "";
                else
                    CategoryFilter = filterCategoryText;
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

        private void Deep_Copy()
        {
            CurrSearchResult = new List<DataProduct>();
            foreach (DataProduct dp in SearchResult)
            {
                CurrSearchResult.Add(dp);
            }
        }
    }
}