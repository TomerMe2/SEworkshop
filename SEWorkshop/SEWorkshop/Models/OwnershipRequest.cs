﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using SEWorkshop.Enums;
using SEWorkshop.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SEWorkshop.Models
{
    public class OwnershipRequest
    {
        [ForeignKey("Stores"), Key, Column(Order = 0)]
        public Store Store { get; private set; }
        public ICollection<(LoggedInUser, RequestState )> Answers{ get; private set; }
        public LoggedInUser Owner { get; private set; }
        [ForeignKey("Users"), Key, Column(Order = 1)]
        public LoggedInUser NewOwner { get; private set; }
        public OwnershipRequest(Store store, LoggedInUser owner, LoggedInUser newOwner)
        {
            Store = store;
            Owner = owner;
            NewOwner = newOwner;
            Answers = new List<(LoggedInUser, RequestState)>();
            foreach (var ow in store.Ownership)
            {
                owner.WriteMessage(store,"ownership request", false);
                Answers.Add((ow.LoggedInUser, RequestState.Pending));
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

        public bool IsDenied() => ((from ans in Answers
            where ans.Item1.Username != Owner.Username && ans.Item2 == RequestState.Denied
            select ans).ToList().Count() > 0);

        public bool IsPending() => ((from ans in Answers
            where ans.Item1.Username != Owner.Username && ans.Item2 == RequestState.Pending
            select ans).ToList().Count() > 0);
    }

}
