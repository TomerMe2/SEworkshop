using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Review
    {
        public RegisteredUser Writer { get; private set; }
        public Product Product { get; private set; }

        public Review(RegisteredUser writer, Product product)
        {
            Writer = writer;
            Product = product;
        }
    }
}
