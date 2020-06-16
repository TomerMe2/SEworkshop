namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class ProductIsNotInCartException : SystemException
    {
        public ProductIsNotInCartException() { }
        public ProductIsNotInCartException(string message) : base(message) { }
        public ProductIsNotInCartException(string message, System.Exception inner) : base(message, inner) { }
        protected ProductIsNotInCartException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Product is not in the cart";
    }
}