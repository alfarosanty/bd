using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TallerController : ControllerBase
{

    private readonly ILogger<TallerController> _logger;
    private readonly ITallerService _tallerService;

    public TallerController(ILogger<TallerController> logger, ITallerService tallerService)
    {
        _logger = logger;
        _tallerService = tallerService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<Taller>>> GetTalleres()
    {
        var talleres = await _tallerService.GetAll();
        if (talleres == null || talleres.Count == 0)
            return NoContent();

        return Ok(talleres);
    }


}
