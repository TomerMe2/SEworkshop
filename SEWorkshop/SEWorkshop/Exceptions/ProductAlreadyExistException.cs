namespace SEWorkshop.Exceptions
{
    public class ProductAlreadyExistException : TradingSystemException
    {
        public ProductAlreadyExistException() { }
        public ProductAlreadyExistException(string message) : base(message) { }
        public ProductAlreadyExistException(string message, System.Exception inner) : base(message, inner) { }
        protected ProductAlreadyExistException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Product already exists";
    }
}
