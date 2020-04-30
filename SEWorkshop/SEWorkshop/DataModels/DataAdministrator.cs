using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SEWorkshop.DataModels
{
    class DataAdministrator : DataUser
    {
        public IReadOnlyCollection<DataPurchase> PurchasesToView => InnerAdmin.PurchasesToView.Select(prchs =>
                                                                        new DataPurchase(prchs)).ToList().AsReadOnly();
        private Administrator InnerAdmin { get; }

        public DataAdministrator(Administrator admin) : base(admin)
        {
            InnerAdmin = admin;
        }

    }
}
