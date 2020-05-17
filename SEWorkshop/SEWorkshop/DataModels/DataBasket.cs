using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataBasket : DataModel<Basket>
    {
        public DataStore Store => new DataStore(InnerModel.Store);
        public IReadOnlyCollection<(DataProduct, int)> Products =>
                InnerModel.Products.Select((tup) => (new DataProduct(tup.Item1), tup.Item2)).ToList().AsReadOnly();

        public double PriceWithoutDiscount => InnerModel.PriceWithoutDiscount();
        public double PriceAfterDiscount => InnerModel.PriceAfterDiscount();

        public DataBasket(Basket basket) : base(basket) { }

    }
}
