using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    [Table("ATPolicies")]
    public class AlwaysTruePolicy : Policy
    {
        public AlwaysTruePolicy(Store store) : base(store) { }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            return true;
        }
    }
}
