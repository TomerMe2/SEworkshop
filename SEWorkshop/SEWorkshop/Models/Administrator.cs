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

        public static readonly string ADMIN_USER_NAME = "admin";
        public ICollection<Purchase> PurchasesToView { get; set; }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Administrator()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
            
        }

        public Administrator(string username, byte[] password) : base(username, password)
        {
            PurchasesToView = new List<Purchase>();

        }

    }
}
