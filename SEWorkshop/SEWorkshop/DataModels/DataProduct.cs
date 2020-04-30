using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    class DataProduct
    {
        public DataStore Store { get => new DataStore(); }
        public string Name { get => InnerProduct.Name; }
        public string Description { get => InnerProduct.Description; }
        public string Category { get => InnerProduct.Category; }
        public double Price { get => InnerProduct.Price; }
        public int Quantity { get => InnerProduct.Quantity; }
        public IReadOnlyCollection<DataReview> Reviews { get => InnerProduct.Reviews.Select(rev => new DataReview()).ToList().AsReadOnly(); }
        private Product InnerProduct { get; }

        public DataProduct(Product product)
        {
            InnerProduct = product;
        }
    }
}
