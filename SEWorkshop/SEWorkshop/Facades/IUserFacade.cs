using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    interface IUserFacade
    {
        void CreateBasket(Cart cart, Store store);
        
        void AddProductToBasket(Product proudct);

        void StoreInfo(Store store);

        void SearchProducts();

        void CartInfo(Cart cart);

        void Purchase(Basket basket);
    }
}