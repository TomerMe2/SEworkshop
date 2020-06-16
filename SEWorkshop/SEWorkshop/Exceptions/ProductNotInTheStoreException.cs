namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class ProductNotInTheStoreException : TradingSystemException
    {
        public ProductNotInTheStoreException() { }
        public ProductNotInTheStoreException(string message) : base(message) { }
        public ProductNotInTheStoreException(string message, System.Exception inner) : base(message, inner) { }
        protected ProductNotInTheStoreException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Product does not exist in the store";
    }
}