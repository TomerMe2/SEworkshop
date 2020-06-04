using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEWorkshop.Models;
using SEWorkshop.Models.Discounts;
using SEWorkshop.Models.Policies;

namespace SEWorkshop.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=AzamazonDB") 
        {
            Addresses = Set<Address>();
            Admins = Set<Administrator>();;
            Authorities = Set<Authority>();
            AuthorityHandlers = Set<AuthorityHandler>();
            Baskets = Set<Basket>();
            Carts = Set<Cart>();
            Discounts = Set<Discount>();
            Messages = Set<Message>();
            OwnershipRequests = Set<OwnershipRequest>();
            OwnershipAnswers = Set<OwnershipAnswer>();
            Policies = Set<Policy>();
            Products = Set<Product>();
            ProductsInBaskets = Set<ProductsInBasket>();
            Purchases = Set<Purchase>();
            Reviews = Set<Review>();
            Stores = Set<Store>();
            Users = Set<LoggedInUser>();
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Administrator> Admins { get; set; }
        public DbSet<Authority> Authorities { get; set; }
        public DbSet<AuthorityHandler> AuthorityHandlers { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OwnershipRequest> OwnershipRequests { get; set; }
        public DbSet<OwnershipAnswer> OwnershipAnswers { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductsInBasket> ProductsInBaskets { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<LoggedInUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
