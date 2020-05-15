using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataStore : DataModel<Store>
    {
        public IReadOnlyCollection<DataProduct> Products => InnerModel.Products.Select(prod =>
                        new DataProduct(prod)).ToList().AsReadOnly();
        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> Managers => InnerModel.Managers
                .Select((item) =>
                    (new DataLoggedInUser(item.Key), new DataLoggedInUser(item.Value)))
                .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> Owners => InnerModel.Owners
                .Select((item) =>
                    (new DataLoggedInUser(item.Key), new DataLoggedInUser(item.Value)))
                .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        public IReadOnlyList<DataMessage> Messages => InnerModel.Messages.Select(msg => new DataMessage(msg)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataDiscount> Discounts => InnerModel.Discounts.Select(discount =>
                                                                            DataDiscount.CreateDataFromDiscount(discount)).ToList().AsReadOnly();
        public bool IsOpen => InnerModel.IsOpen;
        public string Name => InnerModel.Name;
        public DataPolicy Policy => new DataPolicy(InnerModel.Policy);
        public IReadOnlyCollection<DataPurchase> Purchases => InnerModel.Purchases.Select(prchs =>
                                                                    new DataPurchase(prchs)).ToList().AsReadOnly();

        public DataStore(Store store) : base(store) { }
    }
}
