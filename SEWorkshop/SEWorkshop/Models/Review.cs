using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Review
    {
        public User Writer { get; private set; }
        public string Description;

        public Review(User writer, string description)
        {
            Writer = writer;
            Description = description;
        }
    }
}
