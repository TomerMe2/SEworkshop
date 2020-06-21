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

        public Cart()
        {
            /*Baskets = new List<Basket>();
            LoggedInUser = null;
            Username = "";*/
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
