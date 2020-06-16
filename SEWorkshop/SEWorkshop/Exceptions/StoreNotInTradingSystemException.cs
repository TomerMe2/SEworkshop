namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class StoreNotInTradingSystemException : SystemException
    {
        public StoreNotInTradingSystemException() { }
        public StoreNotInTradingSystemException(string message) : base(message) { }
        public StoreNotInTradingSystemException(string message, System.Exception inner) : base(message, inner) { }
        protected StoreNotInTradingSystemException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Store does not exist in the trading system";
    }
}
