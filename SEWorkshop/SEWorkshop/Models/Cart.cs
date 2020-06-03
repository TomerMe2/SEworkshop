using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Carts")]
    public class Cart
    {
        public ICollection<Basket> Baskets { get; private set; }
        [ForeignKey("Users"), Key]
        public User User { get; private set; }

        public Cart(User user)
        {
            Baskets = new List<Basket>();
            User = user;
        }
    }
}
