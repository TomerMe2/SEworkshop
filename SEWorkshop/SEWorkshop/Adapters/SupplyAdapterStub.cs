﻿using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models;

namespace SEWorkshop.Adapters
{
    class SupplyAdapterStub : ISupplyAdapter
    {
        public void Supply(ICollection<(Product, int)> products, string city, string street, string houseNum)
        {
            
        }

        public bool CanSupply(ICollection<(Product, int)> products, string city, string street, string houseNum)
        {
            return true;
        }
    }
}
