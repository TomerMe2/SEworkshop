using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    public class NegativeInventoryException : Exception
    {
        public NegativeInventoryException() { }
        public NegativeInventoryException(string message) : base(message) { }
        public NegativeInventoryException(string message, System.Exception inner) : base(message, inner) { }
        protected NegativeInventoryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
