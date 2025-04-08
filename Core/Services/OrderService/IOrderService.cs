using Core.Models;

namespace Core.Services.OrderService;

public interface IOrderService
{
    Task PlaceOrderAsync(OrderReq order);
}