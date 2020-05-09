namespace SEWorkshop.Exceptions
{
    [System.Serializable]
    public class UsernameOrPasswordAreEmpty : System.Exception
    {
        public UsernameOrPasswordAreEmpty() { }
        public UsernameOrPasswordAreEmpty(string message) : base(message) { }
        public UsernameOrPasswordAreEmpty(string message, System.Exception inner) : base(message, inner) { }
        protected UsernameOrPasswordAreEmpty(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        public override string ToString() => "Username or password are empty";
    }
}
