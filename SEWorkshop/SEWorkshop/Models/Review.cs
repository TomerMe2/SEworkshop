using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class Review
    {
        public LoggedInUser Writer { get; private set; }
        public string Description;

        public Review(LoggedInUser writer, string description)
        {
            Writer = writer;
            Description = description;
        }
    }
}
