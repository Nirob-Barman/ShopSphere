
namespace ShopSphere.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public Guid? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }

        public ICollection<Product>? Products { get; set; }

        // Children navigation property to represent the child categories
        public ICollection<Category>? Children { get; set; }
    }
}
