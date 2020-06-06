using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Review
    {
        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string ProdName { get; set; }
        public virtual string StoreName { get; set; }
        public virtual LoggedInUser Writer { get; set; }
        public virtual string Description { get; set; }
        public virtual Product Product { get; set; }

        public Review()
        {

        }
        public Review(LoggedInUser writer, string description, Product product)
        {
            Writer = writer;
            Description = description;
            Product = product;
        }
    }
}
