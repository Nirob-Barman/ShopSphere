using ShopSphere.Application.DTOs.Products;
using ShopSphere.Application.Interfaces.Persistence;
using ShopSphere.Application.Interfaces.Products;
using ShopSphere.Application.Wrappers;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<IEnumerable<ProductResponse>>> GetAllAsync()
        {
            var products = await _unitOfWork.Repository<Product>().GetAllAsync();

            var result = products.Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SKU = p.SKU,
                StockQuantity = p.StockQuantity,
                SellerId = p.SellerId,
                Categories = p.Categories?.Select(c => c.Name!).ToList()
            });

            return ApiResponse<IEnumerable<ProductResponse>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<ProductResponse>> GetByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null)
                return ApiResponse<ProductResponse>.NotFoundResponse("Product not found");

            var result = new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                StockQuantity = product.StockQuantity,
                SellerId = product.SellerId,
                Categories = product.Categories?.Select(c => c.Name!).ToList()
            };

            return ApiResponse<ProductResponse>.SuccessResponse(result);
        }

        public async Task<ApiResponse<string>> AddAsync(CreateProductRequest request)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                SKU = request.SKU,
                StockQuantity = request.StockQuantity,
                SellerId = request.SellerId
                // Link categories here if necessary
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.CreatedResponse("Product created successfully.");
        }

        public async Task<ApiResponse<string>> UpdateAsync(UpdateProductRequest request)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            if (product == null)
                return ApiResponse<string>.NotFoundResponse("Product not found");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.SKU = request.SKU;
            product.StockQuantity = request.StockQuantity;
            product.SellerId = request.SellerId;

            _unitOfWork.Repository<Product>().Update(product);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Product updated successfully.");
        }

        public async Task<ApiResponse<string>> DeleteAsync(Guid id)
        {
            var product = await _unitOfWork.Repository<Product>().GetByIdAsync(id);
            if (product == null)
                return ApiResponse<string>.NotFoundResponse("Product not found");

            _unitOfWork.Repository<Product>().Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<string>.SuccessResponse("Product deleted successfully.");
        }
    }
}
