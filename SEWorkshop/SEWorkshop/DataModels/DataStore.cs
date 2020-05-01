using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    public class DataStore
    {
        public IReadOnlyCollection<DataProduct> Products => InnerStore.Products.Select(prod =>
                        new DataProduct(prod)).ToList().AsReadOnly();
        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> Managers => InnerStore.Managers
                .Select((item) =>
                    (new DataLoggedInUser(item.Key), new DataLoggedInUser(item.Value)))
                .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> Owners => InnerStore.Owners
                .Select((item) =>
                    (new DataLoggedInUser(item.Key), new DataLoggedInUser(item.Value)))
                .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        public IReadOnlyList<DataMessage> Messages => InnerStore.Messages.Select(msg => new DataMessage(msg)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataDiscount> Discounts => InnerStore.Discounts.Select(discount =>
                                                                            new DataDiscount(discount)).ToList().AsReadOnly();
        public bool IsOpen => InnerStore.IsOpen;
        public string Name => InnerStore.Name;
        public DataPolicy Policy => new DataPolicy(InnerStore.Policy);
        public IReadOnlyCollection<DataPurchase> Purchases => InnerStore.Purchases.Select(prchs =>
                                                                    new DataPurchase(prchs)).ToList().AsReadOnly();
        private Store InnerStore { get; }

        public DataStore(Store store)
        {
            InnerStore = store;
        }
    }
}
