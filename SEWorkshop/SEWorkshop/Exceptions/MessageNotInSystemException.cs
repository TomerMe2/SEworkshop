namespace SEWorkshop.Exceptions
{
    public class MessageNotInSystemException : SystemException
    {
        public MessageNotInSystemException() { }
        public MessageNotInSystemException(string message) : base(message) { }
        public MessageNotInSystemException(string message, System.Exception inner) : base(message, inner) { }
        protected MessageNotInSystemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Message does not exist in the system";
    }
}
