using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models.Policies
{
    public class SystemDayPolicy : Policy
    {
        public DayOfWeek CantBuyIn { get; set; }

        public SystemDayPolicy (Store store, DayOfWeek cantBuyIn) : base(store)
        {
            CantBuyIn = cantBuyIn;
        }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            return DateTime.Now.DayOfWeek != CantBuyIn;
        }
    }
}
