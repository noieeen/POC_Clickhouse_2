using Core.Models;
using Core.Services.MessagingService;

namespace Core.Services.OrderService;

public class OrderService : IOrderService
{
    private readonly IRabbitMQPublisher<OrderReq> _rabbitMqPublisher;

    public OrderService(IRabbitMQPublisher<OrderReq> rabbitMqPublisher)
    {
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    public async Task PlaceOrderAsync(OrderReq order)
    {
        // Publish order to RabbitMQ for validation
        await _rabbitMqPublisher.PublishMessageAsync(order, RabbitMQQueues.OrderValidationQueue);
    }
}