using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Discounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;

namespace SEWorkshop.Models
{
    public class Product
    {
        public virtual string StoreName { get; set; }
        public virtual Store Store { get; private set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Category { get; set; }
        public virtual double Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual ICollection<ProductsInBasket> InBaskets {get ; set;}
        public virtual ICollection<Review> Reviews {get ; set;}

        public Product()
        {

        }

        public Product(Store store, string name, string description, string category, double price, int quantity)
        {
            Store = store;
            Name = name;
            Description = description;
            Category = category;
            Price = price;
            Reviews = new List<Review>();
            Quantity = quantity;
        }

        public double PriceAfterDiscount()
        {
            double price = Price;
            ICollection<ProductsInBasket> product = new List<ProductsInBasket>
                    {(new ProductsInBasket(new Basket(this.Store, new Cart(new GuestUser(new AppDbContext())), new AppDbContext()), this, 1))};
            foreach (var discount in Store.Discounts)
            {
                if (discount is OpenDiscount)
                {
                    if (((OpenDiscount) discount).Product == this)
                    {
                        price -= discount.ComputeDiscount(product);
                    }
                }
            }

            return price;
        }

        public override string ToString()
        {
            string output = "Name: " + Name +
            "\nDescription: " + Description + 
            "\nStore: " + Store.Name +
            "\nCategory: " + Category +
            "\nPrice: " + Price;
            return output;
        }
    }
}
