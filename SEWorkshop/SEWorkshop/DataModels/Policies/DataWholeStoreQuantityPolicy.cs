using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;


namespace SEWorkshop.DataModels.Policies
{
    public class DataWholeStoreQuantityPolicy : DataPolicy
    {
        private WholeStoreQuantityPolicy InnerQuantityPolicy { get; }
        public int MinQuantity => InnerQuantityPolicy.MinQuantity;
        public int MaxQuantity => InnerQuantityPolicy.MaxQuantity;

        public DataWholeStoreQuantityPolicy(WholeStoreQuantityPolicy pol) : base(pol)
        {
            InnerQuantityPolicy = pol;
        }
    }
}
