using AutoMapper;
using DataAccess.Entities;
using Interfaces.IManager;
using Interfaces.IRepository;
using Models.DTOs;
using Serilog;

namespace Managers
{
    public class ProductManager : IProductManager
    {
        private readonly IGenericRepository<Product> _repository;
        private readonly IMapper _mapper;
        public ProductManager(IGenericRepository<Product> repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<Result> AddProductAsync(ProductDto productDto)
        {
            Log.Information("Add Product {Name}", productDto.Name);
            try
            {
                var product = _mapper.Map<Product>(productDto);
                await _repository.AddAsync(product);
                Log.Information("Added Product {Name}", productDto.Name);
                return new Result { Success = true, Message = "Product Added successfully" };

            }
            catch (Exception ex)
            {
                Log.Error("Error while adding Product");
                return new Result { Success = false, Message = ex.Message };
            }
        }

        public async Task<Result> DeleteProductAsync(int productId)
        {
            try
            {
                var productExist = await _repository.GetByIdAsync(productId);
                if (productExist == null) return new Result { Success = false, Message = "Product not found" };
                await _repository.Delete(productExist);
                return new Result { Success = true, Message = "Product deleted succesfully" };
            }
            catch (Exception ex)
            {
                return new Result { Success = false, Message = ex.Message };
            }
        }

        public async Task<Result<List<ProductDto>>> FilterProducts(int? minprice, int? maxprice, string? sortstring)
        {
            try
            {
                var products = await _repository.GetAllAsync();
                if (minprice.HasValue)
                {
                    products = products.Where(p => p.Price > minprice.Value);
                }
                if (maxprice.HasValue)
                {
                    products = products.Where(p => p.Price < maxprice.Value);
                }
                if (!string.IsNullOrEmpty(sortstring))
                {
                    if (sortstring == "desc")
                    {
                        products.OrderByDescending(e => e.Price).ToList();
                    }
                    if (sortstring.ToLower().Contains("asc"))
                    {
                        products.OrderBy(e => e.Price).ToList();
                    }
                }
                return new Result<List<ProductDto>> { Success = true, Message = "Filtered products", Data = _mapper.Map<List<ProductDto>>(products) };
            }
            catch (Exception ex)
            {
                return new Result<List<ProductDto>> { Success = false, Message = ex.Message };
            }
        }

        public async Task<Result<List<ProductDto>>> GetAllProductAsync()
        {
            var produtcs = await _repository.GetAllAsync();
            if (produtcs == null) return new Result<List<ProductDto>> { Success = false, Message = "No products found" };
            return new Result<List<ProductDto>> { Success = true, Message = "Products Found", Data = _mapper.Map<List<ProductDto>>(produtcs) };
        }

        public async Task<Result<ProductDto>> GetProductById(int productId)
        {
            var product = await _repository.GetByIdAsync(productId);
            if (product == null) return new Result<ProductDto> { Success = false, Message = "Product not found" };
            return new Result<ProductDto> { Success = true, Message = "Product Found", Data = _mapper.Map<ProductDto>(product) };
        }

        public async Task<Result> UpdateProductAsync(ProductDto productDto)
        {
            var product = await _repository.GetByIdAsync(productDto.Id);
            if (product == null) return new Result { Success = false, Message = "Product Not Found" };
            product.Stock = productDto.Stock > 0 ? productDto.Stock : product.Stock;
            product.Price = productDto.Price > 0 ? productDto.Price : product.Price;
            product.Description = !string.IsNullOrEmpty(productDto.Description) ? productDto.Description : product.Description;
            product.Name = !string.IsNullOrEmpty(productDto.Name) ? productDto.Name : product.Name;
            await _repository.Update(product);
            return new Result { Success = true, Message = "Product Updated Successfully" };
        }

    }
}
