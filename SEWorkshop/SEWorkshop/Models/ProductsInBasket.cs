using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    [Table("ProductsInBaskets")]
    public class ProductsInBasket
    {
        [ForeignKey("Baskets"), Key, Column(Order = 0)]
        public Basket Basket { get; private set; }
        [ForeignKey("Products"), Key, Column(Order = 1)]
        public Product Product { get; private set; }
        public int Quantity { get; private set; }

        public ProductsInBasket(Basket basket, Product product, int quantity)
        {
            Basket = basket;
            Product = product;
            Quantity = quantity;
        }
    }
}
