namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    class OwnershipRequestAlreadyExistsException : SystemException
    {
        public OwnershipRequestAlreadyExistsException() { }
        public OwnershipRequestAlreadyExistsException(string message) : base(message) { }
        public OwnershipRequestAlreadyExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected OwnershipRequestAlreadyExistsException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Authorization does not exist";
    }
}
