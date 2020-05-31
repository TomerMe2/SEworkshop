using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    [Table("Admins")]
    public class Administrator : LoggedInUser
    {
        public ICollection<Purchase> PurchasesToView { get; private set; }

        public Administrator(string username, byte[] password) : base(username, password)
        {
            PurchasesToView = new List<Purchase>();
        }
    }
}
