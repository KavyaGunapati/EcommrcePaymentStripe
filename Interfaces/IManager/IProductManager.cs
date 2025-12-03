using Models.DTOs;

namespace Interfaces.IManager
{
    public interface IProductManager
    {
        Task<Result> AddProductAsync(ProductDto productDto);
        Task<Result> UpdateProductAsync(ProductDto productDto);
        Task<Result> DeleteProductAsync(int productId);
        Task<Result<List<ProductDto>>> GetAllProductAsync();
        Task<Result<ProductDto>> GetProductById(int productId);
        Task<Result<List<ProductDto>>> FilterProducts(int? minprice, int? maxprice,string? sortstring);
    }
}
