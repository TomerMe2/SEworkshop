using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels
{
    public class DataLoggedInUser : DataUser
    {
        public IReadOnlyCollection<DataStore> Owns => InnerLoggedInUser.Owns.Select(owns => new DataStore(owns.Store)).ToList().AsReadOnly();
        public IReadOnlyDictionary<DataStore, ICollection<Authorizations>> Manages => InnerLoggedInUser.Manage.Select(item =>
                                                           (new DataStore(item.Store),
                                                           (ICollection<Authorizations>)item.AuthoriztionsOfUser.Select(auth => auth.Authorization).ToList()))
                                                           .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        public IReadOnlyList<DataReview> Reviews => InnerLoggedInUser.Reviews.Select(review =>
                                                                              new DataReview(review)).ToList().AsReadOnly();
        public IReadOnlyList<DataMessage> Messages => InnerLoggedInUser.Messages.Select(message =>
                                                                                new DataMessage(message)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataOwnershipRequest> OwnershipRequests => InnerLoggedInUser.OwnershipRequests.Select(request =>
                                                                                new DataOwnershipRequest(request)).ToList().AsReadOnly();

        public override string Username => InnerLoggedInUser.Username;
        public byte[] Password => InnerLoggedInUser.Password;
        public int AmountOfUnreadMessages => InnerLoggedInUser.AmountOfUnReadMessage;
        private LoggedInUser InnerLoggedInUser { get; }

        public DataLoggedInUser(LoggedInUser usr) : base(usr)
        {
            InnerLoggedInUser = usr;
        }

        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }

        public bool IsManager(IReadOnlyCollection<DataManages> Management)
        {
            return Management.Where(mngr => mngr.user.Username.Equals(this.Username)).Count() > 0;
        }

        public bool IsOwner(IReadOnlyCollection<DataOwns> Ownership)
        {
            return Ownership.Where(ownr => ownr.user.Username.Equals(this.Username)).Count() > 0;
        }
    }
}
