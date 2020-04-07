using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class RegisteredUser : User
    {
        public ICollection<Store> Owns { get; private set; }
        public ICollection<Store> Manages { get; private set; }
        public IList<Review> Reviews { get; private set; }
        public IList<Message> Messages { get; private set; }

        public RegisteredUser()
        {
            Owns = new List<Store>();
            Manages = new List<Store>();
            Reviews = new List<Review>();
            Messages = new List<Message>();
        }
    }
}
