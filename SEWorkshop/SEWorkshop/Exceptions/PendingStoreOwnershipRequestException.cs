using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class PendingStoreOwnershipRequestException : System.Exception
    {

        public PendingStoreOwnershipRequestException() { }
        public PendingStoreOwnershipRequestException(string message) : base(message) { }
        public PendingStoreOwnershipRequestException(string message, System.Exception inner) : base(message, inner) { }
        protected PendingStoreOwnershipRequestException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Pending Store Ownership Request";
    }
}
