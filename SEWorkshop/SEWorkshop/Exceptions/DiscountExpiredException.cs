namespace SEWorkshop.Exceptions
{
    public class DiscountExpiredException : System.Exception
    {
        public DiscountExpiredException() { }
        public DiscountExpiredException(string message) : base(message) { }
        public DiscountExpiredException(string message, System.Exception inner) : base(message, inner) { }
        protected DiscountExpiredException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Authorization does not exist";
    }
}