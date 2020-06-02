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
        public AppDbContext() : base("name=dbCheck") 
        {
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public DbSet<LoggedInUser>? Users { get; set; }
        public DbSet<Administrator>? Admins { get; set; }
        public DbSet<Store>? Stores { get; set; }
        public DbSet<Owns>? Ownership { get; set; }
        public DbSet<Manages>? Management { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Purchase>? Purchases { get; set; }
        public DbSet<Message>? Messages { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        public DbSet<Manages>? Managers {get; set;}
        public DbSet<Owns>? Owners {get; set;}
        public DbSet<Policy>? Policy { get; set; }
        
        //public DbSet<Discount> Discounts { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
