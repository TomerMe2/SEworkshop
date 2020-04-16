using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class GuestUser : User
    {
        public GuestUser() : base() { }
        
        RegisteredUser Register(string username, string password) {
            RegisteredUser user = new RegisteredUser(username, password);
            //TODO: Serialization
            return user;
        }

        RegisteredUser Login(string username, string password) {
            //TODO: Deserialization Requested User
            //TODO: Check valid information
            //TODO: Serialization Current User
            return null;
        }
    }
}
