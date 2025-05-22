using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Persistence.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);            
            builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
            builder.Property(c => c.ParentCategoryId).IsRequired(false);  // ParentCategoryId is optional (nullable)

            // Configure the self-referencing relationship (ParentCategory -> Category)
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.Children)  // A category can have many child categories
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.NoAction);  // Set to NoAction to avoid cascading delete issues

            // Optionally, add an index for faster lookups
            builder.HasIndex(c => c.Name).IsUnique();


            // Seeding Categories with hardcoded GUIDs for parent-child relationships
            builder.HasData(
                new Category
                {
                    Id = Guid.Parse("b3f9fd1c-7b93-4fd3-91b8-9354a8b49f67"), // Electronics
                    Name = "Electronics",
                    ParentCategoryId = null // No parent category
                },
                new Category
                {
                    Id = Guid.Parse("a2c679cb-7b6b-4a5a-b0a9-1f8a92e77485"), // Smartphones (Child of Electronics)
                    Name = "Smartphones",
                    ParentCategoryId = Guid.Parse("b3f9fd1c-7b93-4fd3-91b8-9354a8b49f67") // Reference to Electronics
                },
                new Category
                {
                    Id = Guid.Parse("d029d029-72b1-4f75-8035-919a4975c74e"), // Laptops (Child of Electronics)
                    Name = "Laptops",
                    ParentCategoryId = Guid.Parse("b3f9fd1c-7b93-4fd3-91b8-9354a8b49f67") // Reference to Electronics
                },
                new Category
                {
                    Id = Guid.Parse("39f9edb3-7d53-46b8-a2b5-8eb9d17f97f3"), // Fashion
                    Name = "Fashion",
                    ParentCategoryId = null // No parent category
                },
                new Category
                {
                    Id = Guid.Parse("7b4c5a91-dc88-4a64-b51c-92ad77c994f8"), // Clothing (Child of Fashion)
                    Name = "Clothing",
                    ParentCategoryId = Guid.Parse("39f9edb3-7d53-46b8-a2b5-8eb9d17f97f3") // Reference to Fashion
                },
                new Category
                {
                    Id = Guid.Parse("2d4b6f9b-0d83-4a8e-9e16-e78f1ab62d74"), // Accessories (Child of Fashion)
                    Name = "Accessories",
                    ParentCategoryId = Guid.Parse("39f9edb3-7d53-46b8-a2b5-8eb9d17f97f3") // Reference to Fashion
                }
            );
        }
    }
}
