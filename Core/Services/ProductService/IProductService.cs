using Core.Models;
using Product = Database.Models.DBModel.Product;

namespace Core.Services.ProductService;

public interface IProductService
{
    public List<Product> GetAllProducts();

    public Task<List<Product>> GetAllProductsAsync();
    public Task<Product> GetProduct(int productId);
    public Product AddProduct(ProductReq product);
    public Task<Product> AddProductAsync(ProductReq product);

    public Task<bool> DeleteProduct(int productId);
    public Task<Product> UpdateProduct(UpdateProductReq product);
}