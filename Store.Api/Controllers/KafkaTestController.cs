using Core.Services.DistributeService;
using Microsoft.AspNetCore.Mvc;

namespace Store.Api.Controllers;

[ApiController]
[Route("test/[controller]/[action]")]
public class KafkaTestController:ControllerBase
{
    private readonly IKafkaProducer _kafkaProducer;

    public KafkaTestController(IKafkaProducer kafkaProducer)
    {
        _kafkaProducer = kafkaProducer;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] string message)
    {
        await _kafkaProducer.ProduceAsync(message);
        return Ok("Message sent.");
    }
}