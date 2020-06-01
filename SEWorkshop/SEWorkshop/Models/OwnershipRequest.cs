using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;


namespace SEWorkshop.Models
{
    public class OwnershipRequest
    {
        public Store Store { get; private set; }
        public ICollection<(LoggedInUser, RequestState )> Answers{ get; private set; }
        public LoggedInUser Owner { get; private set; }
        public LoggedInUser NewOwner { get; private set; }
        public OwnershipRequest(Store store, LoggedInUser owner, LoggedInUser newOwner)
        {
            Store = store;
            Owner = owner;
            NewOwner = newOwner;
            Answers = new List<(LoggedInUser, RequestState)>();
            foreach (var ow in store.Owners.Keys)
            {
                owner.WriteMessage(store,"ownership request", false);
                Answers.Add((ow, RequestState.Pending));
            }
        }
        public bool IsApproved()
        {
            if (GetRequestState() == RequestState.Denied)
            {
                return false;
            }
            if (GetRequestState() == RequestState.Pending)
            {
                throw new PendingStoreOwnershipRequestException();
            }
            return true;
        }

        public RequestState GetRequestState()
        {
            foreach (var answer in Answers)
            {
                if (answer.Item1.Username != Owner.Username)
                {
                    if (answer.Item2 == RequestState.Denied)
                        return RequestState.Denied;
                    if (answer.Item2 == RequestState.Pending)
                    {
                        return RequestState.Pending;
                    }
                }

            }
            return RequestState.Approved;
        }

        public void Answer(LoggedInUser owner, RequestState decision)
        {
            foreach (var answer in Answers)
            {
                if (answer.Item1 == owner)
                {
                    Answers.Remove(answer);
                    Answers.Add((owner, decision));
                    return;
                }
            }
        }

       }

}
