using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataUser : DataModel<User>
    {
        public DataCart Cart => new DataCart(InnerModel.Cart);

        public virtual string Username => InnerModel is LoggedInUser ? ((LoggedInUser)InnerModel).Username : "";
        public DataUser(User usr) : base(usr) { }
    }
}
