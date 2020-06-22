using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataManages : DataModel<Manages>
    {
        public DataLoggedInUser user => new DataLoggedInUser(InnerModel.LoggedInUser);
        public DataStore store => new DataStore(InnerModel.Store);
        public DataLoggedInUser appointer => new DataLoggedInUser(InnerModel.Appointer);

        public DataManages(Manages manages) : base(manages) { }
    }
}
