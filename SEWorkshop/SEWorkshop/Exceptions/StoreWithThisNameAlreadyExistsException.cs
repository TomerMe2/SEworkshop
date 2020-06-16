namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class StoreWithThisNameAlreadyExistsException : TradingSystemException
    {
        public StoreWithThisNameAlreadyExistsException() { }
        public StoreWithThisNameAlreadyExistsException(string message) : base(message) { }
        public StoreWithThisNameAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected StoreWithThisNameAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Store with this name already exists in the system";
    }
}