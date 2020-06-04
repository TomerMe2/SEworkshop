using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.Enums;
using System.ComponentModel.DataAnnotations;

namespace SEWorkshop.Models
{
    [Table("OwnershipAnswers")]
    public class OwnershipAnswer
    {
        [ForeignKey("OwnershipRequests"), Key, Column(Order = 0)]
        public OwnershipRequest Request { get; private set; }
        [ForeignKey("Users"), Key, Column(Order = 1)]
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
