using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations;

namespace SEWorkshop.Models
{
    public class OwnershipAnswer
    {
        public virtual int Id { get; set; }
        public OwnershipRequest Request { get; private set; }
        public LoggedInUser Owner { get; private set; }
        public RequestState Answer { get; private set; }

        public OwnershipAnswer(OwnershipRequest request, LoggedInUser loggedInUser, RequestState answer)
        {
            Request = request;
            Owner = loggedInUser;
            Answer = answer;
        }
    }
}
