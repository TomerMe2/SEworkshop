﻿using SEWorkshop.Models.Discounts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels
{
    public abstract class DataDiscount : DataModel<Discount>
    {
        public DateTime Deadline => InnerModel.Deadline;
        public int DiscountId => InnerModel.DiscountId;
        public DataStore Store { get; }

        public Operator? opeartor => InnerModel.Op;

        public DataDiscount? leftChild => InnerModel.leftChild != null ? CreateDataFromDiscount(InnerModel.leftChild) : null;
        public DataDiscount? rightChild => InnerModel.rightChild != null ? CreateDataFromDiscount(InnerModel.rightChild) : null;

        public DataDiscount(Discount discount) : base(discount)
        {
            Store = new DataStore(InnerModel.Store);
        }
        
        public static DataDiscount CreateDataFromDiscount(Discount dis)
        {
            return dis switch
            {
                SpecificProducDiscount d => new DataOpenDiscount(d),
                ProductCategoryDiscount d => new DataProductCategoryDiscount(d),
                BuyOverDiscount d => new DataBuyOverDiscount(d),
                BuySomeGetSomeDiscount d => new DataBuySomeGetSomeDiscount(d),
                /*PrimitiveDiscount d => new DataPrimitiveDiscount(d),*/
                ComposedDiscount d => new DataComposedDiscount(d),
                _ => throw new Exception("Should not get here"),
            };
        }
    }
}
