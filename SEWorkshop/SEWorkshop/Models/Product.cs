using System;
using System.Collections.Generic;
using System.Text;
using SEWorkshop.Models.Discounts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SEWorkshop.DAL;
using SEWorkshop.Models.Policies;

namespace SEWorkshop.Models
{
    public class Product
    {
        public virtual int Id { get; set; }
        public virtual string StoreName { get; set; }
        public virtual Store Store { get; private set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Category { get; set; }
        public virtual double Price { get; set; }
        public virtual int Quantity { get; set; }
        public virtual ICollection<BuySomeGetSomeDiscount> BuySomeGetSomeDiscounts { get; set; }
        public virtual ICollection<OpenDiscount> OpenDiscounts { get; set; }
        public virtual ICollection<ConditionalDiscount> ConditionalDiscounts { get; set; }
        public virtual ICollection<SingleProductQuantityPolicy> ProductPolicies { get; set; }
        public virtual ICollection<ProductsInBasket> InBaskets {get ; set;}
        public virtual ICollection<Review> Reviews {get ; set;}

        private Product()
        {
            Store = null!;
            Name = "";
            StoreName = "";
            Description = "";
            Category = "";
            Reviews = new List<Review>();
            InBaskets = new List<ProductsInBasket>();

            BuySomeGetSomeDiscounts = new List<BuySomeGetSomeDiscount>();
            ConditionalDiscounts = new List<ConditionalDiscount>();
            OpenDiscounts = new List<OpenDiscount>();
            ProductPolicies = new List<SingleProductQuantityPolicy>();

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
            StoreName = store.Name;

            InBaskets = new List<ProductsInBasket>();

            BuySomeGetSomeDiscounts = new List<BuySomeGetSomeDiscount>();
            ConditionalDiscounts = new List<ConditionalDiscount>();
            OpenDiscounts = new List<OpenDiscount>();
            ProductPolicies = new List<SingleProductQuantityPolicy>();

        }

        public double PriceAfterDiscount()
        {
            double price = Price;
            //TODO: THIS.
            //ICollection<ProductsInBasket> product = new List<ProductsInBasket>{(new ProductsInBasket(new Basket(this.Store, new Cart(new GuestUser(new AppDbContext())), new AppDbContext()), this, 1))};
            ICollection<ProductsInBasket> product = new List<ProductsInBasket>();
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
