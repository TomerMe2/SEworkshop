using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    class DataStore
    {
        public IReadOnlyCollection<DataProduct> Products { get => InnerStore.Products.Select(prod =>
                        new DataProduct(prod)).ToList().AsReadOnly(); }
        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> Managers { get => InnerStore.Managers
                .Select((usr1, usr2) =>
                    (new DataLoggedInUser(), new DataLoggedInUser()))
                .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        }
        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> Owners { get => InnerStore.Owners
                .Select((usr1, usr2) =>
                    (new DataLoggedInUser(), new DataLoggedInUser()))
                .ToDictionary(tup => tup.Item1, tup => tup.Item2);
        }
        public IReadOnlyList<DataMessage> Messages { get => InnerStore.Messages.Select(msg => new DataMessage()).ToList().AsReadOnly(); }
        public IReadOnlyCollection<DataDiscount> Discounts { get => InnerStore.Discounts.Select(discount =>
                                                                            new DataDiscount()).ToList().AsReadOnly(); }
        public bool IsOpen { get => InnerStore.IsOpen; }
        public string Name { get => InnerStore.Name; }
        public DataPolicy Policy { get => new DataPolicy(); }
        public IReadOnlyCollection<DataPurchase> Purchases { get => InnerStore.Purchases.Select(prchs => new DataPurchase()).ToList().AsReadOnly(); }
        private Store InnerStore { get; }

        public DataStore(Store store)
        {
            InnerStore = store;
        }
    }
}
