using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    class DataUser
    {
        public DataCart Cart { get => new DataCart(InnerUser.Cart); }
        private User InnerUser { get; }

        public DataUser(User usr)
        {
            InnerUser = usr;
        }
    }
}
