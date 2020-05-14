using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Policies
{
    public class UserCityPolicy : Policy
    {
        public string RequiredCity { get; set; }

        public UserCityPolicy(Store store, string requiredCity) : base(store)
        {
            RequiredCity = requiredCity;
        }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            return address.City.Equals(RequiredCity);
        }
    }
}
