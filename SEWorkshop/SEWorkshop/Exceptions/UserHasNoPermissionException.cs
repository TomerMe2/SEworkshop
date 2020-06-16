namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserHasNoPermissionException : TradingSystemException
    {
        public UserHasNoPermissionException() { }
        public UserHasNoPermissionException(string message) : base(message) { }
        public UserHasNoPermissionException(string message, System.Exception inner) : base(message, inner) { }
        protected UserHasNoPermissionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "You don't have permission";
    }
}