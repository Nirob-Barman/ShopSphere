
using ShopSphere.Application.DTOs.Products;

namespace ShopSphere.Application.Validators.Product
{
    public static class ProductRequestValidator
    {
        public static List<string> Validate(CreateProductRequest request)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(request.Name))
                errors.Add("Product name is required.");

            if (request.Name?.Length > 200)
                errors.Add("Product name must not exceed 200 characters.");

            if (request.Price <= 0)
                errors.Add("Price must be greater than zero.");

            if (string.IsNullOrWhiteSpace(request.SKU))
                errors.Add("SKU is required.");

            if (request.StockQuantity < 0)
                errors.Add("Stock quantity cannot be negative.");

            if (request.SellerId == string.Empty)
                errors.Add("Seller ID is required.");

            return errors;
        }

        public static List<string> Validate(UpdateProductRequest request)
        {
            var errors = Validate(new CreateProductRequest
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                SKU = request.SKU,
                StockQuantity = request.StockQuantity,
                SellerId = request.SellerId
            });

            if (request.Id == Guid.Empty)
                errors.Add("Product ID is required.");

            return errors;
        }
    }
}
