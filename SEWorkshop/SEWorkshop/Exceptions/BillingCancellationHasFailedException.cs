namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class BillingCancellationHasFailedException : TradingSystemException
    {
        public BillingCancellationHasFailedException() { }
        public BillingCancellationHasFailedException(string message) : base(message) { }
        public BillingCancellationHasFailedException(string message, System.Exception inner) : base(message, inner) { }
        protected BillingCancellationHasFailedException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Basket is empty";
    }
}