using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Models
{
    public class User
    {
        public Cart Cart { get; private set; }

        public User()
        {
            Cart = new Cart();
        }
    }
}
