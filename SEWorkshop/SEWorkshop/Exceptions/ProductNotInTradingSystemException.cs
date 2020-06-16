namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class ProductNotInTradingSystemException : TradingSystemException
    {
        public ProductNotInTradingSystemException() { }
        public ProductNotInTradingSystemException(string message) : base(message) { }
        public ProductNotInTradingSystemException(string message, System.Exception inner) : base(message, inner) { }
        protected ProductNotInTradingSystemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Product does not exist in the system";
    }
}