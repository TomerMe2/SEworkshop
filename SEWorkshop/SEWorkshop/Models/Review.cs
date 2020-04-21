using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Review
    {
        public LoggedInUser Writer { get; private set; }
        public Product Product { get; private set; }

        public Review(LoggedInUser writer, Product product)
        {
            Writer = writer;
            Product = product;
        }
    }
}
