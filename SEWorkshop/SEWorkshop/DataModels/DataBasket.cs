using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataBasket
    {
        public DataStore Store => new DataStore(InnerBasket.Store);
        public IReadOnlyCollection<(DataProduct, int)> Products =>
                InnerBasket.Products.Select((tup) => (new DataProduct(tup.Item1), tup.Item2)).ToList().AsReadOnly();
        private Basket InnerBasket { get; }

        public DataBasket(Basket basket)
        {
            InnerBasket = basket;
        }

    }
}
