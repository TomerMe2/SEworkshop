using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataCart : DataModel<Cart>
    {
        public int Id => InnerModel.Id;
        public IReadOnlyCollection<DataBasket> Baskets => InnerModel.Baskets.Select(bskt => new DataBasket(bskt)).ToList().AsReadOnly();

        public DataCart(Cart cart) : base(cart) { }
    }
}
