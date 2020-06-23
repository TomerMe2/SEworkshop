using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    public class AlwaysTruePolicy : Policy
    {

        public AlwaysTruePolicy() : base() { }

        public AlwaysTruePolicy(Store store) : base(store) { }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            return true;
        }
    }
}
