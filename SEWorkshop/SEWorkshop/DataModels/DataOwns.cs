using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataOwns : DataModel<Owns>
    {
        public DataLoggedInUser user => new DataLoggedInUser(InnerModel.LoggedInUser);
        public DataStore store => new DataStore(InnerModel.Store);
        public DataLoggedInUser appointer => new DataLoggedInUser(InnerModel.Appointer);

        public DataOwns(Owns owns) : base(owns) { }
    }
}
