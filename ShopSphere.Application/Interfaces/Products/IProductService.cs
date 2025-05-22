
using ShopSphere.Application.DTOs.Products;
using ShopSphere.Application.Wrappers;

namespace ShopSphere.Application.Interfaces.Products
{
    public interface IProductService
    {
        Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync();
        Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id);
        Task<ApiResponse<string>> AddAsync(CreateProductRequest request);
        Task<ApiResponse<string>> UpdateAsync(UpdateProductRequest request);
        Task<ApiResponse<string>> DeleteAsync(Guid id);
    }
}
