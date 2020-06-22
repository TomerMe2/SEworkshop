using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    public class UserCityPolicy : Policy
    {
        public virtual string RequiredCity { get; set; }

        public UserCityPolicy() : base()
        {
            //RequiredCity = "";
        }

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
