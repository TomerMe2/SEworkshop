namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class TradingSystemException : System.Exception
    {
        public TradingSystemException() { }
        public TradingSystemException(string message) : base(message) { }
        public TradingSystemException(string message, System.Exception inner) : base(message, inner) { }
        protected TradingSystemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "System Exception";
    }
}