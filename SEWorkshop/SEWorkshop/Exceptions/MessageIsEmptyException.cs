namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class MessageIsEmptyException : System.Exception
    {
        public MessageIsEmptyException() { }
        public MessageIsEmptyException(string message) : base(message) { }
        public MessageIsEmptyException(string message, System.Exception inner) : base(message, inner) { }
        protected MessageIsEmptyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}