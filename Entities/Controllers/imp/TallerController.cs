using BlumeApi.Models;
using BlumeAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlumeAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TallerController : ControllerBase
{

    private readonly ILogger<TallerController> _logger;
    private readonly ITallerService iTallerService;

    public TallerController(ILogger<TallerController> logger, ITallerService _iTallerService)
    {
        iTallerService = _iTallerService;
        _logger = logger;
    }

    [HttpGet("GetTalleres")]
    public async Task<IEnumerable<Taller>> Get()
    {
        List<Taller> talleres = await iTallerService.listarTalleresAsync();
        return talleres;
    }


}
