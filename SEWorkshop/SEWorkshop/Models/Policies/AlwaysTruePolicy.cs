using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Policies
{
    public class AlwaysTruePolicy : Policy
    {
        public AlwaysTruePolicy(Store store) : base(store) { }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            return true;
        }
    }
}
