using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    public class NoPolicyInTheGivenIndex : SystemException
    {
        public NoPolicyInTheGivenIndex() { }
        public NoPolicyInTheGivenIndex(string message) : base(message) { }
        public NoPolicyInTheGivenIndex(string message, System.Exception inner) : base(message, inner) { }
        protected NoPolicyInTheGivenIndex(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "No policy found in the given index";
    }
}
