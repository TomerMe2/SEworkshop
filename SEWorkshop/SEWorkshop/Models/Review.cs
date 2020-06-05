﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Review
    {
        public virtual int Id { get; set; }
        public LoggedInUser Writer { get; private set; }
        public string Description;
        public Product Product;

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
