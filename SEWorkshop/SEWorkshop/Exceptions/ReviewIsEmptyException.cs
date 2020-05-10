namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class ReviewIsEmptyException : System.Exception
    {
        public ReviewIsEmptyException() { }
        public ReviewIsEmptyException(string message) : base(message) { }
        public ReviewIsEmptyException(string message, System.Exception inner) : base(message, inner) { }
        protected ReviewIsEmptyException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Review is empty";
    }
}
