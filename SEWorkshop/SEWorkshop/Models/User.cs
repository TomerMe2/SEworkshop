using SEWorkshop.Adapters;
using SEWorkshop.Exceptions;
using SEWorkshop.Facades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SEWorkshop.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SEWorkshop.Models
{
    public abstract class User
    {

        public virtual Cart Cart { get; set; }



        public User()
        {
            Cart = new Cart(this);
        }

        public abstract void AddProductToCart(Product product, int quantity);
        public abstract void RemoveProductFromCart(User user, Product product, int quantity);
        public abstract Purchase Purchase(Basket basket, string creditCardNumber, DateTime expirationDate, string cvv, Address address, string username, string id);
    }
}