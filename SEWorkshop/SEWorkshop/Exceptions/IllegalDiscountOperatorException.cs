namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class IllegalDiscountOperatorException : System.Exception
    {
        public IllegalDiscountOperatorException() { }
        public IllegalDiscountOperatorException(string message) : base(message) { }
        public IllegalDiscountOperatorException(string message, System.Exception inner) : base(message, inner) { }
        protected IllegalDiscountOperatorException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Illegal Discount Operator";
    }
}
