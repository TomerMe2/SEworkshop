namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserIsAlreadyStoreManagerException : System.Exception
    {
        public UserIsAlreadyStoreManagerException() { }
        public UserIsAlreadyStoreManagerException(string message) : base(message) { }
        public UserIsAlreadyStoreManagerException(string message, System.Exception inner) : base(message, inner) { }
        protected UserIsAlreadyStoreManagerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}