using SEWorkshop.Enums;
using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataOwnershipRequest : DataModel<OwnershipRequest>
    {

        public DataStore Store => new DataStore(InnerModel.Store);
        public IReadOnlyCollection<(DataLoggedInUser, RequestState)> Answers =>
            InnerModel.Answers.Select(tup => (new DataLoggedInUser(tup.Item1), tup.Item2)).ToList().AsReadOnly();
        public DataLoggedInUser Owner => new DataLoggedInUser(InnerModel.Owner);
        public DataLoggedInUser NewOwner => new DataLoggedInUser(InnerModel.NewOwner);

        public DataOwnershipRequest(OwnershipRequest request) : base(request) { }
    }
}
