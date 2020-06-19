using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class ProductsInBasket
    {
        public virtual int BasketId { get; set; }
        public virtual Basket Basket { get; private set; }
        public virtual int ProductId { get; set; }
        public virtual Product Product { get; private set; }
        public virtual int Quantity { get; set; }

        private ProductsInBasket() : base()
        {
            Basket = null!;
            Product = null!;
        }

        public ProductsInBasket(Basket basket, Product product, int quantity)
        {
            Basket = basket;
            Product = product;
            Quantity = quantity;
            ProductId = product.Id;
        }
    }
}
