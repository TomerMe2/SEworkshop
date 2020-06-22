using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels.Policies
{
    public class DataSystemDayPolicy : DataPolicy
    {
        private SystemDayPolicy InnerDayPol { get; }
        public Weekday CantBuyIn => InnerDayPol.CantBuyIn;

        public DataSystemDayPolicy(SystemDayPolicy pol) : base(pol)
        {
            InnerDayPol = pol;
        }

        public override string ToString()
        {
            return "Can't purchase on "+ CantBuyIn.ToString();
        }
    }
}
