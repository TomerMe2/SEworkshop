﻿using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataBasket : DataModel<Basket>
    {
        public DataStore Store => new DataStore(InnerModel.Store);
        public IReadOnlyCollection<DataProductsInBasket> Products =>
                InnerModel.Products.Select((tup) => new DataProductsInBasket(tup)).ToList().AsReadOnly();

        public double PriceWithoutDiscount => InnerModel.PriceWithoutDiscount();
        public double PriceAfterDiscount => InnerModel.PriceAfterDiscount();
        public int Id => InnerModel.Id;

        public DataBasket(Basket basket) : base(basket) { }

    }
}
