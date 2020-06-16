namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserAlreadyLoggedInException : TradingSystemException
    {
        public UserAlreadyLoggedInException() { }
        public UserAlreadyLoggedInException(string message) : base(message) { }
        public UserAlreadyLoggedInException(string message, System.Exception inner) : base(message, inner) { }
        protected UserAlreadyLoggedInException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "You are already logged in";
    }
}