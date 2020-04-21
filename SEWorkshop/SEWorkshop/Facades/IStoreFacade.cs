using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    interface IStoreFacade
    {
        public Store CreateStore(LoggedInUser owner, string storeName);
        public ICollection<Store> BrowseStores();
        public ICollection<Store> SearchStore(Func<Store, bool> pred);
        public bool IsProductExists(Product product);
        public ICollection<Product> SearchProducts(Func<Product, bool> pred);
        public ICollection<Product> FilterProducts(ICollection<Product> products, Func<Product, bool> pred);
    }
}
