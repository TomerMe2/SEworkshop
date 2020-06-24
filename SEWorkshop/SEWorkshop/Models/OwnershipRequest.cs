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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public OwnershipRequest()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
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
                    ow.LoggedInUser.OwnershipAnswers.Add(answer);
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

        public void Answer(LoggedInUser owner, RequestState decision)
        {
            var currAnswer = Answers.FirstOrDefault(ans => ans.Owner.Username.Equals(owner.Username));
            currAnswer.Answer = decision;
        }

        public bool IsDenied() => ((from ans in Answers
            where ans.Owner.Username != Owner.Username && ans.Answer == RequestState.Denied
            select ans).ToList().Count() > 0);

        public bool IsPending() => ((from ans in Answers
            where ans.Owner.Username != Owner.Username && ans.Answer == RequestState.Pending
            select ans).ToList().Count() > 0);
    }

}
