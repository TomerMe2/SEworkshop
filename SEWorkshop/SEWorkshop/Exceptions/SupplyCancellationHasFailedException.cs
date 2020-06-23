namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class SupplyCancellationHasFailedException : TradingSystemException
    {
        public SupplyCancellationHasFailedException() { }
        public SupplyCancellationHasFailedException(string message) : base(message) { }
        public SupplyCancellationHasFailedException(string message, System.Exception inner) : base(message, inner) { }
        protected SupplyCancellationHasFailedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Basket is empty";
    }
}