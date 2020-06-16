namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserAlreadyExistsException : SystemException
    {
        public UserAlreadyExistsException() { }
        public UserAlreadyExistsException(string message) : base(message) { }
        public UserAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected UserAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "User with this name already exists";
    }
}