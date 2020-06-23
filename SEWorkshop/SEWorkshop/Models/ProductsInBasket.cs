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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public ProductsInBasket()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

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
