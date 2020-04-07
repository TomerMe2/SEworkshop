using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class User
    {
        public Cart Cart { get; private set; }

        public User()
        {
            Cart = new Cart(this);
        }
    }
}
