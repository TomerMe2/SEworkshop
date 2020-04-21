namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserAlreadyLoggedInException : System.Exception
    {
        public UserAlreadyLoggedInException() { }
        public UserAlreadyLoggedInException(string message) : base(message) { }
        public UserAlreadyLoggedInException(string message, System.Exception inner) : base(message, inner) { }
        protected UserAlreadyLoggedInException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}