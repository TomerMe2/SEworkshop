namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserHasNoPermissionException : System.Exception
    {
        public UserHasNoPermissionException() { }
        public UserHasNoPermissionException(string message) : base(message) { }
        public UserHasNoPermissionException(string message, System.Exception inner) : base(message, inner) { }
        protected UserHasNoPermissionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}