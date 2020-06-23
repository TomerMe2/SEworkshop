using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    public class CantLoginAsDemoExeption : SystemException
    {
        public CantLoginAsDemoExeption() { }
        public CantLoginAsDemoExeption(string message) : base(message) { }
        public CantLoginAsDemoExeption(string message, System.Exception inner) : base(message, inner) { }
        protected CantLoginAsDemoExeption(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Can't login as DEMO user";
    }
}
