using Core.Models;

namespace Core.Services.OrderService;

public interface IOrderService
{
    Task<Guid?> PlaceOrderAsync(OrderReq order);
}