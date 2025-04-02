using Core.Models;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Product = Database.Models.DBModel.Product;

namespace Core.Services.ProductService;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext context,
        ILogger<ProductService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public List<Product> GetAllProducts()
    {
        return _context.Products.ToList();
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetProduct(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            _logger.LogError("Product not found: {ProductId}", productId);
            throw new ArgumentNullException($"Product not found: {productId}");
        }

        return product;
    }

    public Product AddProduct(ProductReq productReq)
    {
        var product = new Product
        {
            Name = productReq.name,
            Price = productReq.price,
            StockQuantity = productReq.quantity,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Products.Add(product);
        _context.SaveChanges();

        return product;
    }

    public async Task<Product> AddProductAsync(ProductReq productReq)
    {
        var product = new Product
        {
            Name = productReq.name,
            Price = productReq.price,
            StockQuantity = productReq.quantity,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }


    public async Task<bool> DeleteProduct(int productId)
    {
        var product = await GetProduct(productId);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Product> UpdateProduct(UpdateProductReq productReq)
    {
        var existProduct = await GetProduct(productReq.id);

        existProduct.Name = productReq.name;
        existProduct.Price = productReq.price;
        existProduct.StockQuantity = productReq.quantity;
        existProduct.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(existProduct);
        await _context.SaveChangesAsync();

        return existProduct;
    }
}