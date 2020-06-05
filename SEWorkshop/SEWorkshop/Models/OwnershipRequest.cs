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
        public Store Store { get; private set; }
        public ICollection<OwnershipAnswer> Answers{ get; private set; }
        public LoggedInUser Owner { get; private set; }
        public LoggedInUser NewOwner { get; private set; }
        private AppDbContext DbContext { get; }
        public OwnershipRequest(Store store, LoggedInUser owner, LoggedInUser newOwner, AppDbContext dbContext)
        {
            DbContext = dbContext;
            Store = store;
            Owner = owner;
            NewOwner = newOwner;
            Answers = (IList<OwnershipAnswer>)DbContext.OwnershipAnswers.Select(ans => ans.Request.Equals(this));
            foreach(var ow in store.Ownership)
            {
                if(!HasAnswered(ow.LoggedInUser))
                    Answers.Add(new OwnershipAnswer(this, ow.LoggedInUser, RequestState.Pending));
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
