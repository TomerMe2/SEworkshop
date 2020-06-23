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
using System.ComponentModel.DataAnnotations.Schema;
using NLog;

namespace SEWorkshop.DAL
{
    public abstract class AppDbContext : DbContext
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        public AppDbContext(string conString) : base(conString)
        {
            
        }

        public DbSet<Address> Addresses { get; set; } = null!;
        public DbSet<Authority> Authorities { get; set; } = null!;
        public DbSet<Owns> Owns { get; set; } = null!;
        public DbSet<Manages> Manages { get; set; } = null!;
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
        public DbSet<Administrator> Administrators { get; set; } = null!;
        public DbSet<UseRecord> UseRecords { get; set; } = null!;

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch(Exception e)
            {
                //we would like to catch any exception here, since EF is abstracted from all the physical links exceptions
                Log.Error(string.Format("SaveChanges() failed. Exception: {0}", e.ToString()));
                //return error code, but do not throw exception because the changes are cached
                return -1;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            InstantiateKeys(modelBuilder);

            DefineRelations(modelBuilder);
        }

        

        private void InstantiateKeys(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UseRecord>()
                    .Property(record => record.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UseRecord>()
                    .ToTable("UseRecords")
                    .HasKey(record => record.Id);

            modelBuilder.Entity<Address>()
                    .Property(address => address.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Address>()
                    .ToTable("Addresses")
                    .HasKey(address => new { address.Id, address.City, address.Street, address.HouseNumber, address.Country, address.Zip });

            modelBuilder.Entity<Administrator>()
                    .ToTable("Administrators")
                    .HasKey(admin => admin.Username);

            modelBuilder.Entity<Authority>()
                    .ToTable("Authorities")
                    .HasKey(auth => new { auth.AuthHandlerId, auth.Authorization });

            modelBuilder.Entity<Owns>()
                    .ToTable("Owns")
                    .HasKey(owns => owns.Id);

            modelBuilder.Entity<Manages>()
                    .ToTable("Manages")
                    .HasKey(manages => manages.Id);

            modelBuilder.Entity<AuthorityHandler>()
               .Property(auth => auth.Id)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Basket>()
                    .ToTable("Baskets")
                    .HasKey(basket => basket.Id);

            modelBuilder.Entity<Basket>()
                .Property(bskt => bskt.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Cart>()
                    .ToTable("Carts")
                    .HasKey(cart => cart.Id);

            modelBuilder.Entity<Cart>()
                .Property(cart => cart.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            

            modelBuilder.Entity<Discount>()
                    .ToTable("Discounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<Discount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<BuyOverDiscount>()
                    .ToTable("BuyOverDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<BuyOverDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<BuySomeGetSomeDiscount>()
                    .ToTable("BuySomeGetSomeDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<BuySomeGetSomeDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ComposedDiscount>()
                    .ToTable("ComposedDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<ComposedDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ConditionalDiscount>()
                    .ToTable("ConditionalDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<ConditionalDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<OpenDiscount>()
                    .ToTable("OpenDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<OpenDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<PrimitiveDiscount>()
                    .ToTable("PrimitiveDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<PrimitiveDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ProductCategoryDiscount>()
                    .ToTable("ProductCategoryDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<ProductCategoryDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<SpecificProducDiscount>()
                    .ToTable("SpecificProducDiscounts")
                    .HasKey(discount => discount.DiscountId);

            modelBuilder.Entity<SpecificProducDiscount>()
                .Property(discount => discount.DiscountId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<LoggedInUser>()
                    .ToTable("LoggedInUsers")
                    .HasKey(user => user.Username);

            modelBuilder.Entity<Manages>()
                    .ToTable("Manages")
                    .HasKey(manages => new { manages.Username, manages.StoreName });

            modelBuilder.Entity<Message>()
                    .ToTable("Messages")
                    .HasKey(message => message.Id);

            modelBuilder.Entity<Message>()
                .Property(message => message.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<OwnershipAnswer>()
                    .ToTable("OwnershipAnswers")
                    .HasKey(ans => ans.Id);

            modelBuilder.Entity<OwnershipAnswer>()
                .Property(answer => answer.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<OwnershipRequest>()
                    .ToTable("OwnershipRequests")
                    .HasKey(req => req.Id);

            modelBuilder.Entity<OwnershipRequest>()
                .Property(request => request.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Policy>()
                    .ToTable("Policies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<Policy>()
                .Property(pol => pol.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<AlwaysTruePolicy>()
                    .ToTable("AlwaysTruePolicies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<AlwaysTruePolicy>()
                .Property(pol => pol.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<SingleProductQuantityPolicy>()
                    .ToTable("SingleProductQuantityPolicies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<SingleProductQuantityPolicy>()
                .Property(pol => pol.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<SystemDayPolicy>()
                    .ToTable("SystemDayPolicies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<SystemDayPolicy>()
                .Property(pol => pol.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UserCityPolicy>()
                    .ToTable("UserCityPolicies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<UserCityPolicy>()
                .Property(request => request.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<UserCountryPolicy>()
                    .ToTable("UserCountryPolicies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<UserCountryPolicy>()
                .Property(pol => pol.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<WholeStoreQuantityPolicy>()
                    .ToTable("WholeStoreQuantityPolicies")
                    .HasKey(policy => policy.Id);

            modelBuilder.Entity<WholeStoreQuantityPolicy>()
                .Property(pol => pol.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Product>()
                    .ToTable("Products")
                    .HasKey(product => product.Id);

            modelBuilder.Entity<Product>()
                .Property(prod => prod.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<ProductsInBasket>()
                    .ToTable("ProductsInBaskets")
                    .HasKey(pb => new { pb.BasketId, pb.ProductId });

            modelBuilder.Entity<Purchase>()
                .Property(purchase => purchase.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Purchase>()
                    .ToTable("Purchases")
                    .HasKey(purchase => purchase.Id);

            modelBuilder.Entity<Review>()
                    .ToTable("Reviews")
                    .HasKey(review => review.Id);

            modelBuilder.Entity<Review>()
                .Property(review => review.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Store>()
                    .ToTable("Stores")
                    .HasKey(store => store.Name);
        }

        
         
        private void DefineRelations(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Authority>()
                    .HasRequired(auth => auth.AuthHandler)
                    .WithMany(handler => handler.AuthoriztionsOfUser)
                    .HasForeignKey(auth => auth.AuthHandlerId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<AuthorityHandler>()
                    .HasOptional(handler => handler.Appointer)
                    .WithMany(appointer => appointer.Appointements)
                    .HasForeignKey(handler => handler.AppointerName)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Owns>()
                    .HasRequired(handler => handler.Store)
                    .WithMany(store => store.Ownership)
                    .HasForeignKey(handler => handler.StoreName)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manages>()
                   .HasOptional(handler => handler.Store)
                   .WithMany(store => store.Management)
                   .HasForeignKey(handler => handler.StoreName)
                   .WillCascadeOnDelete(false);

            modelBuilder.Entity<Owns>()
                    .HasRequired(handler => handler.LoggedInUser)
                    .WithMany(user => user.Owns)
                    .HasForeignKey(handler => handler.Username)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manages>()
                    .HasOptional(handler => handler.LoggedInUser)
                    .WithMany(user => user.Manage)
                    .HasForeignKey(handler => handler.Username)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Basket>()
                    .HasOptional(basket => basket.Cart)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    .WithMany(cart => cart.Baskets)
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    .HasForeignKey(basket => new { basket.CartId })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Purchase>()
                .HasRequired(prchs => prchs.Admin)
                .WithMany(admin => admin.PurchasesToView)
                .HasForeignKey(prchs => prchs.AdminUserName)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Basket>()
                    .HasRequired(basket => basket.Store)
                    .WithMany(store => store.Baskets)
                    .HasForeignKey(basket => new { basket.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cart>()
                .HasOptional(cart => cart.LoggedInUser)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                .WithRequired(user => user.Cart)
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discount>()
                    .HasRequired(discount => discount.Store)
                    .WithMany(store => store.Discounts)
                    .HasForeignKey(discount => new { discount.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<ComposedDiscount>()
                    .HasOptional(discount => discount.RightChild)
                    .WithMany()
                    .HasForeignKey(discount => discount.RightChildId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<ComposedDiscount>()
                    .HasOptional(discount => discount.LeftChild)
                    .WithMany()
                    .HasForeignKey(discount => discount.LeftChildId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discount>()
                    .HasOptional(discount => discount.Father)
                    .WithMany()
                    .HasForeignKey(discount => discount.FatherId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<ConditionalDiscount>()
                    .HasRequired(discount => discount.Product)
                    .WithMany(product => product.ConditionalDiscounts)
                    .HasForeignKey(discount => discount.ProductId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<OpenDiscount>()
                    .HasRequired(discount => discount.Product)
                    .WithMany(product => product.OpenDiscounts)
                    .HasForeignKey(discount => discount.ProductId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<BuySomeGetSomeDiscount>()
                    .HasRequired(discount => discount.ProdUnderDiscount)
                    .WithMany(product => product.BuySomeGetSomeDiscounts)
                    .HasForeignKey(discount => discount.ProductIdUnderDiscount)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manages>()
                    .HasRequired(manager => manager.LoggedInUser)
                    .WithMany(handler => handler.Manage)
                    .HasForeignKey(manager => new { manager.Username })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manages>()
                    .HasRequired(manager => manager.Store)
                    .WithMany(store => store.Management)
                    .HasForeignKey(manager => new { manager.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                    .HasRequired(message => message.ToStore)
                    .WithMany(store => store.Messages)
                    .HasForeignKey(manager => new { manager.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                    .HasRequired(message => message.WrittenBy)
                    .WithMany(user => user.Messages)
                    .HasForeignKey(manager => new { manager.Writer })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Message>()
                    .HasOptional(message => message.Prev)
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
                    .WithOptionalPrincipal(message => message.Next)
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<OwnershipAnswer>()
                    .HasRequired(ans => ans.Request)
                    .WithMany(req => req.Answers)
                    .HasForeignKey(ans => ans.RequestId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<OwnershipAnswer>()
                    .HasRequired(ans => ans.Owner)
                    .WithMany(owner => owner.OwnershipAnswers)
                    .HasForeignKey(ans => ans.Username)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<OwnershipRequest>()
                    .HasRequired(req => req.Store)
                    .WithMany(store => store.OwnershipRequests)
                    .HasForeignKey(req => new { req.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<OwnershipRequest>()
                    .HasRequired(req => req.Owner)
                    .WithMany(store => store.OwnershipRequestsFrom)
                    .HasForeignKey(req => new { req.OwnerUsername })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<OwnershipRequest>()
                    .HasRequired(req => req.NewOwner)
                    .WithMany(store => store.OwnershipRequests)
                    .HasForeignKey(req => new { req.NewOwnerUsername })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Owns>()
                    .HasRequired(owner => owner.LoggedInUser)
                    .WithMany(handler => handler.Owns)
                    .HasForeignKey(owner => owner.Username)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Owns>()
                    .HasRequired(owner => owner.Store)
                    .WithMany(store => store.Ownership)
                    .HasForeignKey(owner => owner.StoreName)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Policy>()
                    .HasOptional(policy => policy.InnerPolicy)
                    .WithMany()
                    .HasForeignKey(policy => policy.InnerPolicyId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Policy>()
                    .HasOptional(policy => policy.OuterPolicy)
                    .WithMany()
                    .HasForeignKey(policy => policy.OuterPolicyId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Policy>()
                    .HasRequired(policy => policy.Store)
                    .WithMany(store => store.Policies)
                    .HasForeignKey(policy => policy.StoreName)
                    .WillCascadeOnDelete(false);
                
            modelBuilder.Entity<SingleProductQuantityPolicy>()
                    .HasRequired(policy => policy.Prod)
                    .WithMany(prod => prod.ProductPolicies)
                    .HasForeignKey(policy => policy.ProductId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                    .HasRequired(product => product.Store)
                    .WithMany(store => store.Products)
                    .HasForeignKey(product => new { product.StoreName })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductsInBasket>()
                    .HasRequired(pb => pb.Basket)
                    .WithMany(Basket => Basket.Products)
                    .HasForeignKey(pb => pb.BasketId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductsInBasket>()
                    .HasRequired(pb => pb.Product)
                    .WithMany(product => product.InBaskets)
                    .HasForeignKey(pb => pb.ProductId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Purchase>()
                    .HasRequired(purchase => purchase.Address)
                    .WithMany(address => address.Purchases)
                    .HasForeignKey(prchs => new { prchs.AddressId, prchs.City, prchs.Street, prchs.HouseNumber, prchs.Country, prchs.Zip})
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Basket>()
                    .HasOptional(basket => basket.Purchase)
#pragma warning disable CS8603 // Possible null reference return.
                    .WithRequired(purchase => purchase.Basket)
#pragma warning restore CS8603 // Possible null reference return.
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Purchase>()
                    .HasRequired(purchase => purchase.store)
                    .WithMany(store => store.Purchases)
                    .HasForeignKey(prchs => prchs.storeName)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Purchase>()
                    .HasOptional(purchase => purchase.User)
                    .WithMany(user => user.Purchases)
                    .HasForeignKey(prchs => prchs.Username)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                    .HasRequired(review => review.Writer)
                    .WithMany(user => user.Reviews)
                    .HasForeignKey(review => new { review.Username })
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Review>()
                    .HasRequired(review => review.Product)
                    .WithMany(product => product.Reviews)
                    .HasForeignKey(review => review.ProductId)
                    .WillCascadeOnDelete(false);
        }
    }
}
