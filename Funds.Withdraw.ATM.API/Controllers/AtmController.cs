using Microsoft.AspNetCore.Mvc;

namespace EvDbSample.ATM.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AtmController : ControllerBase
{
    private readonly ILogger<AtmController> _logger;

    public AtmController(ILogger<AtmController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("124");
    }
}
