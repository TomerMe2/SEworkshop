using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataUser
    {
        public DataCart Cart => new DataCart(InnerUser.Cart);
        private User InnerUser { get; }

        public DataUser(User usr)
        {
            InnerUser = usr;
        }
    }
}
