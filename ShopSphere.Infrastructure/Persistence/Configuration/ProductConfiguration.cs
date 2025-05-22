using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

            builder.Property(p => p.Description).HasMaxLength(500);

            builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.Property(p => p.SKU).HasMaxLength(50).IsRequired();

            builder.Property(p => p.StockQuantity).IsRequired();

            builder.Property(p => p.SellerId).HasMaxLength(50);

            // Configure the many-to-many relationship between Product and Category (if applicable)
            builder.HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .UsingEntity(j => j.ToTable("ProductCategories"));  // Define a join table for the many-to-many relationship

        }
    }
}
