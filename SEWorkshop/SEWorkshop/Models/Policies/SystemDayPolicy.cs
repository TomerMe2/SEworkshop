using System;
using SEWorkshop.Enums;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models.Policies
{
    public class SystemDayPolicy : Policy
    {
        public virtual Weekday CantBuyIn { get; set; }

        protected SystemDayPolicy() : base()
        {
            CantBuyIn = default;
        }

        public SystemDayPolicy (Store store, Weekday cantBuyIn) : base(store)
        {
            CantBuyIn = cantBuyIn;
        }

        protected override bool IsThisPolicySatisfied(User user, Address address)
        {
            return (int)DateTime.Now.DayOfWeek != (int)CantBuyIn;
        }
    }
}
