using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.DataModels
{
    public class DataGuestUser : DataUser
    {
        public int Id { get; }

        public DataGuestUser(GuestUser guestUsr) : base(guestUsr)
        {
            Id = guestUsr.Id;
        }
    }
}
