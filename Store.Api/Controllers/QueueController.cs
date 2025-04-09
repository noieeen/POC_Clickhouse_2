using Core.Services.MessagingService;
using Microsoft.AspNetCore.Mvc;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueController : ControllerBase
{
    private readonly IDynamicQueueConsumer _consumer;

    public QueueController(IDynamicQueueConsumer consumer)
    {
        _consumer = consumer;
    }

    [HttpPost()]
    public IActionResult ActiveQueues(string queueName)
    {
        var queues = _consumer.ListActiveQueues();
        return Ok($"Active queue: {queues.ToString()}.");
    }

    [HttpPost("consume/{queueName}")]
    public async Task<IActionResult> StartConsuming(string queueName)
    {
        await _consumer.ConsumeQueueAsync(queueName);
        return Ok($"Started consuming queue: {queueName}");
    }

    [HttpPost("close-consume/{queueName}")]
    public async Task<IActionResult> CloseConsuming(string queueName)
    {
        await _consumer.CloseQueueAsync(queueName);
        return Ok($"Close queue: {queueName} successfully.");
    }
}