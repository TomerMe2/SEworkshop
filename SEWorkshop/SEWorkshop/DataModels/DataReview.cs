using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataReview : DataModel<Review>
    {
        public DataLoggedInUser Writer => new DataLoggedInUser(InnerModel.Writer);
        public string Description => InnerModel.Description;

        public DataReview(Review review) : base(review) { }
    }
}
