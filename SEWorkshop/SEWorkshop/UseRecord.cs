using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Enums;

namespace SEWorkshop
{
    /*
     * THIS CLASS IS NOT INSIDE A SPECIFIC NAMESPACE BECUASE UserManager SHOULD HANDLE IT, AND IT'S SAVED IN THE DB.
     * THIS CLASS BREAKS THE ABSTRACTION.
     */


    public class UseRecord
    {
        public virtual int Id { get; set; }
        public virtual string HashedSessionId { get; set; }
        public virtual DateTime Timestamp { get; set; }
        public virtual KindOfUser Kind { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. FOR EF.
        public UseRecord()
#pragma warning restore CS8618 // Non-nullable field is uninitialized.
        {

        }

        public UseRecord(string hashedSessionId, DateTime timestamp, KindOfUser kind)
        {
            HashedSessionId = hashedSessionId;
            Timestamp = timestamp;
            Kind = kind;
        }

    }
}
