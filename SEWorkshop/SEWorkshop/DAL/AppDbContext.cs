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

namespace SEWorkshop.DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=\"C:\\Users\\Ravid\\Desktop\\1st Degree\\3rd Year\\Semester F\\Workshop for SE Project\\Code\\SEworkshop\\SEWorkshop\\SEWorkshop\\DAL\\AzamazonLocal.mdf\";Integrated Security = True") 
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
            LoggedInUsers = Set<LoggedInUser>();
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
        public DbSet<LoggedInUser> LoggedInUsers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>()
                    .ToTable("Addresses")
                    .HasKey(address => new {address.City, address.Street, address.HouseNumber, address.Country});
            
            modelBuilder.Entity<Administrator>()
                    .ToTable("Administrators");

            modelBuilder.Entity<Authority>()
                    .ToTable("Authorities")
                    .HasKey(auth => new { auth.AuthHandlerName, auth.StoreName, auth.Authorization });

            modelBuilder.Entity<AuthorityHandler>()
                    .ToTable("AuthorityHandlers")
                    .HasKey(handler => new {handler.Id});
            
            modelBuilder.Entity<Basket>()
                    .ToTable("Baskets")
                    .HasKey(basket => basket.Id);
            
            modelBuilder.Entity<Cart>()
                    .ToTable("Carts")
                    .HasKey(cart => cart.Id);

            modelBuilder.Entity<LoggedInUser>()
                    .ToTable("LoggedInUsers")
                    .HasKey(user => user.Username);
            
            modelBuilder.Entity<Manages>()
                    .ToTable("Managers");

            modelBuilder.Entity<Message>()
                    .ToTable("Messages")
                    .HasKey(message => message.Id);
            
            modelBuilder.Entity<Owns>()
                    .ToTable("Owners");

            modelBuilder.Entity<Product>()
                    .ToTable("Products")
                    .HasKey(product => new {product.Name, product.StoreName});
            
            modelBuilder.Entity<ProductsInBasket>()
                    .ToTable("ProductsInBaskets")
                    .HasKey(pb => new {pb.BasketId, pb.ProductName});
            
            modelBuilder.Entity<Purchase>()
                    .ToTable("Purchases")
                    .HasKey(purchase => purchase.BasketId);

            modelBuilder.Entity<Store>()
                    .ToTable("Stores")
                    .HasKey(store => store.Name);



            modelBuilder.Entity<Authority>()
                    .HasRequired(auth => auth.AuthHandler)
                    .WithMany(handler => handler.AuthoriztionsOfUser)
                    .HasForeignKey(auth => new {auth.AuthHandlerId});

            modelBuilder.Entity<Authority>()
                    .Property(auth => auth.Authorization)
                    .HasColumnType("tinyint");

            modelBuilder.Entity<AuthorityHandler>()
                    .HasRequired(handler => handler.Appointer)
                    .WithMany(appointer => appointer.Appointements)
                    .HasForeignKey(handler => new {handler.AppointerName });

            modelBuilder.Entity<Basket>()
                    .HasRequired(basket => basket.Cart)
                    .WithMany(cart => cart.Baskets)
                    .HasForeignKey(basket => new {basket.CartId});
            
            modelBuilder.Entity<Basket>()
                    .HasRequired(basket => basket.Store)
                    .WithMany(store => store.Baskets)
                    .HasForeignKey(basket => new {basket.StoreName});
            
            modelBuilder.Entity<Cart>()
                    .HasOptional(cart => cart.LoggedInUser)
                    .WithOptionalPrincipal(user => user.Cart);

            modelBuilder.Entity<Manages>()
                    .HasRequired(manager => manager.LoggedInUser)
                    .WithMany(handler => handler.Manage)
                    .HasForeignKey(manager => new {manager.Username});
            
            modelBuilder.Entity<Manages>()
                    .HasRequired(manager => manager.Store)
                    .WithMany(store => store.Management)
                    .HasForeignKey(manager => new {manager.StoreName});

            modelBuilder.Entity<Message>()
                    .HasOptional(message => message.Prev)
                    .WithOptionalPrincipal(message => message.Next);

            modelBuilder.Entity<Owns>()
                    .HasRequired(owner => owner.LoggedInUser)
                    .WithMany(handler => handler.Owns)
                    .HasForeignKey(owner => new {owner.Username});
        
            modelBuilder.Entity<Owns>()
                    .HasRequired(owner => owner.Store)
                    .WithMany(store => store.Ownership)
                    .HasForeignKey(owner => new {owner.StoreName});

            modelBuilder.Entity<Product>()
                    .HasRequired(product => product.Store)
                    .WithMany(store => store.Products)
                    .HasForeignKey(product => new { product.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductsInBasket>()
                    .HasRequired(pb => pb.Basket)
                    .WithMany(Basket => Basket.Products)
                    .HasForeignKey(pb => new {pb.BasketId});

            modelBuilder.Entity<ProductsInBasket>()
                    .HasRequired(pb => pb.Product)
                    .WithMany(product => product.InBaskets)
                    .HasForeignKey(pb => new {pb.ProductName, pb.StoreName});

            modelBuilder.Entity<Purchase>()
                .HasRequired(purchase => purchase.Basket)
                .WithOptional(basket => basket.Purchase);
        }
    }
}
