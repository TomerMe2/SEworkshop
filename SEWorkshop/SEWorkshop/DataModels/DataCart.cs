using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataCart
    {
        public IReadOnlyCollection<DataBasket> Baskets => InnerCart.Baskets.Select(bskt => new DataBasket(bskt)).ToList().AsReadOnly();
        private Cart InnerCart{ get; }

        public DataCart(Cart cart)
        {
            InnerCart = cart;
        }
    }
}
