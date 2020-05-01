using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataProduct
    {
        public DataStore Store => new DataStore(InnerProduct.Store);
        public string Name => InnerProduct.Name;
        public string Description => InnerProduct.Description;
        public string Category => InnerProduct.Category;
        public double Price => InnerProduct.Price;
        public int Quantity => InnerProduct.Quantity;
        public IReadOnlyCollection<DataReview> Reviews => InnerProduct.Reviews.Select(rev => new DataReview(rev)).ToList().AsReadOnly();
        private Product InnerProduct { get; }

        public DataProduct(Product product)
        {
            InnerProduct = product;
        }
    }
}
