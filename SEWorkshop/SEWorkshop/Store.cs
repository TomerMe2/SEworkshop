using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop
{
    class Store
    {
        public ICollection<Product> Products { get; private set; }
        public ICollection<RegisteredUser> Managers { get; private set; }
        public ICollection<RegisteredUser> Owners { get; private set; }
        public IList<Message> Messages { get; private set; }
        public bool IsOpen { get; private set; }
        public string Name { get; private set; }
        public double? Rating { get; private set; } = null;   // Rating is null when no one has rated the store
        private int NumberOfRaters { get; set; } = 0;   // No one has rated the store before it was created

        public Store(RegisteredUser owner, string name)
        {
            Products = new List<Product>();
            Managers = new List<RegisteredUser>();
            Owners = new List<RegisteredUser>() { owner };
            Messages = new List<Message>();
            IsOpen = true;
            Name = name;
        }

        public void CloseStore()
        {
            IsOpen = false;
        }

        public void Rate(double rate)
        {
            Rating = (NumberOfRaters * Rating + rate) / (NumberOfRaters + 1);
            NumberOfRaters++;
        }


    }
}
