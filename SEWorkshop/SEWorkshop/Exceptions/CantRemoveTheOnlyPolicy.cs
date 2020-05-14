using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    class CantRemoveTheOnlyPolicy : Exception
    {
        public CantRemoveTheOnlyPolicy() { }
        public CantRemoveTheOnlyPolicy(string message) : base(message) { }
        public CantRemoveTheOnlyPolicy(string message, System.Exception inner) : base(message, inner) { }
        protected CantRemoveTheOnlyPolicy(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Can't remove the only policy";
    }
}
