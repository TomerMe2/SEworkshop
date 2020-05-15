using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    class PolicyCauseCycilicError : Exception
    {
        public PolicyCauseCycilicError() { }
        public PolicyCauseCycilicError(string message) : base(message) { }
        public PolicyCauseCycilicError(string message, System.Exception inner) : base(message, inner) { }
        protected PolicyCauseCycilicError(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Add this policy will cause cycilic error";
    }
}
