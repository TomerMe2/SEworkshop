using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Policies;

namespace SEWorkshop.DataModels.Policies
{
    public class DataAlwaysTruePolicy : DataPolicy
    {
        public DataAlwaysTruePolicy(AlwaysTruePolicy pol) : base(pol) { }
    }
}
