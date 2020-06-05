using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SEWorkshop.Enums;

namespace SEWorkshop.Models
{
    public class ProductsInBasket
    {
        public virtual int BasketId { get; set; }
        public virtual Basket Basket { get; private set; }
        public virtual string ProductName { get; set; }
        public virtual string StoreName { get; set; }
        public virtual Product Product { get; private set; }
        public virtual int Quantity { get; private set; }

        public ProductsInBasket()
        {

        }

        public ProductsInBasket(Basket basket, Product product, int quantity)
        {
            Basket = basket;
            Product = product;
            Quantity = quantity;
        }
    }
}
