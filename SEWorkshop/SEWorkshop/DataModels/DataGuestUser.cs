using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    class DataGuestUser : DataUser
    {
        public DataGuestUser(GuestUser guestUsr) : base(guestUsr) { }
    }
}
