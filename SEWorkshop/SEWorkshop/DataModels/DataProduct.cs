using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataProduct : DataModel<Product>
    {
        public DataStore Store => new DataStore(InnerModel.Store);
        public string Name => InnerModel.Name;
        public string Description => InnerModel.Description;
        public string Category => InnerModel.Category;
        public double Price => InnerModel.Price;
        public int Quantity => InnerModel.Quantity;
        public IReadOnlyCollection<DataReview> Reviews => InnerModel.Reviews.Select(rev => new DataReview(rev)).ToList().AsReadOnly();

        public double PriceAfterDiscount => InnerModel.PriceAfterDiscount();

        public DataProduct(Product product) : base(product) { }
    }
}
