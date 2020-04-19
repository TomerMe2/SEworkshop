using System;
using System.Collections.Generic;
using System.Text;

namespace SEWorkshop.Facades
{
    interface IProductManagerFacade
    {
        void AddProduct();
        
        Product GetProduct();
    }
}