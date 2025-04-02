using Core;
using Core.Api.Controller;
using Core.Models;
using Core.Services.ProductService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Store.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductController : BaseApiController
{
    private readonly IProductService _productService;

    public ProductController(
        ILogger<ProductController> logger,
        ICommon_Exception_Factory commonExFactory,
        IHttpContextAccessor httpContextAccessor,
        IProductService productService
    )
        : base(logger, commonExFactory, httpContextAccessor)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProduct(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct([FromBody] ProductReq productReq)
    {
        var product = await _productService.AddProductAsync(productReq);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductReq productReq)
    {
        var updatedProduct = await _productService.UpdateProduct(productReq);
        return Ok(updatedProduct);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProduct(id);
        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}