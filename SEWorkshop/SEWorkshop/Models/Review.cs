using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Reviews")]
    public class Review
    {
        [ForeignKey("Users"), Key, Column(Order = 0)]
        public LoggedInUser Writer { get; private set; }
        public string Description;
        [Key, Column(Order = 1)]
        public Product Product;

        public Review(LoggedInUser writer, string description, Product product)
        {
            Writer = writer;
            Description = description;
            Product = product;
        }
    }
}
