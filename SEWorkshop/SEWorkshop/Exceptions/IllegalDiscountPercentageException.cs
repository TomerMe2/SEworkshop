namespace SEWorkshop.Exceptions
{
    public class IllegalDiscountPercentageException : SystemException
    {
        public IllegalDiscountPercentageException() { }
        public IllegalDiscountPercentageException(string message) : base(message) { }
        public IllegalDiscountPercentageException(string message, System.Exception inner) : base(message, inner) { }
        protected IllegalDiscountPercentageException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Illegal discount percentage";
        
    }
}