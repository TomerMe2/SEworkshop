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
        public virtual int ProductId { get; set; }
        public virtual LoggedInUser Writer { get; set; }
        public virtual string Description { get; set; }
        public virtual Product Product { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Review()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public Review(LoggedInUser writer, string description, Product product)
        {
            Writer = writer;
            Description = description;
            Product = product;
            Username = writer.Username;
            ProductId = product.Id;
        }
    }
}
