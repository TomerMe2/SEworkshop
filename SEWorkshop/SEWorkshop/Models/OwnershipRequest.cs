using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.Models
{
    public class OwnershipRequest
    {
        public Store Store { get; private set; }

         public ICollection<(LoggedInUser, Boolean)> Answers{ get; private set; }
        public LoggedInUser Owner { get; private set; }
        public LoggedInUser NewOwner { get; private set; }
        public OwnershipRequest(Store store, LoggedInUser owner, LoggedInUser newOwner)
        {
            Store = store;
            Owner = owner;
            NewOwner = newOwner;
            Answers = new List<(LoggedInUser, Boolean)>();
            foreach (var ow in store.Owners.Values)
            {
                if (ow != owner)
                {
                    Message m = new Message(ow, store, "Ownership request", true, null);
                    Answers.Add((ow, false));
                }
                else
                {
                    Answers.Add((ow, true));
                }

            }
        }
        public Boolean IsApproved()
        {
            foreach(var answer in Answers)
            {
                if (answer.Item2 == false)
                    return false;
            }
            return true;
        }

        public void Answer(LoggedInUser owner, Boolean descision)
        {
            foreach (var answer in Answers)
            {
                if (answer.Item1 == owner)
                {
                    Answers.Remove(answer);
                    Answers.Add((owner, descision));
                }
            }
        }
    }
}
