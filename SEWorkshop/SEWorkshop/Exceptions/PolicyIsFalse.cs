namespace SEWorkshop.Exceptions
{
    public class PolicyIsFalse : SystemException
    {
        public PolicyIsFalse() { }
        public PolicyIsFalse(string message) : base(message) { }
        public PolicyIsFalse(string message, System.Exception inner) : base(message, inner) { }
        protected PolicyIsFalse(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Can't purchase because of the store's policy";
    }
}
