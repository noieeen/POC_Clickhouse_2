using Core.Models;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Core.Services.CacheService;
using Product = Database.Models.DBModel.Product;

namespace Core.Services.ProductService;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductService> _logger;
    private readonly IRedisCacheService _cache;

    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public ProductService(AppDbContext context,
        ILogger<ProductService> logger,
        IRedisCacheService cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public List<Product> GetAllProducts()
    {
        var cacheKey = "all_products";
        var cachedProducts = _cache.GetCacheValue<List<Product>>(cacheKey);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = _context.Products.ToList();
        if (products.Any())
        {
            _cache.SetCacheValue(cacheKey, products, CacheExpiration);


            return products;
        }
        else
        {
            _logger.LogInformation("No products found");
        }

        return products;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        var cacheKey = "all_products";
        var cachedProducts = await _cache.GetCacheValueAsync<List<Product>>(cacheKey);
        if (cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _context.Products.ToListAsync();
        if (products.Any())
        {
            await _cache.SetCacheValueAsync(cacheKey, products, CacheExpiration);

            return products;
        }
        else
        {
            _logger.LogInformation("No products found");
        }

        return products;
    }

    public async Task<Product> GetProduct(int productId)
    {
        string cacheKey = $"product:{productId}";
        var cachedProduct = await _cache.GetCacheValueAsync<Product>(cacheKey);

        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            _logger.LogError("Product not found: {ProductId}", productId);
            throw new ArgumentNullException($"Product not found: {productId}");
        }

        await _cache.SetCacheValueAsync(cacheKey, product, CacheExpiration);


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

        await _cache.RemoveAsync("all_products");

        return product;
    }

    public async Task<bool> DeleteProduct(int productId)
    {
        var product = await GetProduct(productId);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"product:{productId}");
        await _cache.RemoveAsync("all_products");

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

        await _cache.RemoveAsync($"product:{productReq.id}");
        await _cache.RemoveAsync("all_products");

        return existProduct;
    }
}