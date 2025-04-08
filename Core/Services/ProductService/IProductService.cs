using Core.Models;
using Product = Database.Models.DBModel.Product;

namespace Core.Services.ProductService;

public interface IProductService
{
    public List<Product> GetAllProducts();

    public Task<List<Product>> GetAllProductsAsync();
    public Product? GetProduct(int productId);
    public Task<Product?> GetProductAsync(int productId);
    public Product AddProduct(ProductReq product);
    public Task<Product> AddProductAsync(ProductReq product);
    public bool DeleteProduct(int productId);

    public Task<bool> DeleteProductAsync(int productId);
    public Product UpdateProduct(UpdateProductReq product);
    public Task<Product> UpdateProductAsync(UpdateProductReq product);
}