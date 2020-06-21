using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class ProductsInBasket
    {
        public virtual int BasketId { get; set; }
        public virtual Basket Basket { get; set; }
        public virtual int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public virtual int Quantity { get; set; }

        public ProductsInBasket()
        {
            /*Basket = null!;
            Product = null!;*/
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
