﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Exceptions
{
    public class BasketNotInSystemException : Exception
    {
        public BasketNotInSystemException() { }
        public BasketNotInSystemException(string message) : base(message) { }
        public BasketNotInSystemException(string message, System.Exception inner) : base(message, inner) { }
        protected BasketNotInSystemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Basket does not exist in the system";
    }
}
