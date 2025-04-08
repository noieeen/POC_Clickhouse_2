using Core.Models;
using Database.Models.DBModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

    public Product? GetProduct(int productId)
    {
        string cacheKey = $"product:{productId}";
        var cachedProduct = _cache.GetCacheValue<Product>(cacheKey);

        if (cachedProduct != null)
        {
            return cachedProduct;
        }

        var product = _context.Products.Find(productId);
        if (product == null)
        {
            _logger.LogError("Product not found: {ProductId}", productId);
            return null;
        }

        _cache.SetCacheValue(cacheKey, product, CacheExpiration);


        return product;
    }

    public async Task<Product?> GetProductAsync(int productId)
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
            return null;
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

    public bool DeleteProduct(int productId)
    {
        var product = GetProduct(productId);
        _context.Products.Remove(product);
        _context.SaveChangesAsync();

        _cache.Remove($"product:{productId}");
        _cache.Remove("all_products");

        return true;
    }

    public async Task<bool> DeleteProductAsync(int productId)
    {
        var product = await GetProductAsync(productId);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        await _cache.RemoveAsync($"product:{productId}");
        await _cache.RemoveAsync("all_products");

        return true;
    }

    public Product UpdateProduct(UpdateProductReq productReq)
    {
        var existProduct = GetProduct(productReq.id);

        existProduct.Name = productReq.name;
        existProduct.Price = productReq.price;
        existProduct.StockQuantity = productReq.quantity;
        existProduct.UpdatedAt = DateTime.UtcNow;

        _context.Products.Update(existProduct);
        _context.SaveChangesAsync();

        _cache.Remove($"product:{productReq.id}");
        _cache.Remove("all_products");

        return existProduct;
    }

    public async Task<Product> UpdateProductAsync(UpdateProductReq productReq)
    {
        var existProduct = await GetProductAsync(productReq.id);

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