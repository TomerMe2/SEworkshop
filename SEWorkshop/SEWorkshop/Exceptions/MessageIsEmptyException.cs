namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class MessageIsEmptyException : SystemException
    {
        public MessageIsEmptyException() { }
        public MessageIsEmptyException(string message) : base(message) { }
        public MessageIsEmptyException(string message, System.Exception inner) : base(message, inner) { }
        protected MessageIsEmptyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Message is empty";
    }
}