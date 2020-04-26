namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserIsNotMangerOfTheStoreException : System.Exception
    {
        public UserIsNotMangerOfTheStoreException() { }
        public UserIsNotMangerOfTheStoreException(string message) : base(message) { }
        public UserIsNotMangerOfTheStoreException(string message, System.Exception inner) : base(message, inner) { }
        protected UserIsNotMangerOfTheStoreException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}