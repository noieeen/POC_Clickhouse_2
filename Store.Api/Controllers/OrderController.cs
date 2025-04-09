using Core;
using Core.Api.Controller;
using Core.Models;
using Core.Services.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace Store.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class OrderController : BaseApiController
{
    private readonly IOrderService _orderService;

    public OrderController(
        ILogger<OrderController> logger,
        ICommon_Exception_Factory commonExFactory,
        IHttpContextAccessor httpContextAccessor,
        IOrderService orderService
    )
        : base(logger, commonExFactory, httpContextAccessor)
    {
        _orderService = orderService;
    }

    // [HttpGet]
    // public async Task<IActionResult> GetAllProducts()
    // {
    //     var products = await _orderService.GetAllProductsAsync();
    //     return Ok(products);
    // }
    [HttpOptions]
    public IActionResult Health()
    {
        return Ok("Order Controller Healthy.");
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] OrderReq order)
    {
        try
        {
            if (order == null) return BadRequest("Invalid order.");

            // Call the OrderService to place the order and publish to the queue
            await _orderService.PlaceOrderAsync(order);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
        return Ok("Order placed successfully.");
    }
}