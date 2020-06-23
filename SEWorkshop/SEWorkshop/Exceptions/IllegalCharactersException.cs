namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class IllegalCharactersException : TradingSystemException
    {
        public IllegalCharactersException() { }
        public IllegalCharactersException(string message) : base(message) { }
        public IllegalCharactersException(string message, System.Exception inner) : base(message, inner) { }
        protected IllegalCharactersException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Username/password contain illegal characters such as: Space";
    }
}
