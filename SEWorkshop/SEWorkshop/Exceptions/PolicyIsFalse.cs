using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    public class PolicyIsFalse : Exception
    {
        public PolicyIsFalse() { }
        public PolicyIsFalse(string message) : base(message) { }
        public PolicyIsFalse(string message, System.Exception inner) : base(message, inner) { }
        protected PolicyIsFalse(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Can't purchase because of the store's policy";
    }
}
