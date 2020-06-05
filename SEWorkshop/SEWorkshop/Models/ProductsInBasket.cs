using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class ProductsInBasket
    {
        public Basket Basket { get; private set; }
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
