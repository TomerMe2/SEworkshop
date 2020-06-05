using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Cart
    {
        public ICollection<Basket> Baskets { get; private set; }
        public User User { get; private set; }

        public Cart(User user)
        {
            Baskets = new List<Basket>();
            User = user;
        }
    }
}
