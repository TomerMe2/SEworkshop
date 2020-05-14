using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;


namespace SEWorkshop.DataModels.Policies
{
    public class DataUserCountryPolicy : DataPolicy
    {
        private UserCountryPolicy InnerCityPol { get; }
        public string RequiredCountry => InnerCityPol.RequiredCountry;

        public DataUserCountryPolicy(UserCountryPolicy pol) : base(pol)
        {
            InnerCityPol = pol;
        }
    }
}
