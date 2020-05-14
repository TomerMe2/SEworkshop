using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;


namespace SEWorkshop.DataModels.Policies
{
    public class DataSingleProductQuantityPolicy : DataPolicy
    {
        private SingleProductQuantityPolicy InnerQuantityPolicy { get; }
        public DataProduct Product => new DataProduct(InnerQuantityPolicy.Prod);
        public int MinQuantity => InnerQuantityPolicy.MinQuantity;
        public int MaxQuantity => InnerQuantityPolicy.MaxQuantity;

        public DataSingleProductQuantityPolicy(SingleProductQuantityPolicy pol) : base(pol)
        {
            InnerQuantityPolicy = pol;
        }
    }
}
