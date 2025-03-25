using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ServiceA.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TraceLogController : ControllerBase
{
    private readonly ILogger<TraceLogController> _logger;
    private static readonly HttpClient client = new HttpClient();

    public class TraceRequest
    {
        public string path { get; set; }
    }

    [HttpPost]
    public async Task<IActionResult> TraceHttpCallback([FromBody] TraceRequest req)
    {
        try
        {
            var url = new Uri(req.path);

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                // _logger.LogInformation("Response: {Result}",response);
                return Ok(response);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }


        return BadRequest(req);
    }

    [HttpGet]
    // [Route("api/trace-http-callback-2")]
    public Dictionary<string, string> TraceHttpCallbackk()
    {
        var result = new Dictionary<string, string>();
        var activity = new Activity("ActivityInsideHttpRequest");
        activity.Start();
        result["TraceId"] = activity.Context.TraceId.ToString();
        result["ParentSpanId"] = activity.ParentSpanId.ToString();
        if (activity.Context.TraceState != null)
        {
            result["TraceState"] = activity.Context.TraceState;
        }

        activity.Stop();
        return result;
    }
}