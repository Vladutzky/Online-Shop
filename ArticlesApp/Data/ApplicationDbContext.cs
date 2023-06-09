using productsApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;


// PASUL 3 - useri si roluri

namespace productsApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<product> products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<review> reviews { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<productOrder> productorders { get; set; }
        public DbSet<Pending> Pending { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // definire primary key compus
            modelBuilder.Entity<productOrder>()
                .HasKey(ab => new { ab.Id, ab.ProductID, ab.orderId });


            // definire relatii cu modelele order si product (FK)
            modelBuilder.Entity<productOrder>()
                .HasOne(ab => ab.product)
                .WithMany (ab => ab.productorders)
                .HasForeignKey(ab => ab.ProductID);

            modelBuilder.Entity<productOrder>()
                .HasOne(ab => ab.order)
                .WithMany(ab => ab.productorders)
                .HasForeignKey(ab => ab.orderId);
        }
    }
}