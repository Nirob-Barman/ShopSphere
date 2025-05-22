namespace ShopSphere.Application.DTOs.Categories
{
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ParentCategoryId { get; set; }
    }
}
