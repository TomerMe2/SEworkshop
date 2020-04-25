﻿namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserIsNotLoggedInException : System.Exception
    {
        public UserIsNotLoggedInException() { }
        public UserIsNotLoggedInException(string message) : base(message) { }
        public UserIsNotLoggedInException(string message, System.Exception inner) : base(message, inner) { }
        protected UserIsNotLoggedInException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}