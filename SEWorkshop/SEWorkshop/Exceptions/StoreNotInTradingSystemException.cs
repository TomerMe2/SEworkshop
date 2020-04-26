namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class StoreNotInTradingSystemException : System.Exception
    {
        public StoreNotInTradingSystemException() { }
        public StoreNotInTradingSystemException(string message) : base(message) { }
        public StoreNotInTradingSystemException(string message, System.Exception inner) : base(message, inner) { }
        protected StoreNotInTradingSystemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}