
using ShopSphere.Application.DTOs.Categories;

namespace ShopSphere.Application.Validators.Category
{
    public static class CategoryRequestValidator
    {
        public static List<string> Validate(CreateCategoryRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Category name is required.");

            if (request.Name?.Length > 100)
                errors.Add("Category name must not exceed 100 characters.");

            return errors;
        }


        public static List<string> Validate(UpdateCategoryRequest request)
        {
            var errors = new List<string>();

            if (request.Id == Guid.Empty)
                errors.Add("Category ID is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Category name is required.");

            if (request.Name?.Length > 100)
                errors.Add("Category name must not exceed 100 characters.");

            return errors;
        }
    }

}
