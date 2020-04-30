using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    class DataBasket
    {
        public DataStore Store { get => new DataStore(); }
        public IReadOnlyCollection<(DataProduct, int)> Products { get =>
                InnerBasket.Products.Select((product, amount) => (new DataProduct(), amount)).ToList().AsReadOnly(); }
        private Basket InnerBasket { get; }

        public DataBasket(Basket basket)
        {
            InnerBasket = basket;
        }

    }
}
