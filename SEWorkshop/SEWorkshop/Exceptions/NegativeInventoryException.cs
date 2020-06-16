namespace SEWorkshop.Exceptions
{
    public class NegativeInventoryException : TradingSystemException
    {
        public NegativeInventoryException() { }
        public NegativeInventoryException(string message) : base(message) { }
        public NegativeInventoryException(string message, System.Exception inner) : base(message, inner) { }
        protected NegativeInventoryException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Negative inventory";
    }
}
