using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SEWorkshop.Models;
using SEWorkshop.Models.Discounts;
using SEWorkshop.Models.Policies;
using SEWorkshop.Enums;
using System.Collections.Immutable;

namespace SEWorkshop.Tests.TestsDB
{
    public abstract class AppDbContext : DbContext
    {
        public AppDbContext(string conString) : base(conString)
        {
        }

        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Administrator> Admins { get; set; } = null!;
        public DbSet<Authority> Authorities { get; set; } = null!;
        public DbSet<AuthorityHandler> AuthorityHandlers { get; set; } = null!;
        public DbSet<Basket> Baskets { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Discount> Discounts { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<OwnershipRequest> OwnershipRequests { get; set; } = null!;
        public DbSet<OwnershipAnswer> OwnershipAnswers { get; set; } = null!;
        public DbSet<Policy> Policies { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductsInBasket> ProductsInBaskets { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Store> Stores { get; set; } = null!;
        public DbSet<LoggedInUser> LoggedInUsers { get; set; } = null!;

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
