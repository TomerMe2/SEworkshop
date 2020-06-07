using NLog;
using SEWorkshop.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;
using System;

namespace SEWorkshop.Models
{
    public class Administrator : LoggedInUser
    {
        public ICollection<Purchase> PurchasesToView { get; private set; }

        private Administrator() : base()
        {
            PurchasesToView = new List<Purchase>();
        }

        public Administrator(string username, byte[] password) : base(username, password)
        {
            PurchasesToView = new List<Purchase>();
        }
    }
}
