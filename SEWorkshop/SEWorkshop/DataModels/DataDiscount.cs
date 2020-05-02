using SEWorkshop.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SEWorkshop.Enums;

namespace SEWorkshop.DataModels
{
    public class DataDiscount : DataModel<Discount>
    {

        //cast from Models.Discount.DiscountType to DataModels.DataDiscount.DiscountType
        public DiscountType DisType => InnerModel.DisType;
        public int Code => InnerModel.Code;
        public IReadOnlyCollection<DataProduct> Products => InnerModel.Products.Select(prod =>
                                                                                new DataProduct(prod)).ToList().AsReadOnly();

        public DataDiscount(Discount discount) : base(discount) { }
    }
}
