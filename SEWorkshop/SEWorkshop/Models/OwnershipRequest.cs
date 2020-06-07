using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;


namespace SEWorkshop.Models
{
    public class OwnershipRequest
    {
        public virtual int Id { get; set; }
        public virtual string StoreName { get; private set; }
        public virtual Store Store { get; private set; }
        public virtual ICollection<OwnershipAnswer> Answers{ get; private set; }
        public virtual string OwnerUsername { get; private set; }
        public virtual LoggedInUser Owner { get; private set; }
        public virtual string NewOwnerUsername { get; private set; }
        public virtual LoggedInUser NewOwner { get; private set; }

        private OwnershipRequest()
        {
            Store = null!;
            Owner = null!;
            NewOwner = null!;
            Answers = new List<OwnershipAnswer>();
            StoreName = "";
            OwnerUsername = "";
            NewOwnerUsername = "";
        }

        public OwnershipRequest(Store store, LoggedInUser owner, LoggedInUser newOwner)
        {
            Store = store;
            StoreName = Store.Name;
            OwnerUsername = owner.Username;
            NewOwnerUsername = newOwner.Username;
            Owner = owner;
            NewOwner = newOwner;
            Answers = new List<OwnershipAnswer>();
            foreach(var ow in store.Ownership)
            {
                if (!HasAnswered(ow.LoggedInUser))
                {
                    OwnershipAnswer answer = new OwnershipAnswer(this, ow.LoggedInUser, RequestState.Pending);
                    Answers.Add(answer);
                    owner.OwnershipAnswers.Add(answer);
                }
            }
        }

        public RequestState GetRequestState()
        {
            if (IsDenied())
            {
                return RequestState.Denied;
            }
            if (IsPending())
            {
                return RequestState.Pending;
            }
           
            return RequestState.Approved;
        }

        public bool HasAnswered(LoggedInUser owner) => ((from ans in Answers
            where ans.Owner.Username != Owner.Username
            select ans).ToList().Count() > 0);

        //TODO: SEE THAT THIS FUNCTION WAS CHANGED. NOW THERE'S AN ITERATOR CHANGE INSIDE FOREACH
        public void Answer(LoggedInUser owner, RequestState decision)
        {
            foreach (var answer in Answers)
            {
                if (answer.Owner.Equals(owner))
                {
                    Answers.Remove(answer);
                    Answers.Add(new OwnershipAnswer(this, owner, decision));
                    return;
                }
            }
        }

        public bool IsDenied() => ((from ans in Answers
            where ans.Owner.Username != Owner.Username && ans.Answer == RequestState.Denied
            select ans).ToList().Count() > 0);

        public bool IsPending() => ((from ans in Answers
            where ans.Owner.Username != Owner.Username && ans.Answer == RequestState.Pending
            select ans).ToList().Count() > 0);
    }

}
