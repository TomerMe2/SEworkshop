namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserIsAlreadyStoreManagerException : SystemException
    {
        public UserIsAlreadyStoreManagerException() { }
        public UserIsAlreadyStoreManagerException(string message) : base(message) { }
        public UserIsAlreadyStoreManagerException(string message, System.Exception inner) : base(message, inner) { }
        protected UserIsAlreadyStoreManagerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "This user is already manager of this store";
    }
}