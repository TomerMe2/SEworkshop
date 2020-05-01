using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataLoggedInUser : DataUser
    {
        public enum Authorizations
        {
            Products,
            Owner,
            Manager,
            Authorizing,
            Replying,
            Watching
        }

        public IReadOnlyCollection<DataStore> Owns => InnerLoggedInUser.Owns.Select(store => new DataStore(store)).ToList().AsReadOnly();
        public IReadOnlyDictionary<DataStore, ICollection<Authorizations>> Manages => InnerLoggedInUser.Manages.Select(item =>
                                                           (new DataStore(item.Key),
                                                           (ICollection<Authorizations>)item.Value.Select(auth => (Authorizations)auth).ToList()))
                                                           .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        public IReadOnlyList<DataReview> Reviews => InnerLoggedInUser.Reviews.Select(review =>
                                                                              new DataReview(review)).ToList().AsReadOnly();
        public IReadOnlyList<DataMessage> Messages => InnerLoggedInUser.Messages.Select(message =>
                                                                                new DataMessage(message)).ToList().AsReadOnly();
        public string Username => InnerLoggedInUser.Username;
        public byte[] Password => InnerLoggedInUser.Password;
        private LoggedInUser InnerLoggedInUser { get; }

        public DataLoggedInUser(LoggedInUser usr) : base(usr)
        {
            InnerLoggedInUser = usr;
        }
    }
}
