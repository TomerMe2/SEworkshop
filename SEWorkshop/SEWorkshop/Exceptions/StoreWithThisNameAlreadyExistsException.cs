namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class StoreWithThisNameAlreadyExistsException : System.Exception
    {
        public StoreWithThisNameAlreadyExistsException() { }
        public StoreWithThisNameAlreadyExistsException(string message) : base(message) { }
        public StoreWithThisNameAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected StoreWithThisNameAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}