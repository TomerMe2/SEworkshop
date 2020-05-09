namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UserIsAlreadyStoreOwnerException : System.Exception
    {
        public UserIsAlreadyStoreOwnerException() { }
        public UserIsAlreadyStoreOwnerException(string message) : base(message) { }
        public UserIsAlreadyStoreOwnerException(string message, System.Exception inner) : base(message, inner) { }
        protected UserIsAlreadyStoreOwnerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "This user is already owner of this store";
    }
}