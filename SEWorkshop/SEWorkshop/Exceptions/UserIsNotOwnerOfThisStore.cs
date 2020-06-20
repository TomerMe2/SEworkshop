namespace SEWorkshop.Exceptions
{
    public class UserIsNotOwnerOfThisStore : TradingSystemException
    {
        public UserIsNotOwnerOfThisStore() { }
        public UserIsNotOwnerOfThisStore(string message) : base(message) { }
        public UserIsNotOwnerOfThisStore(string message, System.Exception inner) : base(message, inner) { }
        protected UserIsNotOwnerOfThisStore(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "User is not owner of this store";
    }
}
