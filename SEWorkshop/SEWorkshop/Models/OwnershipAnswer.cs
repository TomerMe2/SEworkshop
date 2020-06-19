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

        private OwnershipAnswer()
        {
            Request = null!;
            Owner = null!;
            Answer = default;
            Username = "";
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
