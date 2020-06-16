namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class NoProductsToFilterException : TradingSystemException
    {
        public NoProductsToFilterException() { }
        public NoProductsToFilterException(string message) : base(message) { }
        public NoProductsToFilterException(string message, System.Exception inner) : base(message, inner) { }
        protected NoProductsToFilterException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "No products to filter";
    }
}