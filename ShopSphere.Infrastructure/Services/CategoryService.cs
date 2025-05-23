using ShopSphere.Application.DTOs.Categories;
using ShopSphere.Application.Interfaces.Category;
using ShopSphere.Application.Interfaces.Persistence;
using ShopSphere.Application.Validators.Category;
using ShopSphere.Application.Wrappers;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAllAsync();
            var result = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.Name
            });

            return ApiResponse<IEnumerable<CategoryResponse>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<CategoryResponse>> GetByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
                return ApiResponse<CategoryResponse>.NotFoundResponse("Category not found");

            var result = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory?.Name
            };

            return ApiResponse<CategoryResponse>.SuccessResponse(result);
        }

        public async Task<ApiResponse<string>> AddAsync(CreateCategoryRequest request)
        {
            var validationErrors = CategoryRequestValidator.Validate(request);
            if (validationErrors.Any())
                return ApiResponse<string>.ValidationErrorResponse("Validation failed", validationErrors);

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId
            };

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.CreatedResponse("Category created successfully");
        }

        public async Task<ApiResponse<string>> UpdateAsync(UpdateCategoryRequest request)
        {
            var validationErrors = CategoryRequestValidator.Validate(request);
            if (validationErrors.Any())
                return ApiResponse<string>.ValidationErrorResponse("Validation failed", validationErrors);

            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(request.Id);
            if (category == null)
                return ApiResponse<string>.NotFoundResponse("Category not found");

            category.Name = request.Name;
            category.ParentCategoryId = request.ParentCategoryId;

            _unitOfWork.Repository<Category>().Update(category);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Category updated successfully");
        }

        public async Task<ApiResponse<string>> DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
                return ApiResponse<string>.NotFoundResponse("Category not found");

            _unitOfWork.Repository<Category>().Remove(category);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Category deleted successfully");
        }
    }
}
