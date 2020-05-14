using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataPolicy : DataModel<Policy>
    {
        public (DataPolicy, Operator)? InnerPolicy
        {
            get
            {
                if (InnerModel.InnerPolicy == null)
                {
                    return null;
                }
                return (new DataPolicy(InnerModel.InnerPolicy.Value.Item1), InnerModel.InnerPolicy.Value.Item2);
            }
        }

        public Store Store => InnerModel.Store;

        public DataPolicy(Policy policy) : base(policy) { }
    }

}
