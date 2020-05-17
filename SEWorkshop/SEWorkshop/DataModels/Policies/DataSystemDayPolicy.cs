using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;

namespace SEWorkshop.DataModels.Policies
{
    public class DataSystemDayPolicy : DataPolicy
    {
        private SystemDayPolicy InnerDayPol { get; }
        public DayOfWeek CantBuyIn => InnerDayPol.CantBuyIn;

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
