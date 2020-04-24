using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    class PurchaseFailedException : Exception
    {
        public PurchaseFailedException() { }
        public PurchaseFailedException(string message) : base(message) { }
        public PurchaseFailedException(string message, System.Exception inner) : base(message, inner) { }
        protected PurchaseFailedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
