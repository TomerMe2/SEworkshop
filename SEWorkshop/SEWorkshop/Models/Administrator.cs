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


        public Administrator(string username, byte[] password) : base(username, password)
        {
            //PurchasesToView = (ICollection<Purchase>)dbContext.Purchases.Select(purhcase => true);
            PurchasesToView = new List<Purchase>();
        }
    }
}
