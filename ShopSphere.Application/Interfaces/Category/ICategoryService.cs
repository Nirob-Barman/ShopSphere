
using ShopSphere.Application.DTOs.Categories;
using ShopSphere.Application.Wrappers;

namespace ShopSphere.Application.Interfaces.Category
{
    public interface ICategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryResponse>>> GetAllAsync();
        Task<ApiResponse<CategoryResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<string>> AddAsync(CreateCategoryRequest request);
        Task<ApiResponse<string>> UpdateAsync(UpdateCategoryRequest request);
        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}
