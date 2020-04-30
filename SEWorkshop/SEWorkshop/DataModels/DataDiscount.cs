using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SEWorkshop.DataModels
{
    class DataDiscount
    {
        public enum DiscountType { visible }

        //cast from Models.Discount.DiscountType to DataModels.DataDiscount.DiscountType
        public DiscountType DisType { get => (DiscountType)InnerDiscount.DisType; }
        public int Code { get => InnerDiscount.Code; }
        public IReadOnlyCollection<DataProduct> Products { get => InnerDiscount.Products.Select(prod =>
                                                                                new DataProduct(prod)).ToList().AsReadOnly(); }
        private Discount InnerDiscount { get; }

        public DataDiscount(Discount discount)
        {
            InnerDiscount = discount;
        }
    }
}
