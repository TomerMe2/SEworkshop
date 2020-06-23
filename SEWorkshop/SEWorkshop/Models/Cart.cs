using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public class Cart
    {
        public virtual int Id { get; set; }
        public virtual ICollection<Basket> Baskets { get; set; }
        public virtual string Username { get; set; }
        public virtual LoggedInUser? LoggedInUser { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Cart()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }

        public Cart(User user)
        {
            Baskets = new List<Basket>();
            if (user is LoggedInUser)
            {
                LoggedInUser = (LoggedInUser)user;
                Username = LoggedInUser.Username;
            }
            else
            {
                Username = "";
            }
        }
    }
}
