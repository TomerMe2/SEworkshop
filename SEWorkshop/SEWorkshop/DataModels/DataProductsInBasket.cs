using SEWorkshop.Models;

namespace SEWorkshop.DataModels
{
    public class DataProductsInBasket : DataModel<ProductsInBasket>
    {
        public DataProduct Product => new DataProduct(InnerModel.Product);
        public DataBasket Basket => new DataBasket(InnerModel.Basket);
        public int Quantity => InnerModel.Quantity;

        public DataProductsInBasket(ProductsInBasket products) : base(products) { }
    }
}