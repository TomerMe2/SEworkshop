using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    class DataCart
    {
        public IReadOnlyCollection<DataBasket> Baskets { get => InnerCart.Baskets.Select(bskt => new DataBasket()).ToList().AsReadOnly(); }
        private Cart InnerCart{ get; }

        public DataCart(Cart cart)
        {
            InnerCart = cart;
        }
    }
}
