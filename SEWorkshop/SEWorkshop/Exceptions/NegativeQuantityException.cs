﻿namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class NegativeQuantityException : SystemException
    {
        public NegativeQuantityException() { }
        public NegativeQuantityException(string message) : base(message) { }
        public NegativeQuantityException(string message, System.Exception inner) : base(message, inner) { }
        protected NegativeQuantityException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Negative Quantity";
    }
}