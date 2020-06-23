using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations;

namespace SEWorkshop.Models
{
    public class OwnershipAnswer
    {
        public virtual int Id { get; set; }
        public virtual LoggedInUser Owner { get; set; }
        public virtual string Username { get; set; }
        public virtual OwnershipRequest Request { get; set; }
        public virtual int RequestId { get; set; }

        public virtual RequestState Answer { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public OwnershipAnswer()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        public OwnershipAnswer(OwnershipRequest request, LoggedInUser loggedInUser, RequestState answer)
        {
            Request = request;
            RequestId = request.Id;
            Owner = loggedInUser;
            Username = loggedInUser.Username;
            Answer = answer;
        }
    }
}
