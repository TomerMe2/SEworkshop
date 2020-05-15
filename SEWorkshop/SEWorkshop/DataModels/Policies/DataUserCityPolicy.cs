using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;

namespace SEWorkshop.DataModels.Policies
{
    public class DataUserCityPolicy : DataPolicy
    {
        private UserCityPolicy InnerCityPol { get; }
        public string RequiredCity => InnerCityPol.RequiredCity;

        public DataUserCityPolicy(UserCityPolicy pol) : base(pol)
        {
            InnerCityPol = pol;
        }
    }
}
