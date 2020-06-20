namespace SEWorkshop.Exceptions
{
    class CantRemoveTheOnlyPolicy : TradingSystemException
    {
        public CantRemoveTheOnlyPolicy() { }
        public CantRemoveTheOnlyPolicy(string message) : base(message) { }
        public CantRemoveTheOnlyPolicy(string message, System.Exception inner) : base(message, inner) { }
        protected CantRemoveTheOnlyPolicy(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Can't remove the only policy";
    }
}
