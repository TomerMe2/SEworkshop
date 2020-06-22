using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;
using System.Linq;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels.Policies
{
    public abstract class DataPolicy : DataModel<Policy>
    {
        public DataPolicy? InnerPolicy
        {
            get
            {
                return InnerModel.InnerPolicy != null ? CreateDataPolFromPol(InnerModel.InnerPolicy) : null;
            }
        }

        public Operator? InnerOperator
        {
            get
            {
                return InnerModel.InnerOperator;
            }
        }

        public Store Store => InnerModel.Store;

        public DataPolicy(Policy policy) : base(policy) { }

        public static DataPolicy CreateDataPolFromPol(Policy pol)
        {
            return pol switch
            {
                AlwaysTruePolicy p => new DataAlwaysTruePolicy(p),
                SingleProductQuantityPolicy p => new DataSingleProductQuantityPolicy(p),
                SystemDayPolicy p => new DataSystemDayPolicy(p),
                UserCityPolicy p => new DataUserCityPolicy(p),
                UserCountryPolicy p => new DataUserCountryPolicy(p),
                WholeStoreQuantityPolicy p => new DataWholeStoreQuantityPolicy(p),
                _ => throw new Exception("should not get here"),
            };
        }
    }

}
