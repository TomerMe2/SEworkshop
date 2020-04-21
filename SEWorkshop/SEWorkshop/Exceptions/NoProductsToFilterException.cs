namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class NoProductsToFilterException : System.Exception
    {
        public NoProductsToFilterException() { }
        public NoProductsToFilterException(string message) : base(message) { }
        public NoProductsToFilterException(string message, System.Exception inner) : base(message, inner) { }
        protected NoProductsToFilterException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}