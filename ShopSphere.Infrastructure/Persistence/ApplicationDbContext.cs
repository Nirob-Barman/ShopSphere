
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Identity.Entity;

namespace ShopSphere.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            //builder.Entity<Category>()
            //    .HasOne(c => c.ParentCategory)  // Parent category relationship
            //    .WithMany()  // A category can have many child categories
            //    .HasForeignKey(c => c.ParentCategoryId)  // Foreign key in Category
            //    .OnDelete(DeleteBehavior.SetNull);  // Optional: Set null instead of delete

            //// Product and Category Many-to-Many relationship
            //builder.Entity<Product>()
            //    .HasMany(p => p.Categories)  // A product can belong to many categories
            //    .WithMany(c => c.Products)  // A category can have many products
            //    .UsingEntity(j => j.ToTable("ProductCategories"));  // Join table for many-to-many

        }
    }
}
