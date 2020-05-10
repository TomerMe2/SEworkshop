namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class AuthorizationDoesNotExistException : System.Exception
    {
        public AuthorizationDoesNotExistException() { }
        public AuthorizationDoesNotExistException(string message) : base(message) { }
        public AuthorizationDoesNotExistException(string message, System.Exception inner) : base(message, inner) { }
        protected AuthorizationDoesNotExistException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Authorization does not exist";
    }
}