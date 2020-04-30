using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    class DataReview
    {
        public DataUser Writer { get => new DataUser(InnerReview.Writer); }
        public string Description { get => InnerReview.Description; }
        private Review InnerReview { get; }

        public DataReview(Review review)
        {
            InnerReview = review;
        }
    }
}
