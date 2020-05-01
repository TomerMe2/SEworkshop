using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataReview
    {
        public DataUser Writer => new DataUser(InnerReview.Writer);
        public string Description => InnerReview.Description;
        private Review InnerReview { get; }

        public DataReview(Review review)
        {
            InnerReview = review;
        }
    }
}
