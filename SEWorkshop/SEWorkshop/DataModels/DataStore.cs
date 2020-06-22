using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.DataModels.Policies;

namespace SEWorkshop.DataModels
{
    public class DataStore : DataModel<Store>
    {
        public IReadOnlyCollection<DataProduct> Products => InnerModel.Products.Select(prod =>
                        new DataProduct(prod)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataManages> Management => InnerModel.Management
                .Select((item) =>
                    (new DataManages(item))).ToList();
        public IReadOnlyCollection<DataOwns> Ownership => InnerModel.Ownership
                .Select((item) =>
                    (new DataOwns(item))).ToList();
        public IReadOnlyList<DataMessage> Messages => InnerModel.Messages.Select(msg => new DataMessage(msg)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataDiscount> Discounts => InnerModel.Discounts.Where(discount => discount.Father is null).Select(discount =>
                                                                            DataDiscount.CreateDataFromDiscount(discount)).ToList().AsReadOnly();
        public IReadOnlyCollection<DataPolicy> Policies => InnerModel.Policies.Select(policy =>
                                                                            DataPolicy.CreateDataPolFromPol(policy)).ToList().AsReadOnly();

        public IReadOnlyDictionary<DataLoggedInUser, DataLoggedInUser> OwnershipRequests => InnerModel.OwnershipRequests
            .Select((item) =>
                (new DataLoggedInUser(item.NewOwner), new DataLoggedInUser(item.Owner)))
            .ToDictionary(tup => tup.Item1, tup => tup.Item2);


        public bool IsOpen => InnerModel.IsOpen;
        public string Name => InnerModel.Name;
        public DataPolicy Policy => DataPolicy.CreateDataPolFromPol(InnerModel.Policy);
        public IReadOnlyCollection<DataPurchase> Purchases => InnerModel.Purchases.Select(prchs =>
                                                                    new DataPurchase(prchs)).ToList().AsReadOnly();

        public DataStore(Store store) : base(store) { }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        
        public bool IsManager(DataLoggedInUser user)
        {
            return user.IsManager(Management);
        }

        public bool IsOwner(DataLoggedInUser user)
        {
            return user.IsOwner(Ownership);        
        }
    }
}
